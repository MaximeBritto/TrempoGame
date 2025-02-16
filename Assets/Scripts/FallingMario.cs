using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingMario : MonoBehaviour
{
    private Rigidbody rb;
    
    [SerializeField] private float maxVelocity = 15f;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Limiter la vitesse
        if (rb.linearVelocity.magnitude > maxVelocity)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
        }
    }

    public void Bounce(Vector3 bounceForce)
    {
        rb.linearVelocity = bounceForce;
    }

    private void OnBecameInvisible()
    {
        // Détruire Mario s'il sort de l'écran par le bas
        if (transform.position.y < Camera.main.transform.position.y - Camera.main.orthographicSize)
        {
            Destroy(gameObject);
        }
    }
} 