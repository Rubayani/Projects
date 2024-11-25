using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public enum ButtonType
{
    Normal,
    SelectOneOfMany,
    Toggle,
}

public class ButtonHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] private ButtonType buttonType;
    [SerializeField] private AudioClip buttonSound;

    private Image buttonImage;
    private TextMeshProUGUI innerText;

    [Header("Colors")]
    private Color baseColor;
    private Color baseTextColor;
    [SerializeField] private Color clickColor;
    [SerializeField] private Color textClickColor;
    [SerializeField] private float transitionDuration = 0.3f;

    public bool isToggled { private set; get; }

    public void Start()
    {
        buttonImage = GetComponent<Image>();
        baseColor = buttonImage.color;

        innerText = GetComponentInChildren<TextMeshProUGUI>();
        baseTextColor = innerText.color;

        if (name == "Hunting")
        {
            isToggled = true;
            buttonImage.color = clickColor;
            innerText.color = textClickColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            switch (buttonType)
            {
                case ButtonType.Normal:

                    break;

                case ButtonType.SelectOneOfMany:

                    if (isToggled) return;
                    FindObjectOfType<LocationManager>().ToggleButtons(this);
                    ToggleButton(true);
                    break;

                case ButtonType.Toggle:
                    ToggleButton(!isToggled);
                    break;
            }
        }
    }


    public void ToggleButton(bool toggle)
    {
        if (toggle == isToggled) return;

        isToggled = toggle;

        if (toggle == true)
        {
            buttonImage.DOColor(clickColor, transitionDuration);
            innerText.DOColor(textClickColor, transitionDuration);
        }
        else
        {
            buttonImage.DOColor(baseColor, transitionDuration);
            innerText.DOColor(baseTextColor, transitionDuration);
        }
    }




    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isToggled)
        {
            Color hoverColor = Color.Lerp(baseColor, clickColor, 0.5f);

            buttonImage.DOColor(hoverColor, transitionDuration);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isToggled)
        {
            buttonImage.DOColor(baseColor, transitionDuration);
        }
    }
}
