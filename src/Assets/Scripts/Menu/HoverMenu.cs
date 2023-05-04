using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image imageToAppear;
    Color color;


private void Start() {
    // Set the alpha value of the image to 0
   
    color = imageToAppear.color;
    imageToAppear.color = new Color(1f, 1f, 1f, 0f);
}

    public void OnPointerEnter(PointerEventData eventData)
    {
        
       imageToAppear.color = color;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       

        imageToAppear.color = new Color(1f, 1f, 1f, 0f);
    }
}
