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
    /// Enemy Suspicion Behaviour
    /// </summary>
    private EnemySuspicion suspicion;

    /// <summary>
    /// Player Reference
    /// </summary>
    private GameObject player;

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
    /// Called Before First Frame Update
    /// </summary>
    void Start()
    {
        // Get References
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        suspicion = GetComponent<EnemySuspicion>();
        agent = GetComponent<NavMeshAgent>();

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

            animator.SetLayerWeight(
                gunLayerIndex,
                Mathf.Min(animator.GetLayerWeight(gunLayerIndex) + (Time.deltaTime * 5), 1)
            );

            if (suspicion.IsPlayerVisible)
            {
                aimRig.weight += Time.deltaTime;
            }
            else
            {
                aimRig.weight -= Time.deltaTime;
            }
        }
        else if (suspicionState == Suspicion.Alerted) { }
    }
}
