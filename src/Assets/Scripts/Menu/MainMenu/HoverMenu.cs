using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
     private Animator animatorReference;

    // public Image imageToAppear;
    // public Image imageToAppear2;
    // public Color hoverColor;
    // public Color emptyColor = new Color(1f, 1f, 1f, 0f);

    public static bool isLocked;
    // private Color originalColor;
    private Animator animatorStart;
    private Animator animatorOptions;
    // RectTransform rectTransform;
    // public Image image;
    // float width = 1;

    private void Start()
    {
       //animatorReference = GameObject.FindGameObjectWithTag("HoverStartGameExtended").GetComponent<Animator>();
       
       animatorStart = GameObject.FindGameObjectWithTag("HoverStartGame").GetComponent<Animator>();
       animatorOptions = GameObject.FindGameObjectWithTag("HoverOptions").GetComponent<Animator>();

    
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       // if (!isLocked)
        //{
          
            
            animatorStart.SetBool("MainHover", true);
            animatorOptions.SetBool("Hover", true);
        //}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (isLocked)
        //{
      
            animatorStart.SetBool("MainHover", false);
            animatorOptions.SetBool("Hover", false);
           
            
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
