using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Health Behaviour
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    /// <summary>
    /// Called Before First Frame Update
    /// </summary>
    void Start() { }

    /// <summary>
    /// Called Once per Frame
    /// </summary>
    void Update() { }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Debug.Log("takedmg");
            GameManager.Instance.TakePlayerDamage(-other.gameObject.GetComponent<Bullet>().Damage);
        }
    }
}
