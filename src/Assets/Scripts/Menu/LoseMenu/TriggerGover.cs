using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGover : MonoBehaviour
{
     public Animator animator;
    public Animator hoverAnimator;
    public bool isSet;
   
    public void PlayAnimation()
    {
        isSet = true;
    }

    public bool Set(){
        return isSet;
    }
}
