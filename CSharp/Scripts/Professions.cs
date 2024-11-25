using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Professions : MonoBehaviour
{


    #region Crafting

    [Header("Crafting")]
    public int crafting;
    public int craftingXp;
    public int craftingXpToLvl;
    public TextMeshProUGUI craftingLvlText;
    public TextMeshProUGUI craftingXpProgressText;
    public Image craftingXpProgressBar;

    #endregion

    #region UpdateSkillXP

    public void UpdateSkillXP(string skillName, int xp)
    {
        switch (skillName)
        {
            case "Crafting":
                craftingXp += xp;
                if (craftingXp >= craftingXpToLvl)
                {
                    craftingXp = 0;
                    crafting++;
                    craftingXpToLvl = NewXpLvl(crafting);
                }
                craftingXpProgressText.text = $"{craftingXp:N0}/{craftingXpToLvl:N0}";
                craftingLvlText.text = crafting.ToString();
                craftingXpProgressBar.fillAmount = (float)craftingXp / craftingXpToLvl;
                break;
        }
    }

    private int NewXpLvl(int lvl)
    {
        return lvl * 100;
    }

    #endregion

    public void Start()
    {
        UpdateSkillXP("Crafting", craftingXp);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            UpdateSkillXP("Crafting", 26);
        }
    }


}
