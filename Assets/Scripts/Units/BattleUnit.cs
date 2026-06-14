using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] protected string unitName = "Unit";

    [Header("Stats")]
    [SerializeField] protected int maxHP = 100;
    [SerializeField] protected int currentHP = 100;
    [SerializeField] protected int attack = 10;
    [SerializeField] protected int defense = 5;

    [Header("Advanced Stats")]
    [SerializeField] protected int fireResistancePercent = 0;
    [SerializeField] protected int dodgeChancePercent = 0;
    [SerializeField] protected int extraTurnChancePercent = 0;

    protected bool isDead = false;

    protected List<StatusEffect> activeEffects = new List<StatusEffect>();
    protected UnitKnowledgeProfile knowledgeProfile = new UnitKnowledgeProfile();

    public string UnitName => unitName;
    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;
    public int Attack => GetAttackValue();
    public int Defense => GetDefenseValue();
    public int FireResistancePercent => fireResistancePercent;
    public int DodgeChancePercent => dodgeChancePercent;
    public int ExtraTurnChancePercent => extraTurnChancePercent;
    public bool IsDead => isDead;
    public int ATK;
    public int MATK;
    public int DFS;
    public int MDFS;

    public int PRC;
    public int FRC;

    public int DOG;
    public int CRT;
    public int DCH;

    public int INTL;

    protected virtual void Awake()
    {
        currentHP = maxHP;
        isDead = false;
    }

    public void RevealStat(StatType statType)
    {
        knowledgeProfile.SetVisibility(statType, StatVisibilityLevel.Revealed);
    }

    public void HideStat(StatType statType)
    {
        knowledgeProfile.SetVisibility(statType, StatVisibilityLevel.Hidden);
    }

    public bool IsStatRevealed(StatType statType)
    {
        return knowledgeProfile.IsRevealed(statType);
    }

    public void SetStatVisibility(StatType statType, bool revealed)
    {
        knowledgeProfile.SetVisibility(
            statType,
            revealed ? StatVisibilityLevel.Revealed : StatVisibilityLevel.Hidden
        );
    }

    public void ApplyFinalDamage(int amount)
    {
        if (isDead)
            return;

        currentHP -= amount;
        currentHP = Mathf.Max(0, currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(int amount)
    {
        if (isDead)
            return;

        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);

        //Debug.Log($"{unitName} healed {amount} HP. Current HP: {currentHP}/{maxHP}");
    }

    public virtual int GetAttackValue()
    {
        int totalAttack = attack;

        for (int i = 0; i < activeEffects.Count; i++)
        {
            totalAttack += activeEffects[i].damageBonus;
        }

        return totalAttack;
    }

    public virtual int GetDefenseValue()
    {
        int totalDefense = defense;

        for (int i = 0; i < activeEffects.Count; i++)
        {
            totalDefense += activeEffects[i].defenseBonus;
        }

        return totalDefense;
    }

    public virtual void AddStatusEffect(StatusEffect effect)
    {
        activeEffects.Add(effect);
        //Debug.Log($"{unitName} gained {effect.effectName}.");
    }

    public virtual void ProcessStatusEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            StatusEffect effect = activeEffects[i];

            if (effect.isBurn && effect.burnDamagePerTurn > 0 && !isDead)
            {
                DamageRequest request = new DamageRequest
                {
                    Attacker = null,
                    Target = this,
                    BaseDamage = effect.burnDamagePerTurn,
                    DamageType = DamageType.Fire,
                    CanBeDodged = false,
                    IgnoresDefense = true,
                    SourceName = effect.effectName
                };

                DamageResult result = DamageResolver.Resolve(request);

                //Debug.Log($"{unitName} took {result.FinalDamage} burn damage. Current HP: {currentHP}/{maxHP}");

                if (isDead)
                {
                    return;
                }
            }

            effect.TickDown();

            if (effect.IsExpired())
            {
                //Debug.Log($"{unitName} had {effect.effectName} expire.");
                activeEffects.RemoveAt(i);
            }
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        //Debug.Log($"{unitName} has died.");
    }
}