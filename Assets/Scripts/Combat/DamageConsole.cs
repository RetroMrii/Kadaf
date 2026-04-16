using UnityEngine;

public enum DamageType
{
    Physical,
    Fire,
    True
}

public struct DamageRequest
{
    public BattleUnit Attacker;
    public BattleUnit Target;

    public int BaseDamage;
    public DamageType DamageType;

    public bool CanBeDodged;
    public bool IgnoresDefense;

    public string SourceName;
}

public struct DamageResult
{
    public bool Hit;
    public bool Dodged;

    public int RawDamage;
    public int MitigatedDamage;
    public int FinalDamage;

    public DamageType DamageType;
}