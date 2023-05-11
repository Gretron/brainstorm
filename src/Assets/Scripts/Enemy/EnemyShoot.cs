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
    /// Aim Rig Component
    /// </summary>
    private Rig aimRig;

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
    public int Ammo
    {
        get { return ammo; }
    }

    /// <summary>
    /// Enemy Ammo Amount
    /// </summary>
    private int ammo = 5;

    [SerializeField]
    /// <summary>
    /// Enemy Shooting Speed
    /// </summary>
    private int shootSpeed = 5;

    /// <summary>
    /// Counter Until Next Shot
    /// </summary>
    private float shootCounter = 0;

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

        gunLayerIndex = animator.GetLayerIndex("Pistol Layer");

        GameObject rigGameObject = gameObject.transform.Find("AimRig").gameObject;
        aimRig = rigGameObject.GetComponent<Rig>();

        MultiAimConstraint bodyConstraint = aimRig.transform
            .Find("BodyAim")
            .GetComponent<MultiAimConstraint>();
        var bodyData = bodyConstraint.data.sourceObjects;
        bodyData.Clear();
        bodyData.Add(new WeightedTransform(player.transform, 1));
        bodyConstraint.data.sourceObjects = bodyData;

        bodyWalkConstraint = aimRig.transform
            .Find("BodyWalkAim")
            .GetComponent<MultiAimConstraint>();
        var bodyWalkData = bodyWalkConstraint.data.sourceObjects;
        bodyWalkData.Clear();
        bodyWalkData.Add(new WeightedTransform(player.transform, 1));
        bodyWalkConstraint.data.sourceObjects = bodyWalkData;

        MultiAimConstraint handConstraint = aimRig.transform
            .Find("HandAim")
            .GetComponent<MultiAimConstraint>();
        var handData = handConstraint.data.sourceObjects;
        handData.Clear();
        handData.Add(new WeightedTransform(player.transform, 1));
        handConstraint.data.sourceObjects = handData;

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

                if (speed > 2)
                {
                    bodyWalkConstraint.weight += Time.deltaTime * 3;
                }
                else
                {
                    bodyWalkConstraint.weight -= Time.deltaTime * 3;
                }

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
