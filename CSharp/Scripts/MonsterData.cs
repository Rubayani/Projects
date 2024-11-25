using System.Collections.Generic;
using UnityEngine;

public enum Race
{
    None,
    Animal,
    Humanoid,
    Undead,
    Nature,
    Elemental,
    Giant,
}

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Monsters/MonsterData", order = 1)]
public class MonsterData : ScriptableObject
{
    public string Name;
    public int level;
    public Race race;

    public List<LocationData> foundIn;
    
    [Header("Sprites")]
    public Sprite AreaSprite;
    public Sprite BattleSprite;

    [Header("Stats")]
    public int maxHP;
    public int attackValue;
    public Vector2 damage;
    public float attackCD;

    [Header("Drops")]
    public float chanceOfMarks;
    public Vector2 MarksDrop;
    public float dropChance;
    public List<ItemData> loot;
    public int xpDrop;
}