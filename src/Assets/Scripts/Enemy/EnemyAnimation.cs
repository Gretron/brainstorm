using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy Animation Behaviour
/// </summary>
public class EnemyAnimation : MonoBehaviour
{
    private Animator animator;
    private EnemySuspicion suspicion;
    private NavMeshAgent agent;

    private GameObject player;

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
    }

    /// <summary>
    /// Called Once per Frame
    /// </summary>
    void Update()
    {
        Vector3 s = agent.transform.InverseTransformDirection(agent.velocity);
        float speed = s.z;
        float turn = s.x;
        animator.SetFloat("Velocity", speed);

        Suspicion suspicionState = suspicion.suspicion;

        if (suspicion.IsPlayerVisible)
        {
            animator.SetBool("Spotted", true);

            // Get the direction from the enemy to the player
            Vector3 toPlayer = player.transform.position - transform.position;
            Vector3 enemyForward = transform.forward;
            float angle = Vector3.Angle(enemyForward, toPlayer);
            Vector3 cross = Vector3.Cross(enemyForward, toPlayer);

            if (angle > 15f)
            {
                if (cross.y > 0f)
                {
                    animator.SetFloat("Turn", 1);
                }
                else
                {
                    animator.SetFloat("Turn", -1);
                }
            }
            else
            {
                animator.SetFloat("Turn", 0);
            }
        }
        else
        {
            animator.SetFloat("Turn", 0);

            if (speed != 0)
            {
                animator.SetBool("Spotted", false);
            }
        }

        if (suspicionState == Suspicion.Patrol)
        {
            //animator.SetFloat("Velocity", Vector3.Magnitude(agent.velocity));
        }
        else if (suspicionState == Suspicion.Curious) { }
        else if (suspicionState == Suspicion.Alerted) { }
    }
}
