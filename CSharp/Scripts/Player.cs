using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player instance;
    private void Awake() => instance = this;

    #region Properties

    [Header("___")]
    public Inventory inventory;
    public Professions professions;
    public CombatController combatController;



    [Header("Weapon")]
    public Weapon weapon;
    public WeaponData weaponData;

    [Header("Backpack")]
    public Backpack backpack;

    public float travelSpeed = 0;


    #endregion

    #region Start

    void Start()
    {
        ToggleRegeneration(true);
    }

    #endregion

    #region Update
    int speed = 1;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            speed *= 2;
            if (speed > 20)
                speed = 1;

            Time.timeScale = speed;
        }
        if (Input.GetKey(KeyCode.L))
        {
            UpdateXP(10);
        }
    }

    #endregion

    #region Equip
    [Header("Equipments")]
    public AudioClip equipSound;
    public Image weaponHolder;


    public void Equip(Equipment equipment)
    {
        if (this.weapon != null)
            combatController.damage -= this.weapon.damage;

        if (equipment is Weapon weapon)
        {
            this.weapon = weapon;
            combatController.damage = weapon.damage;
            combatController.attackCD = weapon.attackCD;
            combatController.baseCD = weapon.attackCD;
            combatController.hitChance = weapon.hitChance;
            weaponHolder.sprite = weapon.itemSprite;
            SetBonusDamage();
        }

        SoundManager.instance.PlayUISound(equipSound);


        combatController.UpdateStats();
    }

    #endregion



    #region ApplyEffects

    private class EffectCoroutineRunner
    {
        public Coroutine Coroutine;
        public float RemainingTime;
    }

    private Dictionary<EffectType, EffectCoroutineRunner> effects = new Dictionary<EffectType, EffectCoroutineRunner>();

    public void ApplyEffect(PotionEffect effect)
    {
        float duration = effect.duration;
        if (effects.TryGetValue(effect.effectType, out EffectCoroutineRunner existingRunner))
        {
            duration += existingRunner.RemainingTime;
            StopCoroutine(existingRunner.Coroutine);
            RemoveEffect(effect);
            print(existingRunner.RemainingTime + " seconds left");
        }

        EffectCoroutineRunner newRunner = new EffectCoroutineRunner();

        newRunner.Coroutine = StartCoroutine(EffectCoroutine(effect, duration));
        newRunner.RemainingTime = duration;
        effects[effect.effectType] = newRunner;

        print(effect.effectType + " Up for " + duration + " seconds");
    }

    private IEnumerator EffectCoroutine(PotionEffect effect, float duration)
    {
        switch (effect.effectType)
        {
            case EffectType.Strength:
                combatController.damage += new Vector2((int)effect.effectValue, (int)effect.effectValue);
                break;
            case EffectType.AttackSpeed:
                combatController.attackCD -= effect.effectValue;
                break;

            case EffectType.MovementSpeed:
                travelSpeed += effect.effectValue;
                break;

            case EffectType.Defense:
                combatController.defense += (int)effect.effectValue;
                break;
        }
        combatController.UpdateStats();

        yield return new WaitForSeconds(duration);

        RemoveEffect(effect);
    }

    public void RemoveEffect(PotionEffect effect)
    {
        switch (effect.effectType)
        {
            case EffectType.Strength:
                //combatController.damage -= (int)effect.effectValue;
                break;

            case EffectType.AttackSpeed:
                combatController.attackCD += effect.effectValue;
                break;

            case EffectType.MovementSpeed:
                travelSpeed -= effect.effectValue;
                break;

            case EffectType.Defense:
                combatController.defense -= (int)effect.effectValue;
                break;
        }

        combatController.UpdateStats();
        effects.Remove(effect.effectType);
    }


    #endregion


    #region Level

    [Header("Level")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;
    public Image xpBar;

    public int level { get; private set; }
    public int xp { get; private set; }
    public int xpToLvl { get; private set; }
    public int startingHP = 70;


    public void UpdateXP(int xp = 0)
    {
        this.xp += xp;

        if (this.xp >= xpToLvl)
        {
            this.xp = 0;
            LevelUP();
        }
        UpdateXPText();
    }

    public void UpdateXPText()
    {
        levelText.text = $"Lv {level}";
        xpText.text = $"{xp}/{xpToLvl}";
        xpBar.fillAmount = (float)this.xp / xpToLvl;
    }

    private int CalculateExpToLevel()
    {
        return (int)(level * 50 * Mathf.Pow(1.05f, level - 1));
    }

    public void LevelUP()
    {
        level++;
        xpToLvl = CalculateExpToLevel();
        SetHP(combatController.maxHP + 10, combatController.HP);
        SkillManager.instance.IncreaseAttributePoints(1);
        UpdateXPText();
    }

    #endregion



    #region LoadPlayerData


    public void LoadPlayerData(PlayerSaveData playerData)
    {
        if (playerData.level != 0)
        {

            SetHP(playerData.maxHP, playerData.hp);
            level = playerData.level;
            xp = playerData.xp;
            xpToLvl = playerData.xpToLvl;
            inventory.marks = playerData.marks;
            UpdateXPText();
            inventory.UpdateMarksText();
        }
        else
        {
            level = 1;
            xp = 0;
            inventory.marks = 0;
            inventory.UpdateMarksText();
            xpToLvl = CalculateExpToLevel();

            SetHP(startingHP, startingHP);
            UpdateXPText();
        }

    }

    #endregion

    #region UpdateHP

    public void SetHP(int maxHP, int currentHP)
    {
        combatController.maxHP = maxHP;
        combatController.HP = currentHP;
        combatController.HPBar.fillAmount = combatController.HP / (float)combatController.maxHP;
        combatController.delayedHPBar.fillAmount = combatController.HP / (float)combatController.maxHP;
        combatController.UpdateHealthText();
    }

    #endregion

    #region Regeneration

    private Coroutine regenerationCoroutine;

    public void ToggleRegeneration(bool toggle)
    {
        if (!toggle)
        {
            if (regenerationCoroutine != null)
            {
                StopCoroutine(regenerationCoroutine);
            }
        }
        else
        {
            regenerationCoroutine = StartCoroutine(RegenerateHP());
        }
    }


    private float regenInWild;
    private float regenInTown;
    private float healthPerSecond = 2f;

    private IEnumerator RegenerateHP()
    {
        float acumlatedHealth = 0f;
        while (true)
        {
            if (combatController.HP >= combatController.maxHP) yield return null;
            acumlatedHealth += healthPerSecond;
            yield return new WaitForSeconds(2f);

            if (acumlatedHealth >= 1f)
            {
                int amount = (int)acumlatedHealth;
                combatController.Heal(amount);
                acumlatedHealth -= amount;

            }
        }
    }

    public void SetRegeneration(bool isTown)
    {
        if (regenInTown == 0 || regenInWild == 0)
        {
            regenInWild = healthPerSecond;
            regenInTown = healthPerSecond * 2;
        }

        if (isTown) healthPerSecond = regenInTown;
        else healthPerSecond = regenInWild;
    }

    public void UpdateRegen(float amount)
    {
        healthPerSecond = regenInWild;
        healthPerSecond += amount;
        regenInWild = healthPerSecond;
        regenInTown = healthPerSecond * 2;
    }

    #endregion


    #region SetBonusDamage

    public void SetBonusDamage(float percentage = 0)
    {
        combatController.bonusDamagePercentage += percentage;
        combatController.bonusDamagePercentage = Mathf.Round(combatController.bonusDamagePercentage * 100) / 100f;
        combatController.bonusDamage = Mathf.CeilToInt(combatController.damage.x * combatController.bonusDamagePercentage);
        combatController.UpdateStats();
    }

    #endregion

}