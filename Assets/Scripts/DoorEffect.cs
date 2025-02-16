using UnityEngine;

public class DoorEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem successParticles;

    private void Start()
    {
        // Créer le système de particules
        if (successParticles == null)
        {
            CreateParticleSystem();
        }
    }

    public void PlayEffect()
    {
        successParticles.Play();
    }

    private void CreateParticleSystem()
    {
        GameObject particleObj = new GameObject("SuccessParticles");
        particleObj.transform.parent = transform;
        particleObj.transform.localPosition = Vector3.zero;
        
        successParticles = particleObj.AddComponent<ParticleSystem>();
        var main = successParticles.main;
        main.duration = 0.5f;
        main.startSize = 0.2f;
        main.startSpeed = 3f;
        main.startColor = Color.yellow;
        
        var emission = successParticles.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[]{ new ParticleSystem.Burst(0f, 10) });
    }
} 