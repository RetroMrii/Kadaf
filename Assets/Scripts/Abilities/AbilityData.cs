using UnityEngine;

public enum AbilityType
{
    Damage,
    Buff,
    Heal
}

public enum TargetType
{
    Self,
    SingleEnemy,
    SingleAlly,
    AllEnemies,
    AllAllies
}

[CreateAssetMenu(fileName = "NewAbility", menuName = "Game/Ability")]
public class AbilityData : ScriptableObject
{
    [Header("Basic Info")]
    public string abilityName;
    [TextArea] public string description;

    [Header("Ability Rules")]
    public AbilityType abilityType;
    public TargetType targetType;
    public int basePower = 0;

    [Header("Buff Settings")]
    public int damageBuffAmount = 0;
    public int defenseBuffAmount = 0;
    public int buffDuration = 0;
    public bool isDebuff = false;

    [Header("Burn Settings")]
    public bool appliesBurn = false;
    public int burnDamagePerTurn = 0;
    public int burnDuration = 0;

    [Header("Resource Costs")]
    public int staminaCost = 0;
    public int manaCost = 0;

    [Header("Damage Typing")]
    public DamageType damageType = DamageType.Physical;
    public bool usesMagicScaling = false;
    public bool canCrit = true;
    public bool canBeDodged = true;

    [Header("Elemental Effects")]
    public int fireStacks = 0;
    public int poisonStacks = 0;
    public int frostStacks = 0;
    public int electricStacks = 0;

    [Header("Lifesteal")]
    public bool hasLifeSteal = false;
    [Range(0, 100)] public int lifeStealPercent = 0;
}