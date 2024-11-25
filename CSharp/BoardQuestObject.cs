using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardQuestObject : MonoBehaviour
{
    HuntQuest huntQuest;

    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    #region Init

    public void Init(HuntQuest quest)
    {
        huntQuest = quest;
        questNameText.text = quest.questName;
        descriptionText.text = quest.GetQuestDescription();

    }


    #endregion

    public void AcceptQuest()
    {
        QuestManager.instance.AcceptQuest(huntQuest);
        Destroy(gameObject);
    }
}
