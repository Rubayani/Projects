using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.IK;
using UnityEngine.UI;

public class CombatController : MonoBehaviour
{

    #region Properties
    public Image background;
    public Image monsterIcon;

    [Header("Health")]
    public int maxHP;
    public int HP;
    public TextMeshProUGUI hpText;
    public Image HPBar;
    public Image delayedHPBar;

    [Header("Hit Chance")]
    public int attackValue;
    public float hitChance;
    public TextMeshProUGUI hitChanceText;


    [Header("Damage")]
    public Vector2 damage;
    public float bonusDamagePercentage = 0;
    public int bonusDamage = 0;
    public TextMeshProUGUI damageText;

    [Header("Defense")]
    public int defense;
    public TextMeshProUGUI defenseText;

    [Header("Dodge")]
    public int dodge;
    public TextMeshProUGUI dodgeText;


    [Space(10)]
    public float baseCD;
    public float attackCD;

    public float lifeSteal = 0.05f;


    public bool isPlayer = false;

    public CombatController target;


    public bool isDead = false;
    [Header("Skills")]
    public Dictionary<Type, Technique> skillDictionary = new Dictionary<Type, Technique>();
    #endregion


    #region UpdateStats

    public void UpdateStats()
    {
        hitChance = Mathf.Clamp(CalculateHitChance(), 0, 100);
        hitChanceText.text = hitChance > 0 ? $"{hitChance}%" : "";

        damage = CalculateDamage();
        damageText.text = $"{(int)damage.x} - {(int)damage.y}";
    }

    #endregion


    #region CalculateHitChance

    private float CalculateHitChance()
    {
        if (target == null)
            return 0;

        return 50 + (attackValue - target.dodge) / 2;
    }

    #endregion


    #region CalculateDamage

    private Vector2 CalculateDamage()
    {
        Vector2 actualDamage = new(damage.x + bonusDamage, damage.y + bonusDamage);
        if (!isPlayer) return actualDamage;

        Weapon weapon = Player.instance.weapon;
        if (weapon.Name == "") return actualDamage;

        return actualDamage;
    }
    #endregion


    #region Heal

    public void Heal(int amount)
    {
        int previousHP = HP;
        HP += amount;

        if (HP > maxHP)
            HP = maxHP;

        StartHPBarAnimation(previousHP, HP, true);
    }

    #endregion

    #region AnimateDelayedHPBar

    private Coroutine hpBarCoroutine;

    public Color healColor = Color.green;
    public Color damageColor = Color.red;


    public void StartHPBarAnimation(int previousHP, int newHP, bool isHealing)
    {
        UpdateHealthText();
        if (hpBarCoroutine != null)
        {
            StopCoroutine(hpBarCoroutine);
        }

        hpBarCoroutine = StartCoroutine(AnimateDelayedHPBar(previousHP, newHP, isHealing));
    }

