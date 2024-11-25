using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    #region Items

    [Header("Items")]
    [SerializeField] private Image itemIcon;
    public bool isInItemTip;
    public bool isHoveringItem;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI itemInfoText;


    #region Display

    public void DisplayItemTip(ItemObject item)
    {

        itemIcon.sprite = item.item.itemSprite;
        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        nameText.text = item.item.Name;
        itemInfoText.text = "";

        if (item.item is Equipment equipment)
        {
            if (item.item is Weapon weapon)
            {
                itemInfoText.text = $"Damage {weapon.damage.x} - {weapon.damage.y}\n" +
                                    $"CD {weapon.attackCD}\n";

            }

            foreach (BonusStat bonusStat in equipment.bonusList)
            {
                itemInfoText.text += $"+{bonusStat.value * 100}% {bonusStat.statName}\n";
            }
        }


        itemInfoText.text += $"Value {item.item.value}\n" +
                             $"Weight {item.item.weight}";

        itemInfoText.text += item.item.description != "" ? $"\n\n<size=70%><b><i>\"{item.item.description}\"<i><b>" : "";




        Vector3[] itemCorners = new Vector3[4];
        itemRectTransform.GetWorldCorners(itemCorners);
        Vector3 rightSidePosition = itemCorners[2];

        transform.position = rightSidePosition;

        gameObject.SetActive(true);
    }


    #endregion

    #region Hide
    private Coroutine hideCoroutine;

    public void HideItemTip()
    {
        if (hideCoroutine != null)
             StopCoroutine(hideCoroutine);

        if (gameObject.activeSelf)
            hideCoroutine = StartCoroutine(HideItemTipCoroutine());
    }

    private IEnumerator HideItemTipCoroutine()
    {
        yield return new WaitForSeconds(0.05f);

        if (isInItemTip || isHoveringItem) yield break;
        itemIcon.sprite = null;
        gameObject.SetActive(false);
    }



    #endregion

    #endregion


    public void OnPointerEnter(PointerEventData eventData)
    {
        // isInItemTip = true;
        // if (hideCoroutine != null)
        //     StopCoroutine(hideCoroutine);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isInItemTip = false;
        HideItemTip();
    }
}
