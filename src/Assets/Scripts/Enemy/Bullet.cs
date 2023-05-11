using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bullet Behaviour
/// </summary>
public class Bullet : MonoBehaviour
{
    /// <summary>
    /// Bullet Rigidbody
    /// </summary>
    private Rigidbody rb;

    [SerializeField]
    /// <summary>
    /// Bullet Force
    /// </summary>
    private float force = 10;

    [SerializeField]
    /// <summary>
    /// Bullet Damage
    /// </summary>
    private float damage = 5;

    /// <summary>
    /// Bullet Damage
    /// </summary>
    public float Damage
    {
        get { return damage; }
    }

    /// <summary>
    /// Called Before First Frame Update
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.up * force, ForceMode.Impulse);

        Destroy(this.gameObject, 3);
    }

    /// <summary>
    /// Called Once per Frame
    /// </summary>
    void Update() { }
}
