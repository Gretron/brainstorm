using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy Type Enum
/// </summary>
public enum EnemyType
{
    Scientist = 0,
    Guard = 1,
}

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
/// Enemy Suspicion Behaviour
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

    [SerializeField]
    /// <summary>
    /// Layer Ignored by Raycast
    /// </summary>
    private LayerMask ignoreLayer;

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
    /// Type of Enemy
    /// </summary>
    [SerializeField]
    public EnemyType enemyType = EnemyType.Scientist;

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
    /// If Player Is Visible or Not
    /// </summary>
    public bool IsPlayerVisible { get; private set; } = false;

    /// <summary>
    /// Is Going to Terminal Or Not
    /// </summary>
    private bool goingToTerminal = false;

    /// <summary>
    /// Is Alarm Activated or Not
    /// </summary>
    private bool alarmActivated = false;

    /// <summary>
    /// Is Enemy Keycard Holder
    /// </summary>
    public bool keycardHolder = false;

    /// <summary>
    /// Is Enemy Going to Exit
    /// </summary>
    private bool goingToExit = false;

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

        Transform temp;

        // Shuffle Patrol Points
        for (int i = 0; i < points.Length; i++)
        {
            int rnd = Random.Range(0, points.Length);
            temp = points[rnd];
            points[rnd] = points[i];
            points[i] = temp;
        }
    }

    /// <summary>
    /// Called Once per Frame
    /// </summary>
    void Update()
    {
        // Get Player Visibility
        IsPlayerVisible = PlayerVisible();

        // If Suspicion Is Alerted...
        if (suspicion == Suspicion.Alerted)
        {
            // If Enemy Is Scientist
            if (enemyType == EnemyType.Scientist)
            {
                // If Alarm Is Activated...
                if (!alarmActivated)
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
                }
                // Else Alarm Isn't Activated...
                else
                {
                    // If Keycard Holder...
                    if (keycardHolder)
                    {
                        // If Enemy Is Going to Exit...
                        if (goingToExit)
                        {
                            // If At Exit...
                            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                            {
                                // TODO: Losing Behaviour
                                Debug.Log("You Lost...");
                            }
                        }
                        // Else Enemy Isn't Going to Exit...
                        else
                        {
                            // Going to Exit
                            agent.destination = FindClosestExit().transform.position;
                            goingToExit = true;
                        }
                    }
                    // Else Not Keycard Holder...
                    else
                    {
                        // Stop Moving
                        agent.isStopped = true;
                    }
                }
            }
            // Else If Enemy Is Guard...
            else if (enemyType == EnemyType.Guard)
            {
                // If Player Is visible
                if (IsPlayerVisible)
                {
                    agent.destination = player.transform.position;
                    goingToTerminal = false;

                    if (goTerminalCountdown != null)
                    {
                        StopCoroutine(goTerminalCountdown);
                        goTerminalCountdown = null;
                    }
                }
                // Else Player Is Not Visible...
                else
                {
                    // If Current Going to Terminal....
                    if (goingToTerminal)
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
                    }
                    else
                    {
                        // If Not Going to Terminal And Alarm Isn't Activate...
                        if (goTerminalCountdown == null && !alarmActivated)
                        {
                            // Start Countdown to Go to Terminal
                            goTerminalCountdown = GoTerminalCountdown(5);
                            StartCoroutine(goTerminalCountdown);
                        }

                        // If At Destination...
                        if (!agent.pathPending && agent.remainingDistance < 0.5f)
                        {
                            // Patrol Next Point
                            PatrolNextPoint();
                        }
                    }
                }
            }
        }
        // If Suspicion Is Curious...
        else if (suspicion == Suspicion.Curious)
        {
            // If Player Is Visible...
            if (PlayerVisible())
            {
                if (IsCloseEnough(player.transform, 5))
                {
                    agent.isStopped = true;
                    LookAtPlayer();
                }
                else
                {
                    agent.isStopped = false;
                    agent.destination = player.transform.position;
                }

                if (IncrementCounter(ref detectedCounter, DetectedThreshold, Time.deltaTime))
                {
                    agent.isStopped = false;
                    suspicion = Suspicion.Alerted;
                    agent.speed = 5;

                    // If Enemy Is Scientist
                    if (enemyType == EnemyType.Scientist)
                    {
                        agent.destination = FindClosestTerminal().transform.position;
                        lastPlayerPosition = player.transform.position;
                    }
                }

                // Reset Curious Cooldown
                if (curiousCountdownCoroutine != null)
                {
                    StopCoroutine(curiousCountdownCoroutine);
                    curiousCountdownCoroutine = null;
                }

                // Stop Looking Last Player Location Check
                if (playerLastLocationCoroutine != null)
                {
                    StopCoroutine(playerLastLocationCoroutine);
                    playerLastLocationCoroutine = null;
                }
            }
            // If Player Isn't Visible...
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

                if (curiousCountdownCoroutine == null)
                {
                    curiousCountdownCoroutine = CuriousCountdown(5);
                    StartCoroutine(curiousCountdownCoroutine);
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
                LookAtPlayer();
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

    /// <summary>
    /// Coroutine to Resume Patrolling
    /// </summary>
    private IEnumerator reenableMoving;

    /// <summary>
    /// Coroutine to Resume Patrolling After Delay
    /// </summary>
    /// <param name="delay"> Delay in Seconds </param>
    IEnumerator ReenableMoving(float delay)
    {
        // Apply Delay
        yield return new WaitForSeconds(delay);

        // If Patrolling...
        if (suspicion == Suspicion.Patrol)
            // Resume Patrolling
            agent.updatePosition = true;

        yield return null;
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
            if (Physics.Raycast(ray, out hit, maxDistance, ~LayerMask.GetMask("Ignore Raycast")))
            {
                Debug.Log(hit.collider.name);

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

    /// <summary>
    /// Find Exit Closest to Enemy
    /// </summary>
    /// <returns> Closest Exit </returns>
    GameObject FindClosestExit()
    {
        // Find All Terminal GameObjects in Scene
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Exit");

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

        // Start and Speed Up
        agent.isStopped = false;
        agent.speed = 5;

        // Stop Going to Terminal Since Alarm Is On
        goingToTerminal = false;
        alarmActivated = true;

        // If Enemy Is A Guard...
        if (enemyType == EnemyType.Guard)
        {
            // Set Destination to Player
            agent.destination = position;
        }
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// Coroutine to Resume Patrolling
    /// </summary>
    private IEnumerator resumePatrolCoroutine;

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
    /// Coroutine to Go to Last Known Player Location
    /// </summary>
    private IEnumerator playerLastLocationCoroutine;

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
    /// Coroutine to Countdown to Being Not Curious
    /// </summary>
    private IEnumerator curiousCountdownCoroutine;

    /// <summary>
    /// Coroutine for Curiousity Cooldown After Delay
    /// </summary>
    /// <param name="delay"> Delay in Seconds </param>
    IEnumerator CuriousCountdown(float delay)
    {
        // Apply Delay
        yield return new WaitForSeconds(delay);

        // If Suspicion Curious...
        if (suspicion == Suspicion.Curious)
            // Switch Back to Patrolling
            suspicion = Suspicion.Patrol;

        yield return null;
    }

    /// <summary>
    /// Coroutine to Countdown to Go to Terminal
    /// </summary>
    private IEnumerator goTerminalCountdown;

    /// <summary>
    /// Couroutine to Go to Terminal After Delay
    /// </summary>
    /// <param name="delay"> Delay Before Going to Terminal </param>
    IEnumerator GoTerminalCountdown(float delay)
    {
        // Apply Delay
        yield return new WaitForSeconds(delay);

        // Go to Terminal
        agent.destination = FindClosestTerminal().transform.position;
        goingToTerminal = true;
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

    /// <summary>
    /// See If Enemy Is Close Enough to Player
    /// </summary>
    /// <param name="target"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    private bool IsCloseEnough(Transform target, float range)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance < range;
    }

    /// <summary>
    /// To Make Enemy Look At Player
    /// </summary>
    private void LookAtPlayer()
    {
        Quaternion rotation = Quaternion.LookRotation(
            player.transform.position - transform.position
        );

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 3);
    }

    #endregion
}
