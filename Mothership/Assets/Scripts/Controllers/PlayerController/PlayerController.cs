using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Vector3 cameraOffset = Vector3.zero;
    [SerializeField]
    private float movementSpeed = 1f;
    [SerializeField]
    private float rotateSpeed = 10f;

    private Animator animator = null;

    bool IsRunningLocally { get { return !Network.isClient && !Network.isServer; } }

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsRunningLocally || networkView.isMine)
        {
            // align camera to player
            Camera.main.transform.position = gameObject.transform.position + cameraOffset;

            Move();

        }
    }

    private void Move()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetDir = new Vector3(hit.point.x, transform.position.y, hit.point.z) - transform.position;
            float step = rotateSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0f);
            Debug.DrawRay(transform.position, newDir, Color.red);
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        float fTranslation = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        float sTranslation = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;

        if(fTranslation != 0 && sTranslation != 0)
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
            }
            else
            {
                animator.SetBool("bIsMoving", false);
            }

            if (sTranslation > 0)
            {
                animator.SetBool("bIsTurningR", true);
                animator.SetBool("bIsTurningL", false);
            }
            else if (sTranslation < 0)
            {
                animator.SetBool("bIsTurningL", true);
                animator.SetBool("bIsTurningR", false);
            }
            else
            {
                animator.SetBool("bIsTurningL", false);
                animator.SetBool("bIsTurningR", false);
            }
        }
    }
}
