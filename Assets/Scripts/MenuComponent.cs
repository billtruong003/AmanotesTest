using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image ComponentButton;
    public TextMeshProUGUI ComponentText;

    public Color normalColor = Color.white;
    public Color hoverColor = Color.grey;
    public Color pressedColor = Color.grey;

    public float scaleNormal = 1f;
    public float scaleHover = 1.1f;
    public float scalePressed = 0.95f;

    public float tweenTime = 0.1f;

    [SerializeField] private MenuController menuController;

    public MenuController.MenuFunction menuFunction;

    void Awake()
    {
        menuController = GetComponentInParent<MenuController>();
    }

    void Start()
    {
        ComponentText.color = normalColor;
        transform.localScale = new Vector3(scaleNormal, scaleNormal, scaleNormal);
        // Đảm bảo alpha ban đầu của ComponentButton là 0
        if (ComponentButton != null)
        {
            Color buttonColor = ComponentButton.color;
            buttonColor.a = 0f;
            ComponentButton.color = buttonColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, new Vector3(scaleHover, scaleHover, scaleHover), tweenTime).setEase(LeanTweenType.easeOutQuad);
        ComponentText.color = hoverColor;

        // Thay đổi alpha của ComponentButton khi hover
        if (ComponentButton != null)
        {
            LeanTween.alpha(ComponentButton.rectTransform, 180f / 255f, tweenTime).setEase(LeanTweenType.easeOutQuad);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, new Vector3(scaleNormal, scaleNormal, scaleNormal), tweenTime).setEase(LeanTweenType.easeOutQuad);
        ComponentText.color = normalColor;

        // Đưa alpha của ComponentButton về 0 khi không hover
        if (ComponentButton != null)
        {
            LeanTween.alpha(ComponentButton.rectTransform, 0f, tweenTime).setEase(LeanTweenType.easeOutQuad);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, new Vector3(scalePressed, scalePressed, scalePressed), tweenTime).setEase(LeanTweenType.easeOutQuad);
        ComponentText.color = pressedColor;

        // Thay đổi alpha của ComponentButton khi nhấn
        if (ComponentButton != null)
        {
            LeanTween.alpha(ComponentButton.rectTransform, 220f / 255f, tweenTime).setEase(LeanTweenType.easeOutQuad);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, new Vector3(scaleHover, scaleHover, scaleHover), tweenTime).setEase(LeanTweenType.easeOutQuad);
        ComponentText.color = hoverColor;

        // Trả về alpha của ComponentButton khi thả ra (về hover)
        if (ComponentButton != null)
        {
            LeanTween.alpha(ComponentButton.rectTransform, 180f / 255f, tweenTime).setEase(LeanTweenType.easeOutQuad);
        }

        if (menuController != null)
        {
            menuController.HandleMenuClick(this);
        }
    }
}