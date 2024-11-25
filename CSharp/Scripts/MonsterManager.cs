using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager instance;
    private void Awake()
    {
        instance = this;
        monsterDatas.AddRange(Resources.LoadAll<MonsterData>("Locations/Wilderness"));
    }

    #region Properties

    public List<MonsterData> monsterDatas = new List<MonsterData>();
    public MonsterObject enemyObject;
    public RectTransform enemyHolder;

    public Monster monster;
    public CombatController enemyController;



    public static System.Action<Monster> OnMonsterkilled;



    #endregion

    public void InitWild(WildernessData wild)
    {
        monsterDatas = wild.monstersInArea;
        foreach (Transform child in areaMonstersHolder)
            Destroy(child.gameObject);
        areaMonsters.Clear();

        DisplayMonsters();
    }

    private void DisplayMonsters()
    {
        HuntingManager huntingManager = FindObjectOfType<HuntingManager>();

        huntingManager.monstersToHunt.Clear();

        foreach (MonsterData monsterData in monsterDatas)
        {
            AreaMonsterUI newAreaMonster = Instantiate(areaMonsterPrefab, areaMonstersHolder);
            newAreaMonster.InitAreaMonster(new Monster(monsterData));
            huntingManager.monstersToHunt.Add(monsterData.Name, true);
        }
    }

    #region Area Monsters

    [Header("Area Monsters")]
    public AreaMonsterUI areaMonsterPrefab;
    public RectTransform areaMonstersHolder;
    public List<AreaMonsterUI> areaMonsters = new List<AreaMonsterUI>();
    public Monster currentMonster;



    #region MonsterDeath

    public void MonsterDeath()
    {
        Player.instance.combatController.Heal(5);
        Player.instance.UpdateXP(monster.xpDrop);

        CombatManager.instance.EndBattle();

        DropLoot();
        if (OnMonsterkilled != null)
        {
            OnMonsterkilled(monster);
        }

        ClearMonster();
    }

    #endregion

    #region SelectEnemy

    public void SelectMonster(MonsterData areaMonster)
    {
        if (!enemyController.isDead) return;
        monster = new Monster(areaMonster);
        currentMonster = monster;
        enemyController.InitMonster(monster);
        CombatManager.instance.StartBattle();
    }

    #endregion

    #endregion




    #region DropLoot

    [Header("Loot")]
    public RectTransform lootHolder;
    public List<ItemObject> loot = new List<ItemObject>();
    private LocationManager locationManager;

    private void Start()
    {
        locationManager = FindObjectOfType<LocationManager>();
    }

    public void DropLoot()
    {
        if (UnityEngine.Random.value > monster.dropChance) return;

        foreach (ItemData lootDrop in monster.loot)
        {
            locationManager.DropItem(lootDrop);
        }

    }


    #endregion

    #region ClearEnemy

    public void ClearMonster()
    {
        enemyController.ClearMonster();
    }

    #endregion



    public MonsterData GetMonster(string monsterName)
    {
        foreach (MonsterData monsterData in monsterDatas)
        {
            if (monsterData.Name == monsterName)
                return monsterData;
        }

        return null;
    }
}