    private IEnumerator AnimateDelayedHPBar(int previousHP, int newHP, bool isHealing)
    {
        float elapsed = 0f;
        float duration = 0.5f;
        float startFill = previousHP / (float)maxHP;
        float targetFill = newHP / (float)maxHP;

        if (isHealing)
        {

            delayedHPBar.color = healColor;
            delayedHPBar.fillAmount = targetFill;
            yield return new WaitForSeconds(0.1f);
            if (isDead) yield break;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                HPBar.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / duration);
                yield return null;
            }
        }
        else
        {
            delayedHPBar.color = damageColor;
            HPBar.fillAmount = targetFill;
            yield return new WaitForSeconds(0.1f);
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                delayedHPBar.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / duration);
                yield return null;
            }
        }

        delayedHPBar.fillAmount = targetFill;
    }


    #region UpdateHealthText

    public void UpdateHealthText()
    {
        hpText.text = $"{HP}/{maxHP}";
    }

    #endregion

    #endregion


    #region Attack

    public Coroutine attackCoroutine;

    public void ToggleAttack(bool toggle)
    {
        if (!toggle)
        {
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }
        }
        else
        {
            attackCoroutine = StartCoroutine(AttackCoroutine());
        }
    }

    public IEnumerator AttackCoroutine()
    {
        while (!target.isDead)
        {
            if (isDead) yield break;

            UpdateStatus("Attacking", attackCD);
            yield return new WaitForSeconds(attackCD);

            if (target.isDead) yield break;

            bool isCrit = false;
            target.TakeDamage((int)UnityEngine.Random.Range(damage.x, damage.y), isCrit);
        }
    }


    #endregion

    public DamageText damagePopText;


    #region TakeDamage

    public void TakeDamage(int amount, bool isCrit)
    {
        DamageText newText = Instantiate(damagePopText, background.transform.parent);

        float v = UnityEngine.Random.value;

        bool isHit = v < target.hitChance;
        if (!isHit)
        {
            amount = 0;
            newText.InitTextDamage(amount, isPlayer, isCrit);
            return;
        }

        int previousHP = HP;
        HP -= amount;
        newText.InitTextDamage(amount, isPlayer, isCrit);

        if (HP <= 0)
        {
            HP = 0;
            Death();
        }

        UpdateHealthText();
        StartHPBarAnimation(previousHP, HP, false);
    }




    #endregion



    #region Death

    public void Death()
    {
        isDead = true;
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        if (target.attackCoroutine != null)
        {
            target.StopCoroutine(target.attackCoroutine);
            target.attackCoroutine = null;
        }
        if (isPlayer)
        {
            GameManager.instance.PlayerDeath();
        }
        else
        {
            MonsterManager.instance.MonsterDeath();
        }

        RestartStatus();
        statusText.text = "Dead";
    }

    #endregion





    #region Status

    [Header("Status")]

    public TextMeshProUGUI statusText;
    public TextMeshProUGUI timeText;
    public Image statusBar;

    public Coroutine statusCoroutine;


    public void UpdateStatus(string status, float CD = 0)
    {
        statusText.text = status;
        if (statusCoroutine != null)
        {
            StopCoroutine(statusCoroutine);

        }
        statusCoroutine = StartCoroutine(FillStatusBar(CD));
    }

    #region FillStatusBar

    private IEnumerator FillStatusBar(float CD)
    {
        float currentTime = 0f;
        while (currentTime < CD)
        {
            currentTime += Time.deltaTime;
            statusBar.fillAmount = currentTime / CD;
            timeText.text = $"{(CD - currentTime):F1}s";
            yield return null;
        }
        statusBar.fillAmount = 0f;
        timeText.text = "";
        statusText.text = "Idle";
    }

    #endregion

    #region RestartStatus

    public void RestartStatus()
    {
        if (statusCoroutine != null)
            StopCoroutine(statusCoroutine);
        statusBar.fillAmount = 0f;
        timeText.text = "";
        statusText.text = "Idle";
    }

    #endregion

    #endregion


    #region Later




    #region ClearMonster

    public void ClearMonster()
    {
        StopAllCoroutines();
        hpText.text = "";
        statusText.text = "";
        timeText.text = "";
        hitChanceText.text = "";
        damageText.text = "";
        defenseText.text = "";
        isDead = true;
        statusBar.fillAmount = 0f;
        HPBar.fillAmount = 1f;
        monsterIcon.gameObject.SetActive(false);
    }

    #endregion



    #region Update

    void Update()
    {
        if (!isPlayer) return;
        if (DebugManager.instance.isDebuging)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                TakeDamage(10, false);
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                Heal(maxHP);
            }
        }
    }

    #endregion


    #region InitMonster

    public void InitMonster(Monster monster)
    {
        isDead = false;
        monsterIcon.sprite = monster.battleSprite;
        monsterIcon.gameObject.SetActive(true);

        attackValue = monster.attackValue;
        damage = monster.Damage;

        maxHP = monster.maxHP;
        HP = maxHP;

        attackCD = monster.attackCD;
        baseCD = monster.attackCD;

        target = Player.instance.combatController;
        Player.instance.combatController.target = this;

        HPBar.fillAmount = 1f;
        UpdateHealthText();
        UpdateStats();

        target.UpdateStats();
        target.ToggleAttack(true);

        ToggleAttack(true);


    }

    #endregion

    #region Start

    void Start()
    {
        if (isPlayer)
            skillDictionary = TechniqueManager.instance.GetSkills();

        if (isPlayer)
        {
            UpdateStats();
            RestartStatus();
        }
    }

    #endregion

    #endregion

    #region Events

    public event Action OnCriticalHit;
    #endregion


}