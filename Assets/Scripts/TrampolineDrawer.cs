using UnityEngine;
using System.Collections.Generic;

public class TrampolineDrawer : MonoBehaviour
{
    public static TrampolineDrawer Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private LineRenderer trampolinePrefab;
    [SerializeField] private float bounceForce = 10f; // Force de base du saut
    [SerializeField] private float trampolineLifetime = 3f; // Durée de vie du trampoline
    [SerializeField] private float trampolineWidth = 0.5f; // Largeur de la zone de collision
    
    private List<TrampolineLine> activeLines = new List<TrampolineLine>();
    private LineRenderer currentLine;
    private Vector3 startPos;

    private void Update()
    {
        HandleInput();
        RemoveOldLines();
    }

    private void HandleInput()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        // Gestion de la souris uniquement sur PC
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            mousePos.z = 0f;
            StartLine(mousePos);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            mousePos.z = 0f;
            UpdateLine(mousePos);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndLine();
        }
        #else
        // Gestion du tactile uniquement sur mobile
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
        #endif
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
            
            // Vérifier si la ligne est assez longue
            if (Vector3.Distance(startPos, endPos) < 0.1f)
            {
                Destroy(currentLine.gameObject);
                return;
            }

            Vector3 direction = (endPos - startPos).normalized;
            
            // Ajouter un BoxCollider au trampoline
            BoxCollider collider = currentLine.gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            
            float length = Vector3.Distance(startPos, endPos);
            
            // Positionner le GameObject au centre de la ligne
            currentLine.transform.position = (startPos + endPos) / 2f;
            
            // Configurer le collider
            collider.size = new Vector3(length, trampolineWidth, 0.1f);
            collider.center = Vector3.zero;
            
            // Rotation du trampoline
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            currentLine.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Ajouter et configurer le TrampolineBehaviour
            var behaviour = currentLine.gameObject.AddComponent<TrampolineBehaviour>();
            behaviour.Initialize(bounceForce, direction);
            
            activeLines.Add(new TrampolineLine(currentLine, startPos, endPos, direction, Time.time));
            currentLine = null;
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

    public void RemoveTrampolineLine(TrampolineBehaviour behaviour)
    {
        activeLines.RemoveAll(line => line.LineRenderer.gameObject == behaviour.gameObject);
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