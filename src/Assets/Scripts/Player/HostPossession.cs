using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Animations.Rigging;

/// <summary>
/// Host Possession Behaviour
/// </summary>
public class HostPossession : MonoBehaviour
{
    /// <summary>
    /// Host Possession Event
    /// </summary>
    public UnityEvent hostPossession;

    /// <summary>
    /// Host to be Possessed
    /// </summary>
    public GameObject host;

    /// <summary>
    /// Host to be Possessed
    /// </summary>
    public GameObject enemy;

    /// <summary>
    /// Player Rigidbody
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// Player Box Collider
    /// </summary>
    private BoxCollider boxCollider;

    /// <summary>
    /// Player Movement Component
    /// </summary>
    private Movement movement;

    /// <summary>
    /// Follow Player Camera Component
    /// </summary>
    private FollowPlayer followPlayer;

    /// <summary>
    /// Enemy Host Health
    /// </summary>
    private EnemyHealth hostHealth;

    /// <summary>
    /// Able to Possess Flag
    /// </summary>
    public bool canPossess = false;

    /// <summary>
    /// Possession Progress Lerp Value
    /// </summary>
    public float lerp = 0;

    /// <summary>
    /// Player Old Position
    /// </summary>
    public Vector3 oldPosition;

    /// <summary>
    /// Player New Host Position
    /// </summary>
    public Vector3 newPosition;

    /// <summary>
    /// Player Old Rotation
    /// </summary>
    public Quaternion oldRotation;

    /// <summary>
    /// Kill Indicator Image
    /// </summary>
    private Image KillIndicator;

    /// <summary>
    /// Player New Host Rotation
    /// </summary>
    public Quaternion newRotation;

    /// <summary>
    /// Player Rotation Process Vector
    /// </summary>
    private Vector3 rotationVector;

    /// <summary>
    /// Latch Rig Component
    /// </summary>
    private Rig latchRig;

    /// <summary>
    /// Walking Rig Component
    /// </summary>
    private Rig walkingRig;

    /// <summary>
    /// Currently Possessing Flag
    /// </summary>
    public bool isPossessing = false;

    /// <summary>
    /// Currently Possessed Flag
    /// </summary>
    public bool isPossessed = false;

    [SerializeField]
    /// <summary>
    /// Can Assassinate Flag
    /// </summary>
    private bool canAssassinate = true;

    /// <summary>
    /// Possession Indicator
    /// </summary>
    private Image possessionIndicator;

    /// <summary>
    /// Enemy Suspicion Behaviour
    /// </summary>
    private EnemySuspicion suspicion;

    /// <summary>
    /// Main Exit Reference
    /// </summary>
    private GameObject exit;

    /// <summary>
    /// Called Before First Frame Update
    /// </summary>
    void Start()
    {
        // Create Event If Null
        if (hostPossession == null)
            hostPossession = new UnityEvent();

        // Get References
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        boxCollider = GetComponent<BoxCollider>();
        followPlayer = Camera.main.gameObject.GetComponent<FollowPlayer>();

        exit = GameObject.FindGameObjectWithTag("Exit");

        GameObject latchGameObject = gameObject.transform.Find("LatchRig").gameObject;
        latchRig = latchGameObject.GetComponent<Rig>();

        GameObject walkingGameObject = gameObject.transform.Find("WalkingRig").gameObject;
        walkingRig = walkingGameObject.GetComponent<Rig>();

        // Set Rotation Vector Value
        rotationVector = new Vector3(-90, 0, 0);
    }

