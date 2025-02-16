using UnityEngine;

public class MarioModelController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 360f;
    private Rigidbody parentRb;
    private Animator animator;

    private void Start()
    {
        parentRb = GetComponentInParent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateRotation();
        UpdateAnimations();
    }

    private void UpdateRotation()
    {
        if (Mathf.Abs(parentRb.linearVelocity.y) > 0.1f)
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
    }

    private void UpdateAnimations()
    {
        if (animator != null)
        {
            bool isFalling = parentRb.linearVelocity.y < -0.1f;
            bool isJumping = parentRb.linearVelocity.y > 0.1f;
            
            animator.SetBool("isFalling", isFalling);
            animator.SetBool("isJumping", isJumping);
        }
    }

    public void TriggerJumpAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("jump");
        }
    }
} 