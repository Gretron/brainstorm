using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartTrigger : MonoBehaviour
{
    public Animator animator;

   
    public void PlayAnimation()
    {
        animator.SetTrigger("RestartTrigger");
    }
}

