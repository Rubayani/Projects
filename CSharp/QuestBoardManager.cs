using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestBoardManager : MonoBehaviour
{

    #region Properties

    #endregion

    #region Start

    void Start()
    {
        StartCoroutine(QuestBoardTimer());
    }

    #endregion

    #region Update

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) StartCoroutine(GenerateQuests());
    }

    #endregion



    #region QuestBoard

    [Header("QuestBoard")]
    [SerializeField] private Transform boardQuestHolder;
    [SerializeField] private BoardQuestObject boardQuestObject;
    [SerializeField] private int questAmount;

    #region GenerateQuests

    private IEnumerator GenerateQuests()
    {
        foreach (Transform child in boardQuestHolder)
            Destroy(child.gameObject);


        boardQuestHolder.GetComponent<HorizontalLayoutGroup>().enabled = true;
        for (int i = 0; i < questAmount; i++)
        {
            MonsterData monsterData = MonsterManager.instance.monsterDatas[UnityEngine.Random.Range(0, MonsterManager.instance.monsterDatas.Count)];
            MonsterRequirement monsterRequirement = new MonsterRequirement(monsterData.Name, Random.Range(4, 10));

            string questName = $"{monsterData.Name} Hunt";
            string description = $"Hunt down {monsterRequirement.amount} {monsterData.Name}s.";

            Reward reward = new Reward(Random.Range(10, 50), UnityEngine.Random.Range(50, 200));

            HuntQuest huntQuest = new HuntQuest(questName, description, 0, reward, null, true, null, monsterRequirement);

            BoardQuestObject newQuest = Instantiate(boardQuestObject, boardQuestHolder);
            newQuest.Init(huntQuest);
        }


        yield return null;
        boardQuestHolder.GetComponent<HorizontalLayoutGroup>().enabled = false;
    }


    #endregion



    [Header("Time")]
    [SerializeField] private float questUpdateInterval;
    [SerializeField] private TextMeshProUGUI timerText;

    private float timer;

    #region Timer

    private IEnumerator QuestBoardTimer()
    {
        StartCoroutine(GenerateQuests());
        timer = questUpdateInterval;

        while (true)
        {
            while (timer > 0)
            {
                timer -= 1;
                UpdateTimerUI();
                yield return new WaitForSeconds(1);
            }

            timer = questUpdateInterval;
            StartCoroutine(GenerateQuests());
        }
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }


    #endregion




    #endregion
}
