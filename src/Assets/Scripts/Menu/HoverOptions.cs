using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverOptions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
     private Animator animatorReference;


    public static bool isLocked;

    private Animator animator;


    private void Start()
    {
       //animatorReference = GameObject.FindGameObjectWithTag("HoverStartGameExtended").GetComponent<Animator>();
       
       animator = GameObject.FindGameObjectWithTag("HoverOptions").GetComponent<Animator>();

    
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       // if (!isLocked)
        //{
          
            
            animator.SetBool("Hover", true);
        //}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (isLocked)
        //{
      
            animator.SetBool("Hover", false);
           
            
        // }
        // else{
             
        //      animatorReference.SetBool("Hover", false);
             
             
        // }
    }

    public void OnButtonClick()
    {
        //isLocked = false;

       //animatorStart.SetBool("MainHover", true);
     //animatorOptions.SetBool("Hover", true);
      
    }


}
