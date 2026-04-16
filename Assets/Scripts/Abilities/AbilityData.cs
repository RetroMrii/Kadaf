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

    [Header("Cost")]
    public int manaCost = 1;

    [Header("Damage Settings")]
    public DamageType damageType = DamageType.Physical;

    [Header("Ability Rules")]
    public AbilityType abilityType;
    public TargetType targetType;
    public int basePower = 0;
    public bool canBeDodged = true;

    [Header("Buff Settings")]
    public int damageBuffAmount = 0;
    public int defenseBuffAmount = 0;
    public int buffDuration = 0;
    public bool isDebuff = false;

    [Header("Burn Settings")]
    public bool appliesBurn = false;
    public int burnDamagePerTurn = 0;
    public int burnDuration = 0;
}