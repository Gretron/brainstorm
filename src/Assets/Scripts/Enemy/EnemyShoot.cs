using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    /// Look Rig GameObject
    /// </summary>
    private Rig aimRig;

    /// <summary>
    /// Called Before First Frame Update
    /// </summary>
    void Start()
    {
        // Get References
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        suspicion = GetComponent<EnemySuspicion>();
    }

    /// <summary>
    /// Called Once per Frame
    /// </summary>
    void Update()
    {
        Suspicion suspicionState = suspicion.suspicion;

        if (suspicionState == Suspicion.Patrol) { }
        else if (suspicionState == Suspicion.Curious) { }
        else if (suspicionState == Suspicion.Alerted) { }
    }
}
