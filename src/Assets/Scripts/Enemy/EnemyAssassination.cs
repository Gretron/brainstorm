using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    /// Last Assassination Target
    /// </summary>
    private GameObject lastTarget;

    /// <summary>
    /// Is Assassinating Flag
    /// </summary>
    private bool isAssassinating;

    /// <summary>
    /// Is Animating Flag
    /// </summary>
    private bool isAnimating = false;

    /// <summary>
    /// Has Syringe Flag
    /// </summary>
    public bool hasSyringe = true;

    /// <summary>
    /// Player Host Possession
    /// </summary>
    private HostPossession possession;

    /// <summary>
    /// Called Before First Frame Update
    /// </summary>
    void Start()
    {
        possession = GameObject.FindGameObjectWithTag("Player").GetComponent<HostPossession>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Called Once per Frame
    /// </summary>
    void Update()
    {
        if (!GameObject.ReferenceEquals(possession.enemy, gameObject))
        {
            if (lastTarget != null)
            {
                lastTarget.transform
                    .Find("Canvas/AssassinationIndicator")
                    .GetComponent<Image>()
                    .enabled = false;
                lastTarget = null;
            }

            return;
        }

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

            newDirection.y = 0;

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

                    hasSyringe = false;
                    isAssassinating = false;

                    target.GetComponent<EnemyHealth>().TakeDamage(100);
                }
            }
        }
        else
        {
            if (!hasSyringe)
            {
                return;
            }

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

            if (lastTarget != null && !GameObject.ReferenceEquals(target, lastTarget))
            {
                lastTarget.transform
                    .Find("Canvas/AssassinationIndicator")
                    .GetComponent<Image>()
                    .enabled = false;
            }

            if (target != null)
            {
                target.transform
                    .Find("Canvas/AssassinationIndicator")
                    .GetComponent<Image>()
                    .enabled = true;

                lastTarget = target;

                // If Player Presses R
                if (Input.GetKeyDown(KeyCode.R))
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
