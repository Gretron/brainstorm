using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy Suspicion States
/// </summary>
public enum Suspicion
{
    None = 0, // Patrol
    Curious = 1, // Player Glimpse
    Alerted = 2 // Player Seen
}

/// <summary>
/// Enemy Patroling Behaviour
/// </summary>
public class EnemySuspicion : MonoBehaviour
{
    [Header("References")]
    /// <summary>
    /// Player Reference
    /// </summary>
    [SerializeField]
    private GameObject player;

    /// <summary>
    /// NavMeshAgent Component Reference
    /// </summary>
    private NavMeshAgent agent;

    [Header("Detection Values")]
    /// <summary>
    /// Maximum Detection Angle
    /// </summary>
    [SerializeField]
    private float maxAngle = 45;

    /// <summary>
    /// Maximum Distance Angle
    /// </summary>
    [SerializeField]
    private float maxDistance = 10;

    /// <summary>
    /// Counter to Track Detection Progress
    /// </summary>
    private float detectedCounter;

    /// <summary>
    /// Threshold for Detection
    /// </summary>
    private float detectedThreshold = 1;

    /// <summary>
    /// Terminal Counter
    /// </summary>
    private float terminalCounter;

    /// <summary>
    /// Terminal Complete Value
    /// </summary>
    private float terminalComplete = 2;

    /// <summary>
    /// Enum to Determine Suspicion
    /// </summary>
    private Suspicion suspicion = Suspicion.None;

    // [SerializeField] private float timer = 1.0f;
    // [SerializeField] private float visionCheckRate = 1.0f;

    [Header("Patrol Values")]
    /// <summary>
    /// Points to Patrol At
    /// </summary>
    [SerializeField]
    private Transform[] points;

    /// <summary>
    /// Current Patroling Point
    /// </summary>
    private int currentPatrolPoint = 0;

    /// <summary>
    /// Called Before First Frame Update
    /// </summary>
    void Start()
    {
        // Get References
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();

        agent.autoBraking = true;

        // Subscribe to Terminal Alert Event
        GameManager.Instance.TerminalAlertEvent.AddListener(TerminalAlertHandler);
    }

    /// <summary>
    /// Called Once per Frame
    /// </summary>
    void Update()
    {
        // If Alerted...
        if (suspicion == Suspicion.Alerted)
        {
            // If At Terminal...
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                // If Terminal Counter Completed...
                if (terminalCounter >= terminalComplete)
                {
                    GameManager.Instance.TerminalAlertEvent.Invoke();
                }

                terminalCounter += Time.deltaTime;

                return;
            }
            else
            {
                return;
            }
        }

        // If Player Is Detected...
        if (PlayerDetected())
        {
            // Set Destination to Player and Speed Up
            agent.destination = player.transform.position;
            agent.speed = 5;

            detectedCounter = Mathf.Min(detectedCounter + Time.deltaTime, detectedThreshold);

            if (detectedCounter >= detectedThreshold)
            {
                suspicion = Suspicion.Alerted;
                agent.destination = FindClosestTerminal().transform.position;
            }
        }
        else
        {
            detectedCounter = Mathf.Max(detectedCounter - Time.deltaTime, 0);

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                PatrolNextPoint();
                agent.speed = 3;
            }
        }
    }

    /// <summary>
    /// Verify If Player Is Detected
    /// </summary>
    /// <returns> True or False Based on If Player Is Detected </returns>
    public bool PlayerDetected()
    {
        // Compare Distance Between Player and Enemy
        Vector3 playerToTurret = player.transform.position - transform.position;

        // If Distance Is Smaller Than Threshold...
        if (playerToTurret.magnitude > maxDistance)
        {
            return false;
        }

        // Normalize Vector Between Player and Enemy
        Vector3 normPlayerToTurret = Vector3.Normalize(playerToTurret);

        // Extract Angle Between Enemy Forward Vector and Vector Between Enemy and Player
        float dotProduct = Vector3.Dot(transform.forward, normPlayerToTurret);
        float angle = Mathf.Acos(dotProduct);
        float deg = angle * Mathf.Rad2Deg;

        // If Extracted Angle Is Within Threshold...
        if (deg < maxAngle)
        {
            // Send Raycast to Player
            RaycastHit hit;
            Ray ray = new Ray(transform.position, normPlayerToTurret);

            // If Raycast Hits Player...
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Player")
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Start Patroling Next Patrol Point
    /// </summary>
    void PatrolNextPoint()
    {
        // If There Are No Points...
        if (points.Length == 0)
            return;

        // Change Patrol Point
        agent.destination = points[currentPatrolPoint].position;

        // Set New Current Patrol Point
        currentPatrolPoint = (currentPatrolPoint + 1) % points.Length;
    }

    /// <summary>
    /// Find Terminal Closest to Enemy
    /// </summary>
    /// <returns> Closest Terminal </returns>
    GameObject FindClosestTerminal()
    {
        // Find All Terminal GameObjects in Scene
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Terminal");

        float previousDistance = Mathf.Infinity;
        Vector3 position = transform.position;

        GameObject closest = null;

        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float currentDistance = diff.sqrMagnitude;

            // If Current Distance Is Smaller Than Previous Distance...
            if (currentDistance < previousDistance)
            {
                closest = go;
                previousDistance = currentDistance;
            }
        }

        return closest;
    }

    /// <summary>
    /// Get Detected Percentage
    /// </summary>
    /// <returns> Value from 0 to 1 </returns>
    public float GetPercentageDetected()
    {
        return detectedCounter / detectedThreshold;
    }

    /// <summary>
    /// Handle Terminal Alert Event
    /// </summary>
    public void TerminalAlertHandler()
    {
        // Set Suspiciton to Alerted
        suspicion = Suspicion.Alerted;

        // Max Detected Counter
        detectedCounter = detectedThreshold;

        // Set Destination to Player
        agent.destination = player.transform.position;
    }
}
