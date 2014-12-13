using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MothershipStateMachine;

namespace Mothership
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private Vector3 cameraOffset = Vector3.zero;
        [SerializeField]
        private float rotateSpeed = 10f;

        [SerializeField]
        private GameObject gun = null;

        public Animator animator { get; private set; }
        public Dictionary<int, AnimatorBoolProperty> animatorStates;

        private static CPowerUpSO itemsResource = null;
        private Dictionary<string, GameObject> projectilePrefabs = new Dictionary<string, GameObject>();

        private static List<PlayerController> m_liControllers = new List<PlayerController>();
        public static List<PlayerController> PlayerControllers { get { return m_liControllers; } }

        private ServerManager serverManager = null;

        float lastFireTime = 0f;

        //private Dictionary<string, int> inventory = new Dictionary<string, int>();

        // For debug
        bool IsRunningLocally { get { return !Network.isClient && !Network.isServer; } }

        // Use this for initialization
        void Awake()
        {
            if (Network.isServer)
            {
                m_liControllers.Add(this);
                serverManager = RoleManager.roleManager as ServerManager;
            }

            animator = GetComponent<Animator>();
            animatorStates = new Dictionary<int, AnimatorBoolProperty>();
            animatorStates.Add(0, new AnimatorBoolProperty() { Name = "bIsMoving", State = false }); // Moving
            animatorStates.Add(1, new AnimatorBoolProperty() { Name = "bIsTurningL", State = false }); // TurnL
            animatorStates.Add(2, new AnimatorBoolProperty() { Name = "bIsTurningR", State = false }); // TurnR

            itemsResource = Resources.Load<CPowerUpSO>(ResourcePacks.RESOURCE_CONTAINER_ITEMS);
            projectilePrefabs = itemsResource.Weapons;
            //inventory.Add(Names.NAME_BULLET, 500);
        }

        // Update is called once per frame
        void Update()
        {
            // If this client is the owner of the avatar then use input to alter it's movement and animation
            if (IsRunningLocally || networkView.isMine)
            {
                // align camera to player
                Camera.main.transform.position = gameObject.transform.position + cameraOffset;

                Move();
                Debug.DrawLine(transform.position + Vector3.up, transform.position + transform.forward * 50 + Vector3.up, Color.red);

                if (Input.GetButtonDown("Fire1") && lastFireTime <= Time.time - Constants.PROJECTILE_DELAY_BULLET)
                {
                    if (!IsRunningLocally)
                    {
                        networkView.RPC("Fire", RPCMode.All, Names.NAME_BULLET, gun.transform.position, transform.forward);
                    }
                    else
                    {
                        Fire(Names.NAME_BULLET, gun.transform.position, transform.forward);
                    }

                    lastFireTime = Time.time;
                }
            }
        }

        #region Local

        private void Move()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 targetDir = new Vector3(hit.point.x, transform.position.y, hit.point.z) - transform.position;
                float step = rotateSpeed * Time.deltaTime;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0f);
                transform.rotation = Quaternion.LookRotation(newDir);
            }

            float fTranslation = Input.GetAxis("Vertical") * Constants.DEFAULT_SPEED_DRONE * Time.deltaTime;
            float sTranslation = Input.GetAxis("Horizontal") * Constants.DEFAULT_SPEED_DRONE * Time.deltaTime;

            if (fTranslation != 0 && sTranslation != 0)
            {
                fTranslation *= 0.75f;
                sTranslation *= 0.75f;
            }

            transform.Translate(sTranslation, 0, fTranslation);

            AnimateMovement(fTranslation, sTranslation);
        }

        private void AnimateMovement(float fTranslation, float sTranslation)
        {
            if (animator != null)
            {
                if (fTranslation > 0)
                {
                    animator.SetBool("bIsMoving", true);
                    animatorStates[0].State = true;
                }
                else
                {
                    animator.SetBool("bIsMoving", false);
                    animatorStates[0].State = false;
                }

                if (sTranslation > 0)
                {
                    animator.SetBool("bIsTurningR", true);
                    animator.SetBool("bIsTurningL", false);
                    animatorStates[2].State = true;
                    animatorStates[1].State = false;
                    animatorStates[0].State = false;
                }
                else if (sTranslation < 0)
                {
                    animator.SetBool("bIsTurningL", true);
                    animator.SetBool("bIsTurningR", false);
                    animatorStates[1].State = true;
                    animatorStates[2].State = false;
                    animatorStates[0].State = false;
                }
                else
                {
                    animator.SetBool("bIsTurningL", false);
                    animator.SetBool("bIsTurningR", false);
                    animatorStates[1].State = false;
                    animatorStates[2].State = false;
                }

            }
        }
        #endregion

        #region Remote

        public void CurrentAnimationFlag(int index)
        {
            if (index >= 0)
            {
                animatorStates[index].State = true;
                animator.SetBool(animatorStates[index].Name, true);

                var otherFlags = animatorStates.Where(s => s.Key != index);
                foreach (var flag in otherFlags)
                {
                    animatorStates[flag.Key].State = false;
                    animator.SetBool(flag.Value.Name, false);
                }
            }
            else
            {
                foreach (var flag in animatorStates)
                {
                    animatorStates[flag.Key].State = false;
                    animator.SetBool(flag.Value.Name, false);
                }
            }
        }

        [RPC]
        protected void Fire(string projectileName, Vector3 position, Vector3 direction)
        {
            GameObject goProjectile;
            if (projectilePrefabs.TryGetValue(projectileName, out goProjectile))
            {
                CProjectile cProjectile = goProjectile.GetComponent<CProjectile>();

                cProjectile.Direction = direction;
                cProjectile.Instantiator = gameObject;
                cProjectile.Activation = true;
                cProjectile.FiringPosition = position;
                goProjectile.name = Names.NAME_BULLET;

                Instantiate(goProjectile, gun.transform.position, goProjectile.transform.rotation);
            }
            else
            {
                Debug.LogError("Unrecognised projectile name");
            }
        }

        private void OnCollisionEnter(Collision cCollision)
        {
            // Only react on server and results will be send to client when processed
            if (Network.isServer)
            {
                // Get a handle on the gameObject with which we collided. 
                GameObject goObject = cCollision.gameObject;

                // Check if we collided with a base
                if (goObject.tag == Tags.TAG_BASE)
                {
                    // Base collision logic - deliver flag / heal if own base
                }

                // Depending on the name of the object, react accordingly.
                switch (goObject.tag)
                {
                    case Tags.TAG_WEAPON:

                        // Get the name and team of the attacker using its projectile.
                        CProjectile cProjectile = goObject.GetComponent<CProjectile>();

                        if (null == cProjectile.Instantiator.gameObject)
                            return;

                        //// Set the name.
                        string strAttackerName = "";
                        IAIBase.ETeam eTeam = IAIBase.ETeam.TEAM_NONE;

                        // Check if attacker is another client
                        GameObject goAttacker = cProjectile.Instantiator;
                        PlayerController aPC = goAttacker.GetComponent<PlayerController>();
                        if (aPC != null)
                        {
                            ClientDataOnServer cd = serverManager.RegisteredClients.First(c => c.NetworkPlayer == aPC.networkView.owner);
                            strAttackerName = cd.Profile.DisplayName;
                            eTeam = cd.ClientTeam;
                        }
                        else // attacker is an AI
                        {
                            strAttackerName = "AI";
                            //// Find the team using the attacker's name.
                            if (strAttackerName == Names.NAME_AI_DRONE_BLUE + "(Clone)")
                                eTeam = IAIBase.ETeam.TEAM_BLUE;

                            else if (strAttackerName == Names.NAME_AI_DRONE_RED + "(Clone)")
                                eTeam = IAIBase.ETeam.TEAM_RED;
                        }

                        serverManager.SendGameMessage(new PlayerTakenDamage() { Player = networkView.owner, Damage = (int)cProjectile.Damage, Attacker = strAttackerName, AttackerTeam = eTeam });

                        break;
                }
            }
        }

        private void OnDestroy()
        {
            m_liControllers.Remove(this);
        }

        #endregion
    }

    public class AnimatorBoolProperty
    {
        public string Name { get; set; }
        public bool State { get; set; }
    }
}