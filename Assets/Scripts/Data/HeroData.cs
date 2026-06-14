using UnityEngine;

[CreateAssetMenu(fileName = "NewHero", menuName = "Game/Hero")]
public class HeroData : ScriptableObject
{
    [Header("Basic Info")]
    public string heroName;
    [TextArea] public string description;

    [Header("Core Stats")]
    public int maxHP;
    public int maxMana;
    public int maxStamina;

    public int ATK;
    public int MATK;
    public int DFS;
    public int MDFS;

    [Range(0, 100)] public int PRC;
    [Range(0, 100)] public int FRC;
    [Range(0, 100)] public int DOG;
    [Range(0, 100)] public int CRT;
    [Range(0, 100)] public int DCH;

    [Header("Starting Tech")]
    public ShopItemData starterTech;

    [Header("Visual States")]
    public Sprite normalSprite;
    public Sprite lowHPSprite;
    public Sprite deadSprite;

    [Header("Mana")]
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