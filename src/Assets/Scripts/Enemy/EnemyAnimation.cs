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

    /// <summary>
    /// Called Before First Frame Update
    /// </summary>
    void Start()
    {
        animator = GetComponent<Animator>();
        suspicion = GetComponent<EnemySuspicion>();
        agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Called Once per Frame
    /// </summary>
    void Update()
    {
        Debug.Log(agent.velocity);

        Vector3 s = agent.transform.InverseTransformDirection(agent.velocity);
        float speed = s.z;
        float turn = s.x;
        animator.SetFloat("Velocity", speed);
        animator.SetFloat("Turn", turn);

        Suspicion suspicionState = suspicion.suspicion;

        if (suspicionState == Suspicion.Patrol)
        {
            //animator.SetFloat("Velocity", Vector3.Magnitude(agent.velocity));
        }
        else if (suspicionState == Suspicion.Curious) { }
        else if (suspicionState == Suspicion.Alerted) { }
    }
}
