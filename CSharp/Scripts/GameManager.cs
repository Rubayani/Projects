using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    void Awake() => instance = this;


    #region Properties

    public ItemDatabase itemDatabase;

    #endregion

    #region Start

    public void Start()
    {

    }

    #endregion

    #region Update

    void Update()
    {

    }

    #endregion


    #region AtGameLaunch

    public bool isFirst;
    [SerializeField] private GameObject startGuide;

    public void AtGameLaunch()
    {
        if (!isFirst)
        {
            UiManager.instance.AtGameLaunch();
            startGuide.SetActive(true);
            isFirst = true;
        }
    }

    #endregion

    #region ShowUpdate

    #endregion




    #region PlayerDeath

    [Header("Death")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private int deathTimer;

    public void PlayerDeath()
    {
        UiManager.instance.TogglePanel(deathPanel);
        StartCoroutine(DeathCoroutine());
        CombatManager.instance.EndBattle();
        OnPlayerDeath?.Invoke();
    }

    private IEnumerator DeathCoroutine()
    {
        TextMeshProUGUI deathText = deathPanel.GetComponentInChildren<TextMeshProUGUI>();
        float timePassed = deathTimer;

        while (timePassed >= 0)
        {
            deathText.text = $"You Died!\nRespawning in {timePassed}s";
            yield return new WaitForSeconds(1);
            timePassed -= 1;
        }

        UiManager.instance.ClosePanel(deathPanel);
        MonsterManager.instance.ClearMonster();
        Player player = Player.instance;
        LocationManager locationManager = FindObjectOfType<LocationManager>();

        locationManager.FastTravel(locationManager.townData);
        player.combatController.isDead = false;
        player.combatController.Heal((int)(player.combatController.maxHP));
        player.combatController.UpdateStatus("Idle");

    }

    #endregion


    #region Events


    public static Action OnPlayerDeath;


    #endregion


}
