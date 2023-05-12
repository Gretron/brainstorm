using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
    public bool isPossesing = false;

    /// <summary>
    /// Currently Possessed Flag
    /// </summary>
    public bool isPossessed = false;

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
        // If Able to Possess and Presses Possess Key...
        if (canPossess && Input.GetKeyDown(KeyCode.F))
        {
            hostPossession.Invoke();

            lerp = 0;
            newPosition = host.transform.position;
            isPossesing = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rb.isKinematic = true;
            transform.parent = host.transform;
            oldPosition = transform.localPosition;
            oldRotation = transform.localRotation;
            boxCollider.isTrigger = true;

            movement.enabled = false;
        }

        if (lerp > 0 && host == null)
        {
            lerp -= Time.deltaTime;

            // Progressively Latch Off
            latchRig.weight = lerp;
            walkingRig.weight = 1 - lerp;
        }

        // If Animation Is Not Done and Host Is Not Null...
        if (lerp < 1 && host != null && isPossesing)
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
        else if (lerp >= 1 && host != null && isPossesing && !isPossessed)
        {
            followPlayer.SetVerticalAngles(CameraVerticalAngles.Human);

            EnemySuspicion suspicion = host.GetComponentInParent<EnemySuspicion>();
            suspicion.enabled = false;

            enemy = suspicion.gameObject;

            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            agent.enabled = false;

            Movement movement = enemy.GetComponent<Movement>();
            movement.enabled = true;

            CapsuleCollider collider = enemy.GetComponent<CapsuleCollider>();
            collider.enabled = true;

            Rigidbody hostRigidbody = enemy.GetComponent<Rigidbody>();
            hostRigidbody.freezeRotation = true;
            hostRigidbody.isKinematic = false;
            hostRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            hostHealth = enemy.GetComponent<EnemyHealth>();
            EnemyShoot shoot = enemy.GetComponent<EnemyShoot>();
            GameManager.Instance.hostHealth = hostHealth;
            GameManager.Instance.hostShoot = shoot;

            isPossesing = false;
            isPossessed = true;
        }
        else if (isPossessed)
        {
            hostHealth.TakeDamage(Time.deltaTime * 5);
        }
    }

    /// <summary>
    /// Unpossess Player
    /// </summary>
    public void Unpossess()
    {
        enemy.GetComponent<Movement>().enabled = false;

        var enemyRigidbody = enemy.GetComponent<Rigidbody>();
        enemyRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        enemyRigidbody.isKinematic = true;

        host = null;
        enemy = null;
        //lerp = 0;
        // newPosition = host.transform.position;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        transform.parent = null;
        boxCollider.isTrigger = false;
        // oldPosition = transform.localPosition;
        // oldRotation = transform.localRotation;

        movement.enabled = true;

        isPossesing = false;
        isPossessed = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Possession" && !isPossesing)
        {
            canPossess = true;
            host = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Possession" && !isPossesing)
        {
            canPossess = false;

            if (!isPossesing)
                host = null;
        }
    }
}
