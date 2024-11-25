using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HuntingManager : MonoBehaviour
{
    public Dictionary<string, bool> monstersToHunt = new();
    private CombatController combatController;

    public void Start()
    {
        combatController = Player.instance.combatController;
    }


    #region StartHunting

    private bool isHunting;
    public Coroutine huntingCoroutine;
    [SerializeField] private ButtonHandler huntButton;

    public void ToggleHunting(bool OFF)
    {

        if (OFF)
        {
            if (huntingCoroutine != null) StopCoroutine(huntingCoroutine);
            isHunting = false;
            huntButton.ToggleButton(false);
            return;
        }

        if (isHunting)
        {

            if (huntingCoroutine != null) StopCoroutine(huntingCoroutine);
            isHunting = false;

            if (!CombatManager.instance.isCombat)
                Player.instance.combatController.UpdateStatus("Idle");

        }
        else
        {
            huntingCoroutine = StartCoroutine(HuntingCoroutine());
            isHunting = true;

        }


    }

    private IEnumerator HuntingCoroutine()
    {
        while (true)
        {
            while (CombatManager.instance.isCombat) yield return new WaitForSeconds(0.2f);

            Player.instance.combatController.UpdateStatus("Hunting", 1f);
            yield return new WaitForSeconds(1f);
            float c = UnityEngine.Random.value;
            MonsterData monsterData = MonsterManager.instance.monsterDatas[UnityEngine.Random.Range(0, MonsterManager.instance.monsterDatas.Count)];

            print("Found " + monsterData.name);
            if (monstersToHunt[monsterData.Name])
            {
                MonsterManager.instance.SelectMonster(monsterData);
            }


        }
    }

    #endregion


    #region Flee

    [Header("Flee")]
    public float timeToFlee = 1f;
    private bool isFleeing = false;
    Coroutine fleeCoroutine;


    public void Flee()
    {
        if (!isFleeing)
        {
            fleeCoroutine = StartCoroutine(FleeCoroutine());
            combatController.ToggleAttack(false);
            ToggleHunting(true);
            isFleeing = true;
        }
        else
        {
            if (fleeCoroutine != null)
            {
                StopCoroutine(fleeCoroutine);
            }
            combatController.ToggleAttack(true);
            isFleeing = false;
        }
    }

    public IEnumerator FleeCoroutine()
    {
        combatController.UpdateStatus("Fleeing", timeToFlee);

        float currentTime = 0f;
        while (currentTime < timeToFlee)
        {
            currentTime += Time.deltaTime;
            combatController.statusBar.fillAmount = currentTime / timeToFlee;
            combatController.timeText.text = $"{(timeToFlee - currentTime):F1}s";
            yield return null;
        }

        isFleeing = false;
        CombatManager.instance.EndBattle();
        MonsterManager.instance.ClearMonster();

    }

    #endregion






    #region Initalization

    private void OnEnable()
    {
        GameManager.OnPlayerDeath += () => ToggleHunting(true);
    }


    private void OnDisable()
    {
        GameManager.OnPlayerDeath -= () => ToggleHunting(true);
    }



    #endregion



}

