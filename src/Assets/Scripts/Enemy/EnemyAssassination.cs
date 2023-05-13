using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy Assassination Behaviour
/// </summary>
public class EnemyAssassination : MonoBehaviour
{
    /// <summary>
    /// Enemy Animator Component
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Max Assassination Range
    /// </summary>
    private float assassinationRange = 5f;

    /// <summary>
    /// Assassination Target
    /// </summary>
    private GameObject target;

    /// <summary>
    /// Is Assassinating Flag
    /// </summary>
    private bool isAssassinating;

    /// <summary>
    /// Is Animating Flag
    /// </summary>
    private bool isAnimating = false;

    /// <summary>
    /// Called Before First Frame Update
    /// </summary>
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Called Once per Frame
    /// </summary>
    void Update()
    {
        // If Is Currently Assassinating...
        if (isAssassinating)
        {
            Vector3 targetDirection = -(target.transform.position - transform.position).normalized;

            Vector3 newDirection = Vector3.RotateTowards(
                target.transform.forward,
                targetDirection,
                Time.deltaTime * 5,
                0.0f
            );

            target.transform.rotation = Quaternion.LookRotation(newDirection);

            transform.position = Vector3.MoveTowards(
                transform.position,
                target.transform.position + targetDirection * 1.5f,
                Time.deltaTime * 5
            );

            string clipName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

            if (clipName == "AssassinationStab")
            {
                isAnimating = true;
            }
            else
            {
                if (isAnimating)
                {
                    GetComponent<Movement>().enabled = true;
                    isAssassinating = false;

                    target.GetComponent<EnemyHealth>().TakeDamage(100);
                }
            }
        }
        else
        {
            // Get Enemies Within Range
            Collider[] hitColliders = Physics.OverlapSphere(
                transform.position,
                assassinationRange,
                LayerMask.GetMask("Enemy")
            );

            target = null;

            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (!GameObject.ReferenceEquals(gameObject, hitColliders[i].gameObject))
                {
                    target = hitColliders[i].gameObject;
                    break;
                }
            }

            if (target == null)
            {
                Debug.Log("No");
            }
            else
            {
                Debug.Log(target.name);
            }

            // If Player Presses R
            if (Input.GetKeyDown(KeyCode.R) && target != null)
            {
                GetComponent<Movement>().enabled = false;

                target.GetComponent<EnemySuspicion>().enabled = false;
                target.GetComponent<NavMeshAgent>().enabled = false;
                target.GetComponent<Rigidbody>().useGravity = false;
                target.GetComponent<CapsuleCollider>().enabled = false;

                StartCoroutine(PlayAssassinationAnimation(1f));

                isAssassinating = true;
            }
        }
    }

    /// <summary>
    /// Coroutine to Play Assassination Animation
    /// </summary>
    private IEnumerator playAssassinationAnimation;

    /// <summary>
    /// Coroutine to Play Assassination Animation
    /// </summary>
    /// <param name="delay"> Delay in Seconds </param>
    IEnumerator PlayAssassinationAnimation(float delay)
    {
        // Apply Delay
        yield return new WaitForSeconds(delay);

        animator.SetTrigger("Assassinate");

        yield return null;
    }
}
