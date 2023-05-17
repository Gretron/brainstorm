using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExtendHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private Animator animator;
    private bool isPressed;
    

    private void Start()
    {
        // animator = GetComponent<Animator>();
        // isPressed = false;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        // if (isPressed)
        // {
        //     animator.SetBool("Hover", true);
            
        // }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // if (isPressed)
        // {
        //     animator.SetBool("Hover", false);
        
        // }
    }

    public void clicked()
    {
        // isPressed = true;
        // animator.SetBool("Hover", true);
    }
}
