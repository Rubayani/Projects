using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AreaMonsterUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Properties

    public Monster enemy;

    public TextMeshProUGUI Name;
    public TextMeshProUGUI lvlText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI damageText;
    public Image icon;
    public bool isSelected = true;

    #endregion



    #region InitAreaMonster

    public void InitAreaMonster(Monster enemy)
    {
        this.enemy = enemy;
        Name.text = enemy.Name;
        healthText.text = enemy.maxHP.ToString();
        damageText.text = $"{enemy.Damage.x} - {enemy.Damage.y}";
        icon.sprite = enemy.areaSprite;
    }

    #endregion


    #region SelectMonster

    public void SelectMonster(bool select)
    {
        HuntingManager huntingManager = FindObjectOfType<HuntingManager>();
        if (huntingManager.monstersToHunt.TryGetValue(enemy.Name, out bool value))
        {
            huntingManager.monstersToHunt[enemy.Name] = select;
        }
    }

    #endregion


    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }


}
