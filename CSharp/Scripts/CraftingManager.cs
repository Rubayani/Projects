using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager instance;
    private void Awake() => instance = this;

    #region Properties
    [Header("Display")]
    public TextMeshProUGUI itemName;
    public Image itemSprite;

    [Header("Ingrediants")]
    public Transform materialsHolder;
    public GameObject materialPrefab;

    private Item item;
    [SerializeField] private List<ItemObject> itemObjectsRequired;

    public ItemDatabase craftingDatabase;
    #endregion

    #region PickItem

    public void PickItem(ItemData itemData)
    {
        item = Player.instance.inventory.CastItem(null, itemData);
        itemName.text = item.Name;
        itemSprite.sprite = item.itemSprite;
        itemName.gameObject.SetActive(true);
        itemSprite.gameObject.SetActive(true);
        DisplayMaterials();
    }

    #endregion

    #region DisplayMaterials

    private void DisplayMaterials()
    {
        foreach (Transform child in materialsHolder) Destroy(child.gameObject);

        itemObjectsRequired = new List<ItemObject>();
        foreach (ItemsRequired material in item.itemsRequired)
        {
            GameObject newMaterial = Instantiate(materialPrefab, materialsHolder);
            newMaterial.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = material.itemData.itemSprite;

            ItemObject playerItem = Player.instance.inventory.items.Find(i => i != null && i.item.Name == material.itemData.Name);
            itemObjectsRequired.Add(playerItem);
            int amountInPlayer = playerItem != null ? playerItem.item.quantity : 0;

            TextMeshProUGUI playerAmountText = newMaterial.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
            playerAmountText.text = amountInPlayer.ToString();

            TextMeshProUGUI amountRequiredText = newMaterial.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
            amountRequiredText.text = material.amount.ToString();

            Color insufficient = new Color(255f / 255, 110f / 255, 110f / 255);
            Color sufficient = new Color(60f / 255, 255f / 255, 60f / 255);
            bool hasEnough = amountInPlayer >= material.amount;

            playerAmountText.color = hasEnough ? sufficient : insufficient;
            amountRequiredText.color = hasEnough ? sufficient : insufficient;
            newMaterial.transform.GetChild(1).GetChild(1).GetComponent<Image>().color = hasEnough ? sufficient : insufficient;
        }
    }

    #endregion

    #region CraftItem

    public void CraftItem()
    {
        if (item == null) return;
        foreach (ItemObject itemObject in itemObjectsRequired)
        {
            if (itemObject == null || itemObject.item.quantity < item.itemsRequired.Find(m => m.itemData.Name == itemObject.item.Name).amount) return;
        }

        Player.instance.inventory.AddItem(item);
        foreach (ItemsRequired material in item.itemsRequired)
        {
            Player.instance.inventory.RemoveItem(itemObjectsRequired.Find(i => i.item.Name == material.itemData.Name), material.amount);
        }
        Player.instance.professions.UpdateSkillXP("Crafting", item.makeXP);
        DisplayMaterials();
    }

    #endregion

    #region ResetPanel

    public void ResetPanel()
    {
        foreach (Transform child in materialsHolder) Destroy(child.gameObject);
        itemName.gameObject.SetActive(false);
        itemSprite.gameObject.SetActive(false);
    }

    #endregion
}
