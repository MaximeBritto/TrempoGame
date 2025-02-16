using UnityEngine;
using System.Collections.Generic;

public class TrampolineDrawer : MonoBehaviour
{
    [SerializeField] private LineRenderer trampolinePrefab;
    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private float trampolineLifetime = 3f; // Durée de vie du trampoline
    [SerializeField] private float trampolineWidth = 0.5f; // Largeur de la zone de collision
    
    private List<TrampolineLine> activeLines = new List<TrampolineLine>();
    private LineRenderer currentLine;
    private Vector3 startPos;

    private void Update()
    {
        HandleInput();
        CheckCollisions();
        RemoveOldLines();
    }

    private void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10f));
            touchPos.z = 0f;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    StartLine(touchPos);
                    break;
                case TouchPhase.Moved:
                    UpdateLine(touchPos);
                    break;
                case TouchPhase.Ended:
                    EndLine();
                    break;
            }
        }
    }

    private void StartLine(Vector3 position)
    {
        currentLine = Instantiate(trampolinePrefab);
        startPos = position;
        currentLine.SetPosition(0, startPos);
        currentLine.SetPosition(1, startPos);
    }

    private void UpdateLine(Vector3 position)
    {
        if (currentLine != null)
        {
            currentLine.SetPosition(1, position);
        }
    }

    private void EndLine()
    {
        if (currentLine != null)
        {
            Vector3 endPos = currentLine.GetPosition(1);
            Vector3 direction = (endPos - startPos).normalized;
            
            // Ajouter un BoxCollider au trampoline
            BoxCollider collider = currentLine.gameObject.AddComponent<BoxCollider>();
            Vector3 center = (startPos + endPos) / 2f;
            float length = Vector3.Distance(startPos, endPos);
            
            collider.center = center - currentLine.transform.position;
            collider.size = new Vector3(length, trampolineWidth, 0.1f);
            
            // Rotation du collider pour qu'il s'aligne avec la ligne
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            currentLine.transform.rotation = Quaternion.Euler(0, 0, angle);
            
            activeLines.Add(new TrampolineLine(currentLine, startPos, endPos, direction, Time.time));
            currentLine = null;
        }
    }

    private void CheckCollisions()
    {
        foreach (var line in activeLines)
        {
            Collider[] colliders = Physics.OverlapBox(
                (line.StartPos + line.EndPos) / 2f,
                new Vector3(Vector3.Distance(line.StartPos, line.EndPos) / 2f, trampolineWidth / 2f, 0.05f),
                Quaternion.Euler(0, 0, Mathf.Atan2(line.Direction.y, line.Direction.x) * Mathf.Rad2Deg)
            );

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<FallingMario>(out FallingMario mario))
                {
                    Vector3 bounceDirection = Quaternion.Euler(0, 0, 45) * line.Direction;
                    mario.Bounce(bounceDirection * bounceForce);
                }
            }
        }
    }

    private void RemoveOldLines()
    {
        activeLines.RemoveAll(line => 
        {
            if (Time.time - line.CreationTime > trampolineLifetime)
            {
                Destroy(line.LineRenderer.gameObject);
                return true;
            }
            return false;
        });
    }
}

public class TrampolineLine
{
    public LineRenderer LineRenderer { get; private set; }
    public Vector3 StartPos { get; private set; }
    public Vector3 EndPos { get; private set; }
    public Vector3 Direction { get; private set; }
    public float CreationTime { get; private set; }

    public TrampolineLine(LineRenderer lineRenderer, Vector3 startPos, Vector3 endPos, Vector3 direction, float creationTime)
    {
        LineRenderer = lineRenderer;
        StartPos = startPos;
        EndPos = endPos;
        Direction = direction;
        CreationTime = creationTime;
    }
} 