using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera Follow Player Behaviour
/// </summary>
public class FollowPlayer : MonoBehaviour
{
    [SerializeField]
    /// <summary>
    /// Player GameObject
    /// </summary>
    private GameObject player;

    [SerializeField]
    /// <summary>
    ///  Offset from Player to Camera
    /// </summary>
    private Vector3 offset;

    [SerializeField]
    /// <summary>
    /// Speed of Camera Lerp When Colliding with Environment
    /// </summary>
    private float smoothSpeed = 15f;

    /// <summary>
    /// Sensitivity of Mouse
    /// </summary>
    public float mouseSensitivity = 2f;

    /// <summary>
    /// Minimum Vertical Angle
    /// </summary>
    private float minVerticalAngle = 0;

    /// <summary>
    /// Maximum Vertical Angle
    /// </summary>
    private float maxVerticalAngle = 60f;

    /// <summary>
    /// Current Vertical Angle
    /// </summary>
    private float currentVerticalAngle = 0f;

    /// <summary>
    /// Computed Camera Offset
    /// </summary>
    private Vector3 cameraOffset;

    /// <summary>
    /// Max Distance of Collision Detection
    /// </summary>
    private float maxDistance;

    [SerializeField]
    /// <summary>
    /// Layer Which Constitutes Collisions
    /// </summary>
    private LayerMask collionLayer;

    /// <summary>
    /// Called Before First Frame Update
    /// </summary>
    void Start()
    {
        // Get References
        player = GameObject.FindGameObjectWithTag("Player");

        // Calculate Inital Offset
        cameraOffset = player.transform.position + offset;

        // Hide Cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Calculate Max Distance
        maxDistance = Vector3.Magnitude(offset);
    }

    /// <summary>
    /// Called Once per Frame
    /// </summary>
    private void LateUpdate()
    {
        // Update Camera Position Based on Player Position Offset, with Obstacle Detection
        Ray ray = new Ray(player.transform.position, cameraOffset);
        RaycastHit hit;

        // If Raycast Hits Something...
        if (Physics.Raycast(ray, out hit, maxDistance, collionLayer))
        {
            transform.position = hit.point + hit.normal * 0.6f;
        }
        // Else Raycast Doesn't Hit Something...
        else
        {
            transform.position = player.transform.position + cameraOffset;
        }

        // Calculate Direction from Camera to Player
        Vector3 direction = player.transform.position - transform.position;

        // Apply Horizontal Offset Based on Mouse Input
        float mouseX = Input.GetAxis("Mouse X");
        cameraOffset = Quaternion.AngleAxis(mouseX, Vector3.up) * cameraOffset; // Adds Mouse Input Offset to Current Camera Offset

        // Apply Vertical Offset Based on Mouse Input
        float mouseY = Input.GetAxis("Mouse Y");

        currentVerticalAngle = Mathf.Clamp(
            currentVerticalAngle + mouseY,
            minVerticalAngle,
            maxVerticalAngle
        );

        // If Current Vertical Angle Is Within Angle Range...
        if (currentVerticalAngle > minVerticalAngle && currentVerticalAngle < maxVerticalAngle)
        {
            Quaternion angleRotation = Quaternion.Euler(
                0,
                transform.eulerAngles.y,
                transform.eulerAngles.z
            );

            Vector3 right = angleRotation * Vector3.right;

            cameraOffset = Quaternion.AngleAxis(mouseY, right) * cameraOffset; // Adds Mouse Input Offset to Current Camera Offset
        }

        // Calculate Rotation to Point Camera at Player
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);

        // Update Camera Rotation to Point at Player
        transform.rotation = lookRotation;
    }
}
