using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverRestart : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
     private Animator animatorReference;


    public static bool isLocked;

    private Animator animator;
    private QuitTrigger blockHover;


    private void Start()
    {
  
       animator = GameObject.FindGameObjectWithTag("HoverRestart").GetComponent<Animator>();
       blockHover = GameObject.FindGameObjectWithTag("Hover Image").GetComponent<QuitTrigger>();


    
    }

    public void OnPointerEnter(PointerEventData eventData)
    {        
            if(blockHover.Set()){
                animator.SetBool("HoverRestart", true);
            }
            

    }

    public void OnPointerExit(PointerEventData eventData)
    {

      
            animator.SetBool("HoverRestart", false);
           
            
    }

    public void OnButtonClick()
    {

      
    }


}
