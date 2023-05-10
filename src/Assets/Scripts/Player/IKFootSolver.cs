using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
    [SerializeField]
    Transform body;

    [SerializeField]
    Rigidbody bodyRb;

    Vector3 nextFootPosition;

    Vector3 oldFootPosition;
    Vector3 currentFootPosition;
    Vector3 newFootPosition;

    Vector3 oldFootNormal;
    Vector3 currentFootNormal;
    Vector3 newFootNormal;

    public float angle = 45f;
    public float magnitude = 1.5f;

    public float nextFootPositionOffset = 1.5f;
    public float moveDistance = 1.5f;

    public float speed = 10f;

    public Vector3 footOffset;

    public float stepHeight = 0.5f;

    public bool printDeg;
    public bool checkSign = true;

    private float degreesToFoot;

    private float lerp = 1;

    public IKFootSolver otherFoot;

    public HostPossession possession;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        body = player.transform;
        bodyRb = player.GetComponent<Rigidbody>();
        possession = player.GetComponent<HostPossession>();

        // Find Angle Between Body Forward and Foot
        Vector3 bodyToFoot = Vector3.Normalize(transform.position - body.position);
        float dot = Vector3.Dot(body.forward, bodyToFoot);
        float deg = Mathf.Acos(dot) * Mathf.Rad2Deg;

        // Find Out If Leg Is On Right Side or Left Side
        float rightDot = Vector3.Dot(body.right, bodyToFoot);
        float sign = checkSign ? (rightDot > 0 ? 1 : -1) : 1;
        degreesToFoot = deg * sign;

        currentFootPosition = newFootPosition = oldFootPosition = transform.position;
    }

    Vector3 initialPos;

    private void OnHostPossession() { }

    // Update is called once per frame
    void Update()
    {
        transform.position = currentFootPosition;

        Vector3 forward = body.forward;
        Quaternion rotation = Quaternion.Euler(0, degreesToFoot, 0f);
        Vector3 footPosition =
            (rotation * forward) * nextFootPositionOffset + (body.rotation * footOffset);

        bool otherFootMoving = otherFoot != null ? otherFoot.Moving() : false;

        Ray ray = new Ray(footPosition + body.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 10, LayerMask.GetMask("Environment")))
        {
            // When Distance Between Positions Is Greate Than Step Distance And Previous Step Completed
            if (
                Vector3.Distance(newFootPosition, hit.point) > moveDistance
                && !otherFootMoving
                && lerp >= 1
            )
            {
                lerp = 0;
                oldFootPosition = newFootPosition;
                newFootPosition = hit.point;

                if (printDeg)
                    print("called");
            }

            Debug.DrawLine(oldFootPosition, transform.position);
        }

        if (lerp < 1)
        {
            Vector3 tempPosition = Vector3.Lerp(oldFootPosition, newFootPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight; // TODO Replace with Step Height

            currentFootPosition = tempPosition;

            float magnitude = (bodyRb.velocity.magnitude + 1) / 2;

            lerp += Time.deltaTime * speed * magnitude; // TODO Replace with Speed
        }
        else
        {
            oldFootPosition = newFootPosition;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.up * nextFootPositionOffset, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * moveDistance);
    }

    public bool Moving()
    {
        return lerp < 1;
    }
}
