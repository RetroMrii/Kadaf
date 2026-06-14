using UnityEngine;

public class HeroUnit : BattleUnit
{
    [Header("Hero Data")]
    public HeroData heroData;

    [Header("Mana")]
    public int currentMana;
    public int maxMana;
    public int manaPerTurn;

    private AbilityData[] abilities;

    protected override void Awake()
    {
        base.Awake();
    }

    public void InitializeHero()
    {
        if (heroData != null)
        {
            InitializeFromData();
        }
    }

    private void InitializeFromData()
    {
        unitName = heroData.heroName;
        maxHP = heroData.maxHP;
        attack = heroData.ATK;
        defense = heroData.DFS;

        maxMana = heroData.maxMana;
        manaPerTurn = heroData.manaPerTurn;
        currentMana = 0;

        fireResistancePercent = heroData.FRC;
        dodgeChancePercent = heroData.DOG;
        extraTurnChancePercent = heroData.DCH;

        abilities = heroData.abilities;

        currentHP = maxHP;
        isDead = false;
        activeEffects.Clear();

        SetupDefaultStatVisibility();
    }

    public void RestorePersistentState(int savedHP, int savedMana)
    {
        currentHP = Mathf.Clamp(savedHP, 0, maxHP);
        currentMana = Mathf.Clamp(savedMana, 0, maxMana);
        isDead = currentHP <= 0;

        Debug.Log($"Hero restored: HP={currentHP}/{maxHP}, Mana={currentMana}/{maxMana}");
    }

    private void SetupDefaultStatVisibility()
    {
        RevealStat(StatType.HP);
        HideStat(StatType.ATK);
        HideStat(StatType.DFS);
        RevealStat(StatType.Mana);
        HideStat(StatType.FRC);
        HideStat(StatType.DOG);
        HideStat(StatType.DCH);
    }

    public AbilityData[] GetAbilities()
    {
        return abilities;
    }

    public bool CanUseAbility(AbilityData ability)
    {
        return currentMana >= ability.manaCost;
    }

    public void UseMana(int amount)
    {
        currentMana -= amount;
        currentMana = Mathf.Max(0, currentMana);
    }

    public void StartTurn()
    {
        currentMana += manaPerTurn;
        currentMana = Mathf.Min(currentMana, maxMana);

        ProcessStatusEffects();

        Debug.Log($"{unitName} gained {manaPerTurn} mana. Current mana: {currentMana}/{maxMana}");
        Debug.Log($"{unitName} current effective stats: ATK {GetAttackValue()}, DEF {GetDefenseValue()}");
    }

    public bool HasPassive(HeroPassiveType passiveType)
    {
        return heroData != null && heroData.passive == passiveType;
    }
}