using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;
    private void Awake()
    {
        instance = this;
    }

    [Header("Battle")]
    [SerializeField] private Button mapButton;
    [SerializeField] private Button fleeButton;
    public bool isCombat;

    #region StartBattle

    public void StartBattle()
    {
        mapButton.interactable = false;
        fleeButton.interactable = true;
        isCombat = true;
    }

    #endregion

    #region EndBattle

    public void EndBattle()
    {
        mapButton.interactable = true;
        fleeButton.interactable = false;
        isCombat = false;
    }


    #endregion

}
