using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonsterToolTip : MonoBehaviour
{
    #region Properties

    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private TextMeshProUGUI InfoText;

    #endregion

    public void Update()
    {
        transform.position = Input.mousePosition + new Vector3(0, 2);
    }

    #region DisplayTip

    public void DisplayTip(MonsterData monsterData)
    {
        NameText.text = monsterData.Name;
        InfoText.text = $"{monsterData.foundIn[0].locationName}";
        //gameObject.SetActive(true);
    }

    #endregion

    #region HideTip

    public void HideTip()
    {
        gameObject.SetActive(false);
    }

    #endregion

}
