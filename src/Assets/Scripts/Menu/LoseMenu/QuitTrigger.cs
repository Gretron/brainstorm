using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitTrigger : MonoBehaviour
{
    public Animator animator;
    public Animator hoverAnimator;
    public bool isSet;
   
    public void PlayAnimation()
    {
        isSet = true;
        animator.SetTrigger("QuitTrigger");
    }

    public bool Set(){
        return isSet;
    }
}

