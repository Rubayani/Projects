using System;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgeManager : MonoBehaviour
{
    public Equipment gear;

    public TextMeshProUGUI itemNameText;
    public Image itemIconImage;
    [SerializeField] private Button upgradeButton;

    [Header("Stats Prefab & Container")]
    public GameObject statPrefab;
    public GameObject bonusPrefab;
    public Transform statContainer;

    public bool isUpgrading;

    [Header("Cost")]
    [SerializeField] private TextMeshProUGUI upgradeCostText;



    public static System.Action OnUpgrade;

    #region DisplayItem

    public void DisplayItem(Item item)
    {
        if (item is not Equipment) return;
        if (item is Backpack) return;

        itemNameText.text = item.Name;
        itemIconImage.sprite = item.itemSprite;
        gear = item as Equipment;

        itemNameText.gameObject.SetActive(true);
        itemIconImage.gameObject.SetActive(true);

        foreach (Transform child in statContainer)
        {
            Destroy(child.gameObject);
        }

        if (item is Weapon weapon)
        {



            GameObject damageStat = Instantiate(statPrefab, statContainer);
            TextMeshProUGUI[] statTexts = damageStat.GetComponentsInChildren<TextMeshProUGUI>();
            statTexts[0].text = "Damage";
            statTexts[1].text = $"{weapon.damage.x} - {weapon.damage.y}";
            if (weapon.level < 6)
                statTexts[2].text = $"{weapon.damage.x + 4} - {weapon.damage.y + 4}";
            else
                statTexts[2].text = "";

            GameObject cdStat = Instantiate(statPrefab, statContainer);
            TextMeshProUGUI[] cdTexts = cdStat.GetComponentsInChildren<TextMeshProUGUI>();
            cdTexts[0].text = "Attack CD";
            cdTexts[1].text = $"{weapon.attackCD}";
            if (weapon.level < 6)
                cdTexts[2].text = $"{Mathf.Round((weapon.attackCD - 0.1f) * 100f) / 100f}";
            else
                cdTexts[2].text = "";


        }

        string upgradeCostTextString = gear.level < 6 ? $"Upgrade cost: <color={(Player.instance.inventory.marks >= gear.upgradeCost * gear.level ? "#7FE291" : "#9F5C5C")}>{gear.upgradeCost * gear.level}</color>" : "";
        upgradeCostText.text = upgradeCostTextString;

        if (gear.level < 6 && Player.instance.inventory.marks >= gear.upgradeCost * gear.level) upgradeButton.interactable = true;
    }

    #endregion

    #region UpgradeItem

    public void UpgradeItem()
    {
        if (gear == null) return;
        if (gear.level >= 6 || Player.instance.inventory.marks < gear.upgradeCost * gear.level) return;

        Player.instance.inventory.UpdateMarks(-gear.upgradeCost * gear.level);
        gear.level++;
        Player.instance.inventory.items.Find(item => item.item == gear).UpdateText();
        if (gear is Weapon weapon)
        {
            weapon.damage.x += 4;
            weapon.damage.y += 4;
            weapon.attackCD -= 0.1f;
            weapon.attackCD = Mathf.Round(weapon.attackCD * 100f) / 100f;
            DisplayItem(weapon);
            Player.instance.Equip(weapon);
            if (weapon.Name == "Sword" && OnUpgrade != null) OnUpgrade();
        }

    }


    public void StartSmithing()
    {
        ResetPanel();
        isUpgrading = true;
    }

    #region ResetPanel

    public void ResetPanel()
    {
        gear = null;
        isUpgrading = false;
        foreach (Transform child in statContainer)
        {
            Destroy(child.gameObject);
        }
        itemNameText.gameObject.SetActive(false);
        itemIconImage.gameObject.SetActive(false);
        upgradeCostText.text = "";
        upgradeButton.interactable = false;
    }

    #endregion

    #endregion


    // #region BonusStats

    // [Header("BonusStats")]
    // [SerializeField] BonusStatsDatabase bonusStatsDatabase;
    // public List<BonusStat> bonusStats = new List<BonusStat>();

    // #region RerollStats

    // public void RerollStats()
    // {
    //     bonusStats.Clear();

    //     bonusStats.AddRange(bonusStatsDatabase.allBonus);
    //     bonusStats.AddRange(bonusStatsDatabase.weaponBonus);
    //     Weapon weapon = gear as Weapon;

    //     weapon.ResetStats();
    //     int bounesinEquipment = weapon.bonusList.Count;
    //     for (int i = 0; i < weapon.bonusAmount - bounesinEquipment; i++)
    //     {
    //         if (bonusStats.Count == 0)
    //             break;
    //         BonusStat pickedStats = bonusStats[UnityEngine.Random.Range(0, bonusStats.Count)];
    //         bonusStats.Remove(pickedStats);
    //         weapon.bonusList.Add(new BonusStat(pickedStats.statName, (int)(UnityEngine.Random.Range(0.01f, pickedStats.maxValue) * 100f) / 100f));
    //     }
    //     DisplayItem(weapon);
    // }

    // #endregion


    // #endregion
}