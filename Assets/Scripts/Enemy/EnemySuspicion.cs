using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy Suspicion States
/// </summary>
public enum Suspicion
{
    Patrol = 0, // Patrol
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
    private float spottedCounter = 0;

    /// <summary>
    /// Counter to Track Detection Progress
    /// </summary>
    public float SpottedCounter
    {
        get { return spottedCounter; }
        private set { spottedCounter = value; }
    }

    /// <summary>
    /// Threshold for Detection
    /// </summary>
    public float SpottedThreshold { get; private set; } = 3;

    /// <summary>
    /// Counter to Track Detection Progress
    /// </summary>
    private float detectedCounter = 0;

    /// <summary>
    /// Counter to Track Detection Progress
    /// </summary>
    public float DetectedCounter
    {
        get { return detectedCounter; }
        private set { detectedCounter = value; }
    }

    /// <summary>
    /// Threshold for Detection
    /// </summary>
    public float DetectedThreshold { get; private set; } = 3;

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
    public Suspicion suspicion = Suspicion.Patrol;

    /// <summary>
    /// Last Position of Player
    /// </summary>
    private Vector3 lastPlayerPosition;

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
    /// Coroutine to Resume Patrolling
    /// </summary>
    private IEnumerator resumePatrolCoroutine;

    /// <summary>
    /// Coroutine to Go to Last Known Player Location
    /// </summary>
    private IEnumerator playerLastLocationCoroutine;

    /// <summary>
    /// Coroutine to Countdown to Being Not Curious
    /// </summary>
    private IEnumerator curiousCooldownCoroutine;

    /// <summary>
    /// Boolean for If Player Is Visible or Not
    /// </summary>
    private bool playerVisible = false;

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

