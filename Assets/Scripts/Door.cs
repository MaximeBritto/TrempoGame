using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private int scoreValue = 1;
    [SerializeField] private Color normalColor = Color.blue;
    [SerializeField] private Color highlightColor = Color.cyan;
    
    private Material doorMaterial;
    private float highlightDuration = 0.2f;
    private float highlightEndTime;

    private void Awake()
    {
        // Créer une instance unique du matériau pour chaque porte
        doorMaterial = new Material(GetComponent<Renderer>().material);
        GetComponent<Renderer>().material = doorMaterial;
        doorMaterial.color = normalColor;
    }

    private void Update()
    {
        // Retour à la couleur normale après le highlight
        if (Time.time > highlightEndTime && doorMaterial.color != normalColor)
        {
            doorMaterial.color = normalColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FallingMario>(out FallingMario mario))
        {
            GameManager.Instance.AddScore(scoreValue);
            HighlightDoor();
            GetComponent<DoorEffect>()?.PlayEffect();
            Destroy(mario.gameObject);
        }
    }

    private void HighlightDoor()
    {
        doorMaterial.color = highlightColor;
        highlightEndTime = Time.time + highlightDuration;
    }
} 