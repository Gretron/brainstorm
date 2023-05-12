using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class EnemyShoot : MonoBehaviour
{
    /// <summary>
    /// Enemy Animator
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Enemy Player Movement
    /// </summary>
    private Movement movement;

    /// <summary>
    /// Enemy Suspicion Behaviour
    /// </summary>
    private EnemySuspicion suspicion;

    /// <summary>
    /// Player Reference
    /// </summary>
    private GameObject player;

    [SerializeField]
    /// <summary>
    /// Bullet Reference
    /// </summary>
    private GameObject bullet;

    [SerializeField]
    /// <summary>
    /// Enemy Gun Tip Transform
    /// </summary>
    private Transform gunTip;

    /// <summary>
    /// Camera Follow Player Component
    /// </summary>
    private FollowPlayer followPlayer;

    /// <summary>
    /// Aim Rig Component
    /// </summary>
    private Rig aimRig;

    /// <summary>
    /// Aim Rig Component
    /// </summary>
    private Rig playerAimRig;

    /// <summary>
    /// Body Walk Constraint Component
    /// </summary>
    private MultiAimConstraint bodyWalkConstraint;

    /// <summary>
    /// Index of Gun Animator Layer
    /// </summary>
    private int gunLayerIndex;

    /// <summary>
    /// NPC Navigation
    /// </summary>
    private NavMeshAgent agent;

    /// <summary>
    /// Enemy Ammo Amount
    /// </summary>
    public int MaxAmmo
    {
        get { return maxAmmo; }
    }

    /// <summary>
    /// Enemy Ammo Amount
    /// </summary>
    private int maxAmmo = 5;

    /// <summary>
    ///  Current Ammo Amount
    /// </summary>
    public int Ammo { get; private set; } = 5;

    [SerializeField]
    /// <summary>
    /// Enemy Shooting Speed
    /// </summary>
    private int shootSpeed = 5;

    /// <summary>
    /// Counter Until Next Shot
    /// </summary>
    private float shootCounter = 0;

    [SerializeField]
    /// <summary>
    /// Target to Aim At When Controlled By Player
    /// </summary>
    private Transform playerAimTarget;

    /// <summary>
    /// Called Before First Frame Update
    /// </summary>
    void Start()
    {
        // Get References
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        suspicion = GetComponent<EnemySuspicion>();
        agent = GetComponent<NavMeshAgent>();
        movement = GetComponent<Movement>();
        followPlayer = Camera.main.gameObject.GetComponent<FollowPlayer>();

        gunLayerIndex = animator.GetLayerIndex("Pistol Layer");

        GameObject rigGameObject = gameObject.transform.Find("AimRig").gameObject;
        aimRig = rigGameObject.GetComponent<Rig>();

        MultiAimConstraint bodyConstraint = aimRig.transform
            .Find("BodyAim")
            .GetComponent<MultiAimConstraint>();

        if (bodyConstraint)
        {
            var bodyData = bodyConstraint.data.sourceObjects;
            bodyData.Clear();
            bodyData.Add(new WeightedTransform(player.transform, 1));
            bodyConstraint.data.sourceObjects = bodyData;
        }

        MultiAimConstraint handConstraint = aimRig.transform
            .Find("HandAim")
            .GetComponent<MultiAimConstraint>();

        if (handConstraint)
        {
            var handData = handConstraint.data.sourceObjects;
            handData.Clear();
            handData.Add(new WeightedTransform(player.transform, 1));
            handConstraint.data.sourceObjects = handData;
        }

        GameObject playerRigGameObject = gameObject.transform.Find("PlayerAimRig").gameObject;
        playerAimRig = playerRigGameObject.GetComponent<Rig>();

        MultiAimConstraint playerBodyConstraint = playerAimRig.transform
            .Find("BodyAim")
            .GetComponent<MultiAimConstraint>();

        if (playerBodyConstraint)
        {
            var playerBodyData = playerBodyConstraint.data.sourceObjects;
            playerBodyData.Clear();
            playerBodyData.Add(new WeightedTransform(playerAimTarget, 1));
            playerBodyConstraint.data.sourceObjects = playerBodyData;
        }

        MultiAimConstraint playerHandConstraint = playerAimRig.transform
            .Find("HandAim")
            .GetComponent<MultiAimConstraint>();

        if (playerHandConstraint)
        {
            var playerHandData = playerHandConstraint.data.sourceObjects;
            playerHandData.Clear();
            playerHandData.Add(new WeightedTransform(playerAimTarget, 1));
            playerHandConstraint.data.sourceObjects = playerHandData;
        }

        GetComponent<RigBuilder>().Build();
    }

    /// <summary>
    /// Called Once per Frame
    /// </summary>
    void Update()
    {
        if (movement.enabled)
        {
            if (animator.GetLayerWeight(gunLayerIndex) < 1)
            {
                animator.SetLayerWeight(
                    gunLayerIndex,
                    Mathf.Min(animator.GetLayerWeight(gunLayerIndex) + (Time.deltaTime * 5), 1)
                );
            }

            aimRig.weight -= Time.deltaTime;

            if (Input.GetMouseButton(1))
            {
                playerAimRig.weight += Time.deltaTime;

                movement.turnOnMove = false;

                followPlayer.ZoomIn();
                followPlayer.IncreaseAimOffset();

                Vector3 directionToCamera =
                    (transform.position + Camera.main.transform.rotation * followPlayer.AimOffset)
                    - Camera.main.transform.position;
                directionToCamera.y = 0;
                directionToCamera = directionToCamera.normalized;
                Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

                // Slowly Move Player Rotation to Camera Rotation
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    targetRotation,
                    5f * Time.deltaTime
                );

                // Find Spot Where Enemy Should Be Aiming
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

                RaycastHit hit;

                // If Raycast Hits Something...
                if (Physics.Raycast(ray, out hit, 15f, LayerMask.GetMask("Environment")))
                {
                    playerAimTarget.position = hit.point;
                }
                else
                {
                    playerAimTarget.position = ray.GetPoint(15f);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (Ammo > 0)
                    {
                        var bulletInstance = GameObject.Instantiate(
                            bullet,
                            gunTip.position,
                            gunTip.rotation
                        );

                        Ammo = Mathf.Max(Ammo - 1, 0);
                    }
                }
            }
            else
            {
                playerAimRig.weight -= Time.deltaTime;

                movement.turnOnMove = true;

                followPlayer.ZoomOut();
                followPlayer.DecreaseAimOffset();
            }
        }
        else
        {
            Suspicion suspicionState = suspicion.suspicion;

            if (suspicionState == Suspicion.Patrol)
            {
                animator.SetLayerWeight(
                    gunLayerIndex,
                    Mathf.Max(animator.GetLayerWeight(gunLayerIndex) - (Time.deltaTime * 5), 0)
                );

                aimRig.weight -= Time.deltaTime;
            }
            else if (suspicionState == Suspicion.Curious || suspicionState == Suspicion.Alerted)
            {
                // Get Velocity from Agent
                Vector3 velocity = agent.transform.InverseTransformDirection(agent.velocity);
                float speed = velocity.z;

                if (suspicion.IsPlayerVisible)
                {
                    aimRig.weight += Time.deltaTime;

                    if (suspicionState == Suspicion.Alerted)
                    {
                        if (shootCounter >= shootSpeed)
                        {
                            shootCounter = 0;

                            var bulletInstance = GameObject.Instantiate(
                                bullet,
                                gunTip.position,
                                gunTip.rotation
                            );
                        }

                        if (aimRig.weight == 1)
                        {
                            shootCounter += Time.deltaTime;
                        }
                    }
                }
                else
                {
                    aimRig.weight -= Time.deltaTime;
                }

                if (suspicionState == Suspicion.Curious)
                {
                    animator.SetLayerWeight(
                        gunLayerIndex,
                        Mathf.Min(animator.GetLayerWeight(gunLayerIndex) + (Time.deltaTime * 5), 1)
                    );
                }
            }
        }
    }
}
