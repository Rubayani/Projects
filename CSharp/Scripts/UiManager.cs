using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    private void Awake() => instance = this;

    private GameObject activePanel;

    [SerializeField] private GameObject updatesPanel;
    public void Start()
    {
    }


    public void Update()
    {
        if (activePanel != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) TogglePanel(activePanel);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape)) BringUpSideTab();
        }
    }


    [SerializeField] private GameObject tab;
    private bool isTabOpen = false;
    #region BringUpSideTab


    public void BringUpSideTab()
    {
        RectTransform tabRect = tab.GetComponent<RectTransform>();
        float tabWidth = tabRect.rect.width;

        tabRect.DOKill();

        float targetX = isTabOpen ? -tabWidth : 0;
        float distance = Mathf.Abs(tabRect.localPosition.x - targetX);
        float duration = distance / 400f;

        tabRect.DOLocalMoveX(targetX, duration).SetEase(Ease.OutQuad);
        isTabOpen = !isTabOpen;
    }

    #endregion

    #region Panel

    [Header("Panel")]

    [SerializeField] private GameObject backgroundPanel;

    public void TogglePanel(GameObject panel)
    {
        if (activePanel != null) activePanel.SetActive(false);

        if (activePanel == panel)
        {
            ClosePanel(panel);
            return;
        }

        activePanel = panel;
        backgroundPanel.SetActive(true);
        panel.SetActive(true);

    }


    private bool isFirst = false;

    #region ClosePanel

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
        backgroundPanel.SetActive(false);
        activePanel = null;
        if (!isFirst)
        {
            BringUpSideTab();
            isFirst = true;
        }
    }
    #endregion


    #endregion


    #region ShowUpdate

    private bool showUpdate = false;

    public void ShowUpdate()
    {

    }

    #endregion


    #region Quit

    public void Quit()
    {
        Application.Quit();
    }

    #endregion

    public void OpenDiscordLink()
    {
        Application.OpenURL("https://discord.gg/6Zez6eE2ZV");
    }

    public void AtGameLaunch()
    {
        isTabOpen = true;
        RectTransform tabTran = tab.GetComponent<RectTransform>();
        tabTran.localPosition = new Vector2(0, 0);
        TogglePanel(updatesPanel);
    }
}
