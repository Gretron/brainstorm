using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera Vertical Angles
/// </summary>
public enum CameraVerticalAngles
{
    Alien = 0,
    Human = 1,
}

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

    /// <summary>
    /// Camera Component
    /// </summary>
    private Camera camera;

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
    private float maxVerticalAngle = 45;

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
    private LayerMask collisionLayer;

    /// <summary>
    /// Maximum Zoom Possible
    /// </summary>
    private float zoomMax = 40;

    /// <summary>
    /// Minimum Zoom Possible
    /// </summary>
    private float zoomMin = 60;

    /// <summary>
    /// Zoom Speed Interpolation
    /// </summary>
    private float zoomSpeed = 80;

    /// <summary>
    /// Aim Offset Vector
    /// </summary>
    public Vector3 AimOffset { get; private set; }

    /// <summary>
    /// Aim Offset Speed
    /// </summary>
    private float aimOffsetSpeed = 2;

    /// <summary>
    /// Aim Offset Lerp Amount
    /// </summary>
    private float aimOffsetLerp = 0;

    /// <summary>
    /// Current Vertical Angles
    /// </summary>
    public CameraVerticalAngles VerticalAngles { get; private set; }

    private PauseMenu pauseMenu;
    private bool isPaused = false;

    /// <summary>
    /// Called Before First Frame Update
    /// </summary>
    void Start()
    {
        pauseMenu = FindObjectOfType<PauseMenu>();

        // Get References
        player = GameObject.FindGameObjectWithTag("Player");
        camera = GetComponent<Camera>();

        // Calculate Inital Offset
        cameraOffset = offset;
        AimOffset = Vector3.right;

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
        if (isPaused)
        {
            return; // Don't update camera position or rotation
        }

        Vector3 playerToCamera = (transform.position - player.transform.position).normalized;
        Vector3 playerToCameraWithoutY = transform.position - player.transform.position;
        playerToCameraWithoutY.y = 0;
        playerToCameraWithoutY = playerToCameraWithoutY.normalized;

        /*
        Debug.DrawLine(player.transform.position, playerToCamera + player.transform.position);
        Debug.DrawLine(
            player.transform.position,
            playerToCameraWithoutY + player.transform.position,
            Color.red
        );
        */

        float sign = playerToCamera.y >= 0 ? 1 : -1;
        float angle = Vector3.Angle(playerToCamera, playerToCameraWithoutY) * sign;

        // Update Camera Position Based on Player Position Offset, with Obstacle Detection
        Ray ray = new Ray(player.transform.position, cameraOffset);
        RaycastHit hit;

        // If Raycast Hits Something...
        if (Physics.Raycast(ray, out hit, maxDistance, LayerMask.GetMask("Environment")))
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

        Quaternion angleRotation = Quaternion.Euler(
            0,
            transform.eulerAngles.y,
            transform.eulerAngles.z
        );

        Vector3 right = angleRotation * Vector3.right;

        // If Current Vertical Angle Is Within Angle Range...
        if (angle >= minVerticalAngle && angle <= maxVerticalAngle)
        {
            float value = -mouseY;
            float newAngle = angle + -mouseY;

            if (newAngle > maxVerticalAngle)
            {
                value = -(newAngle - maxVerticalAngle);
            }
            else if (newAngle < minVerticalAngle)
            {
                value = minVerticalAngle - newAngle;
            }

            cameraOffset = Quaternion.AngleAxis(value, right) * cameraOffset; // Adds Mouse Input Offset to Current Camera Offset
        }
        else if (angle < minVerticalAngle)
        {
            float offset = minVerticalAngle - angle;
            cameraOffset = Quaternion.AngleAxis(offset, right) * cameraOffset;
        }
        else if (angle > maxVerticalAngle)
        {
            float offset = angle - maxVerticalAngle;
            cameraOffset = Quaternion.AngleAxis(-offset, right) * cameraOffset;
        }

        // Calculate Rotation to Point Camera at Player
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);

        // Update Camera Rotation to Point at Player
        transform.rotation = lookRotation;

        transform.position += (transform.rotation * AimOffset * aimOffsetLerp);
    }

    /// <summary>
    /// Zoom Camera In
    /// </summary>
    public void ZoomIn()
    {
        // If Zoom Is Over Threshold...
        if (camera.fieldOfView <= zoomMax)
            return;

        camera.fieldOfView = Mathf.Min(zoomMin, camera.fieldOfView - Time.deltaTime * zoomSpeed);
    }

    /// <summary>
    /// Zoom Camera Out
    /// </summary>
    public void ZoomOut()
    {
        // If Zoom Is Under Threshold...
        if (camera.fieldOfView >= zoomMin)
            return;

        camera.fieldOfView = Mathf.Max(zoomMax, camera.fieldOfView + Time.deltaTime * zoomSpeed);
    }

    /// <summary>
    /// Increase Aim Offset Influence
    /// </summary>
    public void IncreaseAimOffset() =>
        aimOffsetLerp = Mathf.Min(aimOffsetLerp + Time.deltaTime * aimOffsetSpeed, 1);

    /// <summary>
    /// Decrease Aim Offset Influence
    /// </summary>
    public void DecreaseAimOffset() =>
        aimOffsetLerp = Mathf.Max(aimOffsetLerp - Time.deltaTime * aimOffsetSpeed, 0);

    /// <summary>
    /// Change Between Camera Vertical Angles
    /// </summary>
    /// <param name="verticalAngles"> Camera Vertical Angles </param>
    public void SetVerticalAngles(CameraVerticalAngles verticalAngles)
    {
        VerticalAngles = verticalAngles;

        if (verticalAngles == CameraVerticalAngles.Alien)
        {
            minVerticalAngle = 0;
            maxVerticalAngle = 45;
        }
        else if (verticalAngles == CameraVerticalAngles.Human)
        {
            minVerticalAngle = -30;
            maxVerticalAngle = 45;
        }
    }

    public void SetPaused(bool value)
    {
        isPaused = value;
    }
}
