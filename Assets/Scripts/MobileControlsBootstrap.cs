using System.Collections.Frozen;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileControlsBootstrap : MonoBehaviour
{
    void Awake()
    {
        EnsureEventSystem();
        Canvas canvas = EnsureCanvas();

        CreateButton(canvas.transform, "LeftButton", new Vector2(20, 20), MobileButton.ButtonType.Left);
        CreateButton(canvas.transform, "RightButton", new Vector2(160 + 20, 20), MobileButton.ButtonType.Right);
        CreateButton(canvas.transform, "JumpButton", new Vector2(-160, 20), MobileButton.ButtonType.Jump, rightSide: true);
    }

    void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }

    Canvas EnsureCanvas()
    {
        Canvas canvas = FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            GameObject go = new GameObject("Canvas");
            canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            go.AddComponent<GraphicRaycaster>();
        }

        return canvas;
    }

    void CreateButton(Transform parent, string name, Vector2 offset, MobileButton.ButtonType type, bool rightSide = false)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent);

        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(120, 120);

        // Anchoring
        if (rightSide)
        {
            rt.anchorMin = new Vector2(1, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(1, 0);
        }
        else
        {
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0, 0);
        }

        rt.anchoredPosition = offset;

        // Visual
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.4f);

        // Interaction
        btnObj.AddComponent<CanvasRenderer>();
        MobileButton mb = btnObj.AddComponent<MobileButton>();
        mb.buttonType = type;
        if (type == MobileButton.ButtonType.Jump)
        {
            AddChildWithTextComponent(btnObj, "[]");
        }
        else if (type == MobileButton.ButtonType.Left)
        {
            AddChildWithTextComponent(btnObj, "<");
        }
        else if (type == MobileButton.ButtonType.Right)
        {
            AddChildWithTextComponent(btnObj, ">");
        }
        else
        {
        }
    }
    public GameObject AddChildWithTextComponent(GameObject obj, string text)
    {
        GameObject child = new GameObject(text);
        child.transform.SetParent(obj.transform);
        child.transform.localPosition = Vector3.zero;
        child.transform.localScale = Vector3.one;
        child.transform.localRotation = Quaternion.identity;
        if (!child.TryGetComponent<RectTransform>(out var rectTransform))
        {
            rectTransform = child.AddComponent<RectTransform>();
        }
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = new Vector2(0, 0);
        rectTransform.offsetMax = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        AddTextComponent(child, text);
        return child;
    }
    public void AddTextComponent(GameObject obj, string text)
    {
        Text textComponent = obj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.fontSize = 80;
        textComponent.color = Color.black;
        textComponent.font = Resources.Load<Font>("PixelOperator8");
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
    }
}