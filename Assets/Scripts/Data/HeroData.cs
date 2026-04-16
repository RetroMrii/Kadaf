using UnityEngine;

[CreateAssetMenu(fileName = "NewHero", menuName = "Game/Hero")]
public class HeroData : ScriptableObject
{
    [Header("Basic Info")]
    public string heroName;
    [TextArea] public string description;

    [Header("Base Stats")]
    public int maxHP = 100;
    public int BonusDMG = 10;
    public int defense = 5;

    [Header("Mana")]
    public int maxMana = 3;
    public int manaPerTurn = 1;

    [Header("Advanced Stats")]
    public int fireResistancePercent = 0;
    public int dodgeChancePercent = 0;
    public int extraTurnChancePercent = 0;

    [Header("Passive")]
    public HeroPassiveType passive = HeroPassiveType.None;

    [Header("Abilities")]
    public AbilityData[] abilities;
}