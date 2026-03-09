using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    bool isPressed = false;
    bool isHovered = false;

    private Image buttonImage;
    private Color originalColor;
    [SerializeField] private float darkenAmount = 0.7f; // Multiply color by this value when hovering

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            originalColor = buttonImage.color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonImage != null && !isPressed)
        {
            buttonImage.color = originalColor * darkenAmount;
            isHovered = true;
            Logger.LogCategory("UI", "EndButton: On PointerEnter");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonImage != null && !isPressed)
        {
            buttonImage.color = originalColor;
            isHovered = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonImage != null)
        {
            isPressed = true;
            buttonImage.color = Color.white;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buttonImage != null && isHovered)
        {
            isPressed = false;
            buttonImage.color = originalColor * darkenAmount;
        }
        else if (buttonImage != null && !isHovered)
        {
            isPressed = false;
            buttonImage.color = originalColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UIEventManager.OnMoveButtonClicked();
        Logger.LogCategory("UI", "MoveButton: OnPointerClick");
    }
}
