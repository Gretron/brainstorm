using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitTrigger : MonoBehaviour
{
    public Animator animator;

   
    public void PlayAnimation()
    {
        animator.SetTrigger("QuitTrigger");
    }
}

