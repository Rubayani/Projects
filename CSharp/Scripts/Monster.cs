using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Monster
{
    #region Properties
    
    public string Name;
    public int level;
    public Sprite areaSprite;
    public Sprite battleSprite;
    public int maxHP;
    public int HP;

    public int attackValue;
    public Vector2 Damage;
    public float attackCD;

    [Header("Loot")]
    public float chanceOfMarks;
    public Vector2 marksDrop;

    public float dropChance;
    public List<ItemData> loot;
    public int xpDrop;


    #endregion
    
    public Monster(MonsterData monsterData)
    {
        Name = monsterData.Name;
        level = monsterData.level;
        areaSprite = monsterData.AreaSprite;
        battleSprite = monsterData.BattleSprite;

        maxHP = monsterData.maxHP;
        HP = maxHP;

        attackValue = monsterData.attackValue;
        Damage = monsterData.damage;
        attackCD = monsterData.attackCD;


        chanceOfMarks = monsterData.chanceOfMarks;
        marksDrop = monsterData.MarksDrop;

        dropChance = monsterData.dropChance;
        loot = monsterData.loot;
        xpDrop = monsterData.xpDrop;
    }
}
