using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Vector3 cameraOffset = Vector3.zero;
    [SerializeField]
    private float movementSpeed = 30f;
    [SerializeField]
    private float rotateSpeed = 10f;

    public Animator animator { get; private set; }
    public Dictionary<int, AnimatorBoolProperty> animatorStates;

    bool IsRunningLocally { get { return !Network.isClient && !Network.isServer; } }

    // Data from server if this is running on replication
    private float errorThreshold = 0.2f;
    public Vector3 inPosition { get; set; }
    public Quaternion inRotation { get; set; }

    // Use this for initialization
    void Awake()
    {
        animator = GetComponent<Animator>();
        animatorStates = new Dictionary<int, AnimatorBoolProperty>();
        animatorStates.Add(0, new AnimatorBoolProperty() { Name = "bIsMoving", State = false }); // Moving
        animatorStates.Add(1, new AnimatorBoolProperty() { Name = "bIsTurningL", State = false }); // TurnL
        animatorStates.Add(2, new AnimatorBoolProperty() { Name = "bIsTurningR", State = false }); // TurnR
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

        float fTranslation = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        float sTranslation = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;

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
        if(index >= 0)
        {
            animatorStates[index].State = true;
            animator.SetBool(animatorStates[index].Name, true);

            var otherFlags = animatorStates.Where(s => s.Key != index);
            foreach(var flag in otherFlags)
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
    #endregion
}

public class AnimatorBoolProperty
{
    public string Name { get; set; }
    public bool State { get; set; }
}