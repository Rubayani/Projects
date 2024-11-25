using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public enum ItemHolder
{
    None,
    Drop,
    Player,
    Shop,
}

public class ItemObject : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Properties

    public Item item;
    public ItemHolder itemHolder;

    [Header("References")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemValueText;
    public TextMeshProUGUI itemWeightText;
    public TextMeshProUGUI itemValWtText;

    #endregion

    #region Start

    void Start()
    {

    }

    public void Update()
    {
    }

    public void InitItem(Item item, int quantity = 1)
    {
        this.item = item;

        item.quantity = quantity;

        itemIcon.sprite = item.itemIcon;

        UpdateText();
    }

    #endregion

    #region UpdateText

    public void UpdateText()
    {
        if (item is Equipment equipment)
            itemNameText.text = $"{item.Name} +{equipment.level}";
        else
            itemNameText.text = item.Name + (item.quantity > 1 ? $" ({item.quantity})" : "");

        itemValueText.text = item.value % 1 == 0 ? $"{item.value}" : $"{item.value:F1}";
        itemWeightText.text = item.weight % 1 == 0 ? $"{item.weight}" : $"{item.weight:F1}";
        if (item.weight != 0)
            itemValWtText.text = item.value / item.weight % 1 == 0 ? $"{item.value / item.weight:N0}" : $"{item.value / item.weight:F1}";
    }

    public void PickUp()
    {
        if (itemHolder != ItemHolder.Drop) return;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Player.instance.inventory.AddItem(item, item.quantity);
            MonsterManager.instance.loot.Remove(this);
            ToolTipManager.instance.itemToolTip.gameObject.SetActive(false);
            Destroy(gameObject);

            return;
        }
        Player.instance.inventory.AddItem(item, 1);

        item.quantity -= 1;
        UpdateText();
        if (item.quantity <= 0)
        {
            MonsterManager.instance.loot.Remove(this);
            ToolTipManager.instance.itemToolTip.gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    #endregion


    #region Give

    public void Give()
    {
        if (itemHolder != ItemHolder.Player && itemHolder != ItemHolder.Shop) return;

        if (itemHolder == ItemHolder.Player)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                ShopManager.instance.SellItem(this, item.quantity);
            }
            else
            {
                ShopManager.instance.SellItem(this, 1);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                ShopManager.instance.BuyItem(this, 10);
            }
            else
            {
                ShopManager.instance.BuyItem(this, 1);
            }
        }

    }

    #endregion


    #region Use

    public void Use()
    {
        if (item is Potion potion)
        {
            potion.UsePotion(this);
        }
        else if (item is Equipment equipment)
        {
            equipment.Equip();
        }
        else if (item is Book book)
        {
            TechniqueManager skillManager = TechniqueManager.instance;
            Player player = Player.instance;
            if (skillManager.skills.Contains(book.skill) && skillManager.skills[skillManager.skills.IndexOf(book.skill)].isMaxLvl) return;
            book.LearnSkill();
            Player.instance.inventory.RemoveItem(this, 1);
        }
    }

    #endregion

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemHolder == ItemHolder.Drop)
        {
            PickUp();
            return;
        }

        if (ShopManager.instance.isTrading)
        {
            Give();
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ForgeManager blacksmithManager = FindObjectOfType<ForgeManager>();
            if (blacksmithManager.isUpgrading)
            {
                blacksmithManager.DisplayItem(item);
                ToolTipManager.instance.itemToolTip.isHoveringItem = false;
                ToolTipManager.instance.itemToolTip.HideItemTip();
            }
            else
                Use();
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTipManager.instance.itemToolTip.isHoveringItem = true;
        ToolTipManager.instance.itemToolTip.DisplayItemTip(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipManager.instance.itemToolTip.isHoveringItem = false;
        ToolTipManager.instance.itemToolTip.HideItemTip();
    }


}
