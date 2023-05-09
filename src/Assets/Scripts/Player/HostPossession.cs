using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

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
    /// Player Rigidbody
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// Player Animator
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Player Movement Component
    /// </summary>
    private Movement movement;

    /// <summary>
    /// Able to Possess Flag
    /// </summary>
    public bool canPossess = false;

    /// <summary>
    /// Possession Progress Lerp Value
    /// </summary>
    public float lerp = 1;

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
    /// Currently Possessing Flag
    /// </summary>
    public bool isPossesing = false;

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
        animator = GetComponent<Animator>();

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

            movement.enabled = false;

            animator.SetTrigger("Possession");
        }

        // If Animation Is Not Done and Host Is Not Null...
        if (lerp < 1 && host != null)
        {
            // Lerp Position and Add Sin Wave Bounce
            Vector3 currentPosition = Vector3.Lerp(oldPosition, Vector3.zero, lerp);
            currentPosition.y += Mathf.Sin(lerp * Mathf.PI) * 1f;
            transform.localPosition = currentPosition;

            // Lerp Rotation to New Rotation
            Quaternion rotation = host.transform.localRotation * Quaternion.Euler(rotationVector);
            transform.localRotation = Quaternion.Lerp(oldRotation, rotation, lerp);

            GetComponent<BoxCollider>().isTrigger = true;

            // Progress Animation
            lerp += Time.deltaTime;
        }
        else if (lerp >= 1 && host != null && isPossesing)
        {
            EnemySuspicion suspicion = host.GetComponentInParent<EnemySuspicion>();
            suspicion.enabled = false;

            NavMeshAgent agent = host.GetComponentInParent<NavMeshAgent>();
            agent.enabled = false;

            Movement movement = host.GetComponentInParent<Movement>();
            movement.enabled = true;

            CapsuleCollider collider = host.GetComponentInParent<CapsuleCollider>();
            collider.enabled = true;

            Rigidbody hostRigidbody = host.GetComponentInParent<Rigidbody>();
            hostRigidbody.freezeRotation = true;
            hostRigidbody.isKinematic = false;
            hostRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Possession")
        {
            canPossess = true;
            host = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Possession")
        {
            canPossess = false;

            if (!isPossesing)
                host = null;
        }
    }
}
