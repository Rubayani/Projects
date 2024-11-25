using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager instance;
    private void Awake() => instance = this;

    public bool isDebuging = false;

    #region Start

    void Start()
    {
    }

    #endregion

    #region Update

    void Update()
    {

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F1))
        {
            isDebuging = true;
            Player.instance.combatController.attackCD = 0;
            Player.instance.combatController.damage = new Vector2(1111, 1111);
        }

        if (!isDebuging) return;
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F))
        {
            FindObjectOfType<LocationManager>().FastTravel(FindObjectOfType<LocationManager>().forestData);
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
        {
            FindObjectOfType<LocationManager>().FastTravel(FindObjectOfType<LocationManager>().townData);
        }
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C))
        {
            FindObjectOfType<LocationManager>().FastTravel(FindObjectOfType<LocationManager>().coastData);
        }
        AddMoney();
    }

    #endregion

    #region Name

    #endregion
    public void AddMoney()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.X))
            Player.instance.inventory.UpdateMarks(100);

    }
}
