using UnityEngine;
using System.Collections;

public class TrampolineBehaviour : MonoBehaviour
{
    private float bounceForce;
    private Vector3 direction;
    private LineRenderer lineRenderer;
    private bool isFading = false;
    private float fadeOutDuration = 0.5f; // Durée du fade out en secondes
    private float trampolineLength;
    private BoxCollider trampolineCollider;
    
    [SerializeField] private float maxBounceMultiplier = 1.5f;  // 150% de la force pour le centre
    [SerializeField] private float minBounceMultiplier = 0.5f;  // 50% de la force pour les extrémités
    [SerializeField] private float smallTrampolineThreshold = 2f; // Seuil pour considérer un trampoline comme petit

    public void Initialize(float force, Vector3 dir)
    {
        bounceForce = force;
        direction = dir;
        lineRenderer = GetComponent<LineRenderer>();
        trampolineCollider = GetComponent<BoxCollider>();
        
        // S'assurer que le collider est en trigger
        trampolineCollider.isTrigger = true;
        
        // Ajuster la taille du collider
        trampolineLength = Vector3.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1));
        trampolineCollider.size = new Vector3(trampolineLength, 0.5f, 1f);
        
        // Activer les collisions
        Physics.IgnoreLayerCollision(gameObject.layer, gameObject.layer, true);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision détectée avec : " + other.gameObject.name); // Debug
        
        if (!isFading && other.TryGetComponent<FallingMario>(out FallingMario mario))
        {
            // Calculer la position relative sur le trampoline
            float bounceMultiplier = CalculateBounceMultiplier(other.transform.position);
            
            // Calculer l'angle de réflexion
            Vector3 normal = new Vector3(-direction.y, direction.x, 0).normalized;
            Vector3 incomingVelocity = mario.GetComponent<Rigidbody>().linearVelocity;
            Vector3 reflection = Vector3.Reflect(incomingVelocity, normal);
            
            // Appliquer la force de rebond ajustée
            float adjustedBounceForce = bounceForce * bounceMultiplier;
            reflection = (reflection + Vector3.up * adjustedBounceForce).normalized * adjustedBounceForce;
            
            mario.Bounce(reflection);
            
            // Commencer le fade out
            StartCoroutine(FadeOutAndDestroy());
            
            // Debug visuel
            Debug.DrawRay(other.transform.position, normal, Color.blue, 1f);
            Debug.DrawRay(other.transform.position, reflection, Color.red, 1f);
        }
    }

    private float CalculateBounceMultiplier(Vector3 hitPosition)
    {
        // Si le trampoline est petit, toujours donner le maximum de force
        if (trampolineLength < smallTrampolineThreshold)
        {
            return maxBounceMultiplier;
        }

        // Convertir la position de hit en position locale du trampoline
        Vector3 localHitPos = transform.InverseTransformPoint(hitPosition);
        
        // Calculer la distance relative au centre (en pourcentage)
        float distanceFromCenter = Mathf.Abs(localHitPos.x) / (trampolineLength * 0.5f);
        distanceFromCenter = Mathf.Clamp01(distanceFromCenter);

        // Interpoler entre max et min bounce
        // 0 = centre (max bounce), 1 = extrémité (min bounce)
        float multiplier = Mathf.Lerp(maxBounceMultiplier, minBounceMultiplier, distanceFromCenter);

        // Debug visuel
        Color debugColor = Color.Lerp(Color.green, Color.red, distanceFromCenter);
        Debug.DrawRay(hitPosition, Vector3.up * multiplier, debugColor, 1f);

        return multiplier;
    }

    private IEnumerator FadeOutAndDestroy()
    {
        isFading = true;
        float elapsedTime = 0;
        Color startColor = lineRenderer.material.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1 - (elapsedTime / fadeOutDuration);
            lineRenderer.material.color = Color.Lerp(endColor, startColor, alpha);
            yield return null;
        }

        // Informer le TrampolineDrawer que ce trampoline est détruit
        TrampolineDrawer.Instance.RemoveTrampolineLine(this);
        Destroy(gameObject);
    }
} 