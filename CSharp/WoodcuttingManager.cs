using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WoodcuttingManager : MonoBehaviour
{
    [HideInInspector] public List<Tree> trees = new();

    [Header("Tree")]
    [SerializeField] private Image treeImage;



    public void InitWoodcutting(List<Tree> trees)
    {
        this.trees = trees;
        currentTreeIndex = 0;
        DisplayTree();
    }


    public void DisplayTree()
    {
        currentTree = trees[currentTreeIndex];
        treeImage.sprite = currentTree.treeSprite;


    }

    #region TreeNav

    private int currentTreeIndex = 0;

    public void GetNextTree()
    {
        if (trees.Count == 0) return;

        currentTreeIndex = (currentTreeIndex + 1) % trees.Count;
        DisplayTree();
    }

    public void GetPreviousTree()
    {
        if (trees.Count == 0) return;

        currentTreeIndex = (currentTreeIndex - 1 + trees.Count) % trees.Count;
        DisplayTree();
    }

    #endregion


    #region StartChopping

    private bool isChopping;
    public Coroutine choppingCoroutine;
    [SerializeField] private ButtonHandler chopButton;
    private Tree currentTree;

    public void ToggleChopping(bool OFF)
    {
        if (OFF)
        {
            if (choppingCoroutine != null) StopCoroutine(choppingCoroutine);
            isChopping = false;
            chopButton.ToggleButton(false);
            return;
        }

        if (isChopping)
        {
            if (choppingCoroutine != null) StopCoroutine(choppingCoroutine);
            isChopping = false;
            Player.instance.combatController.UpdateStatus("Idle");
        }
        else
        {
            choppingCoroutine = StartCoroutine(ChopCoroutine());
            isChopping = true;
        }
    }

    private IEnumerator ChopCoroutine()
    {
        float timeToChop = Mathf.Clamp(currentTree.hitPoints / 11f, 1, Mathf.Infinity);

        while (true)
        {

            Player.instance.combatController.UpdateStatus("Chopping", timeToChop);
            yield return new WaitForSeconds(timeToChop);

            locationManager.DropItem(currentTree.logDroped);
        }
    }



    #endregion


    private LocationManager locationManager;

    private void Start()
    {
        locationManager = FindObjectOfType<LocationManager>();
    }



}