    /// <summary>
    /// Called Once per Frame
    /// </summary>
    void Update()
    {
        if (possessionIndicator != null && canPossess && !isPossessing && !isPossessed)
        {
            possessionIndicator.enabled = true;
        }

        // If Able to Possess and Presses Possess Key...
        if (canPossess && Input.GetKeyDown(KeyCode.F) && !isPossessed)
        {
            possessionIndicator.enabled = false;

            hostPossession.Invoke();

            lerp = 0;
            newPosition = host.transform.position;
            isPossessing = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rb.isKinematic = true;
            transform.parent = host.transform;
            oldPosition = transform.localPosition;
            oldRotation = transform.localRotation;
            boxCollider.isTrigger = true;

            movement.enabled = false;

            if (GameManager.Instance.BrainPower >= 20)
            {
                GameManager.Instance.UseBrainPower(-20);
            }
        }

        if (lerp > 0 && !isPossessing && !isPossessed)
        {
            lerp -= Time.deltaTime;

            // Progressively Latch Off
            latchRig.weight = lerp;
            walkingRig.weight = 1 - lerp;
        }

        // If Animation Is Not Done and Host Is Not Null...
        if (lerp < 1 && host != null && isPossessing)
        {
            // Lerp Position and Add Sin Wave Bounce
            Vector3 currentPosition = Vector3.Lerp(oldPosition, Vector3.zero, lerp);
            currentPosition.y += Mathf.Sin(lerp * Mathf.PI) * 1f;
            transform.localPosition = currentPosition;

            // Lerp Rotation to New Rotation
            Quaternion rotation = host.transform.localRotation * Quaternion.Euler(rotationVector);
            transform.localRotation = Quaternion.Lerp(oldRotation, rotation, lerp);

            // Progress Animation
            lerp += Time.deltaTime;

            // Progressively Latch On
            latchRig.weight = lerp;
            walkingRig.weight = 1 - lerp;
        }
        else if (lerp >= 1 && host != null && isPossessing && !isPossessed)
        {
            followPlayer.SetVerticalAngles(CameraVerticalAngles.Human);

            EnemySuspicion enemySuspicion = host.GetComponentInParent<EnemySuspicion>();
            suspicion = enemySuspicion;
            enemySuspicion.enabled = false;

            var assassination = host.GetComponentInParent<EnemyAssassination>();

            if (assassination)
                assassination.enabled = true;

            enemy = enemySuspicion.gameObject;
            enemy.GetComponentInChildren<SuspicionMeter>().gameObject.SetActive(false);

            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            agent.enabled = false;

            Movement movement = enemy.GetComponent<Movement>();
            movement.enabled = true;

            CapsuleCollider collider = enemy.GetComponent<CapsuleCollider>();
            collider.enabled = true;

            KillIndicator = enemy.transform.Find("Canvas/KillIndicator").GetComponent<Image>();

            Rigidbody hostRigidbody = enemy.GetComponent<Rigidbody>();
            hostRigidbody.freezeRotation = true;
            hostRigidbody.isKinematic = false;
            hostRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            hostHealth = enemy.GetComponent<EnemyHealth>();
            EnemyShoot shoot = enemy.GetComponent<EnemyShoot>();
            EnemyAssassination enemyAssassination = enemy.GetComponent<EnemyAssassination>();
            GameManager.Instance.hostAssassination = enemyAssassination;
            GameManager.Instance.hostSuspicion = enemySuspicion;
            GameManager.Instance.hostHealth = hostHealth;
            GameManager.Instance.hostShoot = shoot;

            isPossessing = false;
            isPossessed = true;
        }
        else if (isPossessed)
        {
            if (suspicion.keycardHolder)
            {
                var hostToExit = enemy.transform.position - exit.transform.position;

                if (hostToExit.sqrMagnitude < 3)
                {
                    GameManager.Instance.LoadNextLevel();
                }
            }

            hostHealth.TakeDamage(Time.deltaTime * 2);

            if (GameManager.Instance.BrainPower > 30 && canAssassinate)
            {
                KillIndicator.enabled = true;
            }
            else
            {
                KillIndicator.enabled = false;
            }

            if (Input.GetKeyDown(KeyCode.F) && canAssassinate)
            {
                KillIndicator.enabled = false;

                GameManager.Instance.TakePlayerDamage(20);

                hostHealth.TakeDamage(100);
                Unpossess();

                followPlayer.SetVerticalAngles(CameraVerticalAngles.Alien);

                if (GameManager.Instance.BrainPower >= 30)
                {
                    GameManager.Instance.UseBrainPower(-30);
                }
            }
        }
    }

    /// <summary>
    /// Unpossess Player
    /// </summary>
    public void Unpossess()
    {
        enemy.GetComponent<Movement>().enabled = false;
        var assassination = host.GetComponentInParent<EnemyAssassination>();

        if (assassination)
            assassination.enabled = false;

        var enemyRigidbody = enemy.GetComponent<Rigidbody>();
        enemyRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        enemyRigidbody.isKinematic = true;

        host = null;
        enemy = null;

        lerp = 1;
        // newPosition = host.transform.position;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        transform.parent = null;
        boxCollider.isTrigger = false;
        // oldPosition = transform.localPosition;
        // oldRotation = transform.localRotation;

        movement.enabled = true;

        isPossessing = false;
        isPossessed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Possession" && !isPossessing)
        {
            var enemyHost = other.GetComponentInParent<EnemySuspicion>().gameObject;
            possessionIndicator = enemyHost.transform
                .Find("Canvas/PossessionIndicator")
                .GetComponent<Image>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Possession" && !isPossessing)
        {
            canPossess = true;
            host = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Possession" && !isPossessing)
        {
            canPossess = false;

            if (!isPossessing)
                host = null;

            possessionIndicator = null;
        }
    }
}
