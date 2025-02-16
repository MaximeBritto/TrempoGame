using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class FallingMario : MonoBehaviour
{
    private Rigidbody rb;
    private BoxCollider boxCollider;
    private MarioModelController modelController;
    
    [SerializeField] private float maxVelocity = 15f;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        modelController = GetComponentInChildren<MarioModelController>();
        
        rb.constraints = RigidbodyConstraints.FreezePositionZ | 
                        RigidbodyConstraints.FreezeRotationX | 
                        RigidbodyConstraints.FreezeRotationY |
                        RigidbodyConstraints.FreezeRotationZ;
        
        boxCollider.isTrigger = false;
        boxCollider.size = new Vector3(0.5f, 0.5f, 0.5f);
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > maxVelocity)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
        }
    }

    public void Bounce(Vector3 bounceForce)
    {
        rb.linearVelocity = bounceForce;
        modelController?.TriggerJumpAnimation();
        Debug.Log("Bounce with force: " + bounceForce);
    }

    private void OnBecameInvisible()
    {
        if (transform.position.y < Camera.main.transform.position.y - Camera.main.orthographicSize)
        {
            transform.parent = null;
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Mario collision avec : " + collision.gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Mario trigger avec : " + other.gameObject.name);
    }
} 