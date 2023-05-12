using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movement Behaviour
/// </summary>
public class Movement : MonoBehaviour
{
    [Header("References")]
    /// <summary>
    /// Transform of Camera
    /// </summary>
    private Transform cameraTransform;

    /// <summary>
    /// Rigidbody of Player
    /// </summary>
    private Rigidbody rb;

    [Header("Parameters")]
    [SerializeField]
    /// <summary>
    /// Turn Lerp Speed
    /// </summary>
    private float turnSmoothTime = 5f;

    [SerializeField]
    /// <summary>
    /// Speed of Movement
    /// </summary>
    private float speed = 20f;

    [SerializeField]
    /// <summary>
    /// Drag When on Ground
    /// </summary>
    private float groundDrag = 5f;

    /// <summary>
    /// Flag to Make Entity Turn On Movement
    /// </summary>
    public bool turnOnMove = true;

    /// <summary>
    /// Called Before First Frame Update
    /// </summary>
    private void Start()
    {
        // Get Camera Transform
        cameraTransform = Camera.main.gameObject.transform;

        // Get Rigidbody and Freeze All Rotations
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    /// <summary>
    /// Called Multiple Times per Frame
    /// </summary>
    private void FixedUpdate()
    {
        // Get Input Axes
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Make Direction Vector From Axes
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // If Length of Vector Indicates We're Moving...
        if (direction.magnitude >= 0.1f && turnOnMove)
        {
            // TODO: Calculate the target rotation based on the camera's forward direction
            float targetAngle =
                Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg
                + cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);

            // Slowly Move Player Rotation to Camera Rotation
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                turnSmoothTime * Time.deltaTime
            );
        }

        // Move Player in Direction of the Camera
        Vector3 moveDirection = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f) * direction;

        // Add Force to Rigidbody to Move
        rb.AddForce(
            moveDirection.normalized * speed * Time.fixedDeltaTime,
            ForceMode.VelocityChange
        );
    }

    /// <summary>
    /// Called When Collision Is Maintained
    /// </summary>
    /// <param name="collision"> Object of Collision </param>
    private void OnCollisionStay(Collision collision)
    {
        // Check if the player is colliding with the floor
        if (collision.gameObject.CompareTag("Floor"))
        {
            if (rb)
            {
                // Apply ground drag to the player
                rb.drag = groundDrag;
            }
        }
    }
}
