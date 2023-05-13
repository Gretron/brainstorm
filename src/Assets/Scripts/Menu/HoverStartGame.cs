using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverStartGame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
     private Animator animatorReference;

    // public Image imageToAppear;
    // public Image imageToAppear2;
    // public Color hoverColor;
    // public Color emptyColor = new Color(1f, 1f, 1f, 0f);

    public static bool isLocked;
    // private Color originalColor;
    private Animator animator;
    // RectTransform rectTransform;
    // public Image image;
    // float width = 1;

    private void Start()
    {
       //animatorReference = GameObject.FindGameObjectWithTag("HoverStartGameExtended").GetComponent<Animator>();
       
       animator = GameObject.FindGameObjectWithTag("HoverStartGame").GetComponent<Animator>();

    
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       // if (!isLocked)
        //{
          
            
            animator.SetBool("MainHover", true);
        //}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (isLocked)
        //{
      
            animator.SetBool("MainHover", false);
           
            
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