        // Get Patrol Points in Scene
        points = GameObject
            .FindGameObjectsWithTag("PatrolPoint")
            .Select(point => point.transform)
            .ToArray();
    }

    /// <summary>
    /// Called Once per Frame
    /// </summary>
    void Update()
    {
        // Get Player Visibility
        playerVisible = PlayerVisible();

        // If Suspicion Is Alerted...
        if (suspicion == Suspicion.Alerted)
        {
            // If At Terminal...
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                // If Terminal Counter Completed...
                if (terminalCounter >= terminalComplete)
                {
                    GameManager.Instance.TerminalAlertEvent.Invoke(lastPlayerPosition);
                }

                terminalCounter += Time.deltaTime;

                return;
            }
            else
            {
                return;
            }
        }
        // If Suspicion Is Curious...
        else if (suspicion == Suspicion.Curious)
        {
            // If Player Is Detected...
            if (PlayerVisible())
            {
                agent.isStopped = false;
                agent.destination = player.transform.position;

                if (IncrementCounter(ref detectedCounter, DetectedThreshold, Time.deltaTime))
                {
                    suspicion = Suspicion.Alerted;
                    agent.destination = FindClosestTerminal().transform.position;
                    lastPlayerPosition = player.transform.position;
                }

                // Reset Curious Cooldown
                if (curiousCooldownCoroutine != null)
                {
                    StopCoroutine(curiousCooldownCoroutine);
                    curiousCooldownCoroutine = null;
                }

                // Stop Looking Last Player Location Check
                if (playerLastLocationCoroutine != null)
                {
                    StopCoroutine(playerLastLocationCoroutine);
                    playerLastLocationCoroutine = null;
                }
            }
            else
            {
                // If Not Going to Look for Last Location...
                if (playerLastLocationCoroutine == null)
                {
                    // Initiate Last Location Check
                    playerLastLocationCoroutine = GoLastPlayerLocation(1);
                    StartCoroutine(playerLastLocationCoroutine);
                }

                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    PatrolNextPoint();
                    agent.speed = 3;
                }

                IncrementCounter(ref detectedCounter, DetectedThreshold, -Time.deltaTime);

                if (curiousCooldownCoroutine == null)
                {
                    curiousCooldownCoroutine = CuriousCooldown(5);
                    StartCoroutine(curiousCooldownCoroutine);
                }
            }
        }
        // Else No Suspicion...
        else
        {
            // If Player Is Detected...
            if (PlayerVisible())
            {
                // Stop Enemy
                agent.isStopped = true;

                // Resume Agent Movement
                if (resumePatrolCoroutine != null)
                {
                    StopCoroutine(resumePatrolCoroutine);
                }

                resumePatrolCoroutine = ResumePatrolling(1);
                StartCoroutine(resumePatrolCoroutine);

                if (IncrementCounter(ref spottedCounter, SpottedThreshold, Time.deltaTime))
                {
                    suspicion = Suspicion.Curious;
                    agent.destination = FindClosestTerminal().transform.position;
                }

                // Look at Player
                Quaternion rotation = Quaternion.LookRotation(
                    player.transform.position - transform.position
                );

                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    rotation,
                    Time.deltaTime * 10
                );
            }
            else
            {
                IncrementCounter(ref spottedCounter, SpottedThreshold, -Time.deltaTime);

                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    PatrolNextPoint();
                    agent.speed = 3;
                }
            }
        }
    }

    #region State Handlers

    private void AlertedHandler() { }

    private void CuriousHandler() { }

    private void PatrolHandler() { }

    #endregion

    #region Environment Checks

    /// <summary>
    /// Verify If Player Is Detected
    /// </summary>
    /// <returns> True or False Based on If Player Is Detected </returns>
    public bool PlayerVisible()
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

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handle Terminal Alert Event
    /// </summary>
    public void TerminalAlertHandler(Vector3 position)
    {
        // Set Suspiciton to Alerted
        suspicion = Suspicion.Alerted;

        // Max Detected Counter
        SpottedCounter = SpottedThreshold;

        // Set Destination to Player
        agent.destination = position;
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// Coroutine to Resume Patrolling After Delay
    /// </summary>
    /// <param name="delay"> Delay in Seconds </param>
    IEnumerator ResumePatrolling(float delay)
    {
        // Apply Delay
        yield return new WaitForSeconds(delay);

        // If Patrolling...
        if (suspicion == Suspicion.Patrol)
            // Resume Patrolling
            agent.isStopped = false;

        yield return null;
    }

    /// <summary>
    /// Coroutine to Go to Last Player Location After Delay
    /// </summary>
    /// <param name="delay"> Delay in Seconds </param>
    IEnumerator GoLastPlayerLocation(float delay)
    {
        // Apply Delay
        yield return new WaitForSeconds(delay);

        // If Still Curious...
        if (suspicion == Suspicion.Curious)
        {
            // Resume and Go to Player Location
            agent.isStopped = false;
            agent.destination = player.transform.position;
        }

        yield return null;
    }

    /// <summary>
    /// Coroutine for Curiousity Cooldown After Delay
    /// </summary>
    /// <param name="delay"> Delay in Seconds </param>
    IEnumerator CuriousCooldown(float delay)
    {
        // Apply Delay
        yield return new WaitForSeconds(delay);

        // If Suspicion Curious...
        if (suspicion == Suspicion.Curious)
            // Switch Back to Patrolling
            suspicion = Suspicion.Patrol;

        yield return null;
    }

    #endregion

    #region Helpers

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
    /// Increase Counter by Increment
    /// </summary>
    /// <param name="counter"> Value to Increase </param>
    /// <param name="threshold"> Max Value </param>
    /// <param name="increment"> Increment Value </param>
    /// <returns> True or False Based on If Counter Is At Threshold </returns>
    private bool IncrementCounter(ref float counter, float threshold, float increment)
    {
        counter = Mathf.Max(Mathf.Min(counter + increment, threshold), 0);

        if (counter >= threshold)
        {
            return true;
        }

        return false;
    }

    #endregion
}
