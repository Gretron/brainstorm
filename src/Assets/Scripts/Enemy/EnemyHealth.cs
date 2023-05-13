using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy Health Behaviour
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    /// <summary>
    /// Collider of Possession Attach
    /// </summary>
    private BoxCollider possessionCollider;

    /// <summary>
    /// Enemy Animator
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Enemy Suspicion Behaviour
    /// </summary>
    private EnemySuspicion suspicion;

    /// <summary>
    /// Enemy NPC Navigation
    /// </summary>
    private NavMeshAgent agent;

    /// <summary>
    /// Host Possession Behaviour
    /// </summary>
    private HostPossession possession;

    /// <summary>
    /// Enemy Main Collider
    /// </summary>
    private CapsuleCollider mainCollider;

    /// <summary>
    /// Ragdoll Rigidbodies
    /// </summary>
    private Rigidbody[] ragdollBodies;

    /// <summary>
    /// Ragdoll Colliders
    /// </summary>
    private BoxCollider[] boxColliders;
    private SphereCollider[] sphereColliders;
    private CapsuleCollider[] capsuleColliders;

    /// <summary>
    /// Enemy Max Health
    /// </summary>
    public float MaxHealth { get; } = 100;

    [SerializeField]
    /// <summary>
    /// Enemy Health Value
    /// </summary>
    public float Health
    {
        get { return health; }
    }

    [SerializeField]
    /// <summary>
    /// Enemy Health Value
    /// </summary>
    private float health = 100;

    /// <summary>
    /// Dead Flag
    /// </summary>
    private bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject bones = transform.Find("Human").gameObject;
        possession = GameObject.FindGameObjectWithTag("Player").GetComponent<HostPossession>();

        animator = GetComponent<Animator>();
        suspicion = GetComponent<EnemySuspicion>();
        agent = GetComponent<NavMeshAgent>();
        mainCollider = GetComponent<CapsuleCollider>();

        ragdollBodies = bones.GetComponentsInChildren<Rigidbody>();
        boxColliders = bones.GetComponentsInChildren<BoxCollider>();
        sphereColliders = bones.GetComponentsInChildren<SphereCollider>();
        capsuleColliders = bones.GetComponentsInChildren<CapsuleCollider>();

        foreach (Rigidbody rb in ragdollBodies)
            rb.isKinematic = true;

        foreach (BoxCollider bc in boxColliders)
            bc.enabled = false;

        foreach (SphereCollider sc in sphereColliders)
            sc.enabled = false;

        foreach (CapsuleCollider cc in capsuleColliders)
            cc.enabled = false;

        possessionCollider.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Health <= 0 && !dead)
        {
            Die();

            if (possession.isPossessed && GameObject.ReferenceEquals(gameObject, possession.enemy))
            {
                possession.Unpossess();
                GameManager.Instance.hostHealth = null;
            }
        }
    }

    /// <summary>
    /// Take Health Damage
    /// </summary>
    /// <param name="value"> Damage Value </param>
    public void TakeDamage(float value)
    {
        health = Mathf.Max(Health - value, 0);
    }

    /// <summary>
    /// Called When Collision With Another Collider Has Begun
    /// </summary>
    /// <param name="other"> Other Collision Info </param>
    void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Bullet")
        {
            TakeDamage(20);
        }
    }

    /// <summary>
    /// Make Enemy Dead
    /// </summary>
    void Die()
    {
        foreach (Rigidbody rb in ragdollBodies)
            rb.isKinematic = false;

        foreach (BoxCollider bc in boxColliders)
            bc.enabled = true;

        foreach (SphereCollider sc in sphereColliders)
            sc.enabled = true;

        foreach (CapsuleCollider cc in capsuleColliders)
            cc.enabled = true;

        animator.enabled = false;
        suspicion.enabled = false;
        agent.enabled = false;
        mainCollider.enabled = false;

        possessionCollider.enabled = false;
    }
}
