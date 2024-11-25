using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    #region Properties

    public RectTransform inventoryHolder;
    public ItemObject itemObjectPrefab;
    public List<ItemObject> items = new List<ItemObject>();

    [Header("Weight")]
    public float maxWeight;
    public float weight;
    [SerializeField] private TextMeshProUGUI weightText;


    [Header("Items icons")]
    public Sprite swordIcon;
    public Sprite backpackIcon;

    public List<EquipmentData> equipmentData;
    public List<ItemData> itemDatas;
    #endregion

    public void Start()
    {
        UpdateWeight();
        UpdateMarksText();

        foreach (EquipmentData equipmentData in equipmentData)
        {
            //AddItem(CastItem(null, equipmentData));
        }
    }


    #region AddItem

    public void AddItem(Item newItem, int amount = 1)
    {
        weight += newItem.weight * amount;

        weight = Mathf.Round(weight * 100f) / 100f;

        weight = Mathf.Max(weight, 0f);
        UpdateWeight();
        if (newItem is not Equipment)
            foreach (ItemObject item in items)
            {
                if (item.item.Name == newItem.Name)
                {
                    item.item.quantity += amount;
                    item.UpdateText();
                    return;
                }
            }

        ItemObject newItemObject = Instantiate(itemObjectPrefab, inventoryHolder);
        newItemObject.name = newItem.Name;
        newItemObject.itemHolder = ItemHolder.Player;
        newItemObject.InitItem(CastItem(newItem), amount);
        items.Add(newItemObject);
    }

    #endregion

    #region RemoveItem

    public void RemoveItem(ItemObject itemObject, int amount = 1)
    {
        weight -= itemObject.item.weight * amount;

        weight = Mathf.Round(weight * 100f) / 100f;

        weight = Mathf.Max(weight, 0f);

        UpdateWeight();

        if (itemObject.item.quantity - amount <= 0)
        {
            items.Remove(itemObject);
            ToolTipManager.instance.itemToolTip.gameObject.SetActive(false);
            Destroy(itemObject.gameObject);
            return;
        }

        itemObject.item.quantity -= amount;
        itemObject.UpdateText();
    }


    #endregion


    #region UpdateWeight

    public void UpdateWeight()
    {
        weightText.text = $" {weight}/{maxWeight}";
    }

    #endregion


    [Header("Marks")]
    public int marks;
    public TextMeshProUGUI marksText;

    #region UpdateMarks

    public void UpdateMarks(int value)
    {
        marks += value;
        UpdateMarksText();
    }

    public void UpdateMarksText()
    {
        marksText.text = $"{marks:N0}";
    }


    #endregion


    #region EquipBackpack

    public void EquipBackpack(Backpack backpack)
    {
        Player player = Player.instance;
        if (player.backpack.Name == backpack.Name) return;
        maxWeight -= player.backpack.bonusWeight;
        maxWeight += backpack.bonusWeight;
        player.backpack = backpack;
        UpdateWeight();
    }
    #endregion

    #region SortInventory

    public enum SortBy
    {
        Name,
        Quantity,
        Value,
        Weight,
        ValueWeight
    }

    private SortBy lastSortBy;
    private bool isAscending = true;

    public void SortInventoryDropdownChanged(string selectedValue)
    {
        SortBy sortBy = (SortBy)Enum.Parse(typeof(SortBy), selectedValue);
        SortInventory(sortBy);
    }

    public void SortInventory(SortBy sortBy)
    {
        if (sortBy == lastSortBy)
        {
            isAscending = !isAscending;
        }
        else
        {
            isAscending = false;
        }

        lastSortBy = sortBy;

        switch (sortBy)
        {
            case SortBy.Name:
                items.Sort((a, b) =>
                {
                    int nameComparison = a.item.Name.CompareTo(b.item.Name);
                    return isAscending ? -nameComparison : nameComparison;
                });
                break;
            case SortBy.Quantity:
                items.Sort((a, b) =>
                {
                    int quantityComparison = a.item.quantity.CompareTo(b.item.quantity);
                    return isAscending ? quantityComparison : -quantityComparison;
                });
                break;
            case SortBy.Value:
                items.Sort((a, b) =>
                {
                    int valueComparison = a.item.value.CompareTo(b.item.value);
                    return isAscending ? valueComparison : -valueComparison;
                });
                break;
            case SortBy.Weight:
                items.Sort((a, b) =>
                {
                    int weightComparison = a.item.weight.CompareTo(b.item.weight);
                    return isAscending ? weightComparison : -weightComparison;
                });
                break;
            case SortBy.ValueWeight:
                items.Sort((a, b) =>
                {
                    int valueWeightComparison = (a.item.value / a.item.weight).CompareTo((b.item.value / b.item.weight));
                    return isAscending ? valueWeightComparison : -valueWeightComparison;
                });
                break;
        }

        for (int i = 0; i < items.Count; i++)
        {
            items[i].transform.SetSiblingIndex(i);
        }
    }


    #endregion


    #region CastItem

    public Item CastItem(Item item = null, ItemData itemData = null)
    {
        if (item != null)
        {
            if (item is Potion potion)
            {
                return new Potion(potion.Name, potion.description, potion.itemSprite, potion.itemIcon, potion.value, potion.weight, potion.itemsRequired, potion.makeXP, potion.potionEffects);
            }
            else if (item is Weapon weapon)
            {
                return new Weapon(weapon.Name, weapon.description, weapon.itemSprite, weapon.itemIcon, weapon.value, weapon.weight, weapon.bonusAmount, weapon.hitChance, weapon.damage, weapon.attackCD, weapon.itemsRequired, weapon.makeXP, weapon.upgradeCost);
            }
            else if (item is Backpack backpack)
            {
                return new Backpack(backpack.Name, backpack.description, backpack.itemSprite, backpack.itemIcon, backpack.value, backpack.weight, backpack.bonusAmount, backpack.bonusAmount, backpack.itemsRequired, backpack.makeXP, 0);
            }
            else if (item is Book book)
            {
                return new Book(book.Name, book.description, book.itemSprite, book.itemIcon, book.value, book.weight, book.itemsRequired, book.makeXP, book.skill.skillName);
            }
            else
            {
                return new Item(item.Name, item.description, item.itemSprite, item.itemIcon, item.value, item.weight, item.itemsRequired, item.makeXP);
            }
        }
        else
        {
            if (itemData is PotionData potion)
            {
                return new Potion(potion.Name, potion.Description, potion.itemSprite, potion.itemIcon, potion.value, potion.weight, potion.itemsRequired, potion.makeXP, potion.potionEffects);
            }
            else if (itemData is WeaponData weapon)
            {
                return new Weapon(weapon.Name, weapon.Description, weapon.itemSprite, weapon.itemIcon, weapon.value, weapon.weight, weapon.bonusAmount, weapon.hitChance, weapon.damage, weapon.attackCD, weapon.itemsRequired, weapon.makeXP, weapon.upgradeCost);
            }
            else if (itemData is BackpackData backpack)
            {
                return new Backpack(backpack.Name, backpack.Description, backpack.itemSprite, backpack.itemIcon, backpack.value, backpack.weight, backpack.bonusAmount, backpack.weightBonus, backpack.itemsRequired, backpack.makeXP, 0);
            }
            else if (itemData is BookData book)
            {
                return new Book(book.Name, book.Description, book.itemSprite, book.itemIcon, book.value, book.weight, book.itemsRequired, book.makeXP, book.skillName);
            }
            else
            {
                return new Item(itemData.Name, itemData.Description, itemData.itemSprite, itemData.itemIcon, itemData.value, itemData.weight, itemData.itemsRequired, itemData.makeXP);
            }
        }
    }

    #endregion

    private void Update()
    {
    }
}