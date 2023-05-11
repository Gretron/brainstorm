using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

/// <summary>
/// Enemy Animation Behaviour
/// </summary>
public class EnemyAnimation : MonoBehaviour
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
    /// NPC Navigation
    /// </summary>
    private NavMeshAgent agent;

    /// <summary>
    /// Player Reference
    /// </summary>
    private GameObject player;

    /// <summary>
    /// Look Rig Component
    /// </summary>
    private Rig lookRig;

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

        GameObject rigGameObject = gameObject.transform.Find("LookRig").gameObject;
        lookRig = rigGameObject.GetComponent<Rig>();
        MultiAimConstraint look = lookRig.transform
            .Find("HeadLook")
            .GetComponent<MultiAimConstraint>();
        var data = look.data.sourceObjects;
        data.Clear();
        data.Add(new WeightedTransform(player.transform, 1));
        look.data.sourceObjects = data;

        GetComponent<RigBuilder>().Build();
    }

    /// <summary>
    /// Called Once per Frame
    /// </summary>
    void Update()
    {
        // Get Velocity from Agent
        Vector3 velocity = agent.transform.InverseTransformDirection(agent.velocity);
        float speed = velocity.z;
        animator.SetFloat("Velocity", speed);

        Suspicion suspicionState = suspicion.suspicion;

        // If Player Is Visible...
        if (suspicion.IsPlayerVisible)
        {
            // Make Enemy Look At Player
            lookRig.weight += Time.deltaTime;

            // Turn Spotted Flag to True
            animator.SetBool("Spotted", true);

            // Get the direction from the enemy to the player
            Vector3 toPlayer = player.transform.position - transform.position;
            Vector3 enemyForward = transform.forward;
            float angle = Vector3.Angle(enemyForward, toPlayer);
            Vector3 cross = Vector3.Cross(enemyForward, toPlayer);

            // If Player Is Not Right In Front...
            if (angle > 15f)
            {
                if (cross.y > 0f)
                    animator.SetFloat("Turn", 1);
                else
                    animator.SetFloat("Turn", -1);
            }
            else
                animator.SetFloat("Turn", 0);
        }
        else
        {
            // Make Enemy Stop Looking At Player
            lookRig.weight -= Time.deltaTime;

            animator.SetFloat("Turn", 0);

            if (speed != 0)
            {
                animator.SetBool("Spotted", false);
            }
        }

        if (suspicionState == Suspicion.Patrol) { }
        else if (suspicionState == Suspicion.Curious) { }
        else if (suspicionState == Suspicion.Alerted) { }
    }
}
