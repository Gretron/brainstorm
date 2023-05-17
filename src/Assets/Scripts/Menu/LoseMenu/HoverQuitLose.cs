using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverQuitLose : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{


    private Animator animator;
    private TriggerGover blockHover;


    private void Start()
    {
      
       
       animator = GameObject.FindGameObjectWithTag("HoverQuitLose").GetComponent<Animator>();
       blockHover = GameObject.FindGameObjectWithTag("QuitImage").GetComponent<TriggerGover>();

    
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

            if(blockHover.Set()){
            animator.SetBool("HoverQuit", true);
            }
 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
  
      
            animator.SetBool("HoverQuit", false);
           
        
    }

    public void OnButtonClick()
    {

      
    }


}
