using UnityEngine;
using UnityEngine.EventSystems;

public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonType
    {
        Left,
        Right,
        Jump
    }

    public ButtonType buttonType;

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case ButtonType.Left:
                MobileInput.Horizontal = -1f;
                break;

            case ButtonType.Right:
                MobileInput.Horizontal = 1f;
                break;

            case ButtonType.Jump:
                MobileInput.JumpPressed = true;
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case ButtonType.Left:
                if (MobileInput.Horizontal < 0f)
                    MobileInput.Horizontal = 0f;
                break;

            case ButtonType.Right:
                if (MobileInput.Horizontal > 0f)
                    MobileInput.Horizontal = 0f;
                break;

            case ButtonType.Jump:
                // optional: do nothing (jump is one-shot)
                break;
        }
    }
}