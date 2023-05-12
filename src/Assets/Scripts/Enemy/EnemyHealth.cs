using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    /// <summary>
    /// Collider of Possession Attach
    /// </summary>
    private BoxCollider possessionCollider;

    private Rigidbody[] ragdollBodies;
    private BoxCollider[] boxColliders;
    private SphereCollider[] sphereColliders;
    private CapsuleCollider[] capsuleColliders;

    public int ragdollbodiescount = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameObject bones = transform.Find("Human").gameObject;

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

        ragdollbodiescount = ragdollBodies.Length;
    }

    // Update is called once per frame
    void Update() { }
}
