using UnityEngine;

public enum ShopItemType
{
    Booster,
    Buff,
    Knowledge,
    Tech
}

public enum BoosterType
{
    None,
    Health,
    XP
}

public enum KnowledgeType
{
    None,
    XenoBiology,
    SlyHands
}

public enum TechRarity
{
    Common,
    Uncommon,
    Rare,
    Experimental
}

public enum ShopStatType
{
    None,

    ATK,
    MATK,
    DFS,
    MDFS,

    PRC,
    FRC,
    DOG,
    CRT,
    DCH,

    MaxHP,
    MaxMana,
    MaxStamina
}

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Game/Shop Item")]
public class ShopItemData : ScriptableObject
{
    [Header("Display")]
    public string itemName;

    [TextArea(2, 5)]
    public string description;

    [Header("Shop")]
    public ShopItemType itemType;
    public int price = 10;

    [Header("Booster")]
    public BoosterType boosterType = BoosterType.None;

    [Range(1, 4)]
    public int boosterStage = 1;

    [Range(0, 100)]
    public int healPercent = 0;

    public int xpAmount = 0;

    [Header("Buff")]
    public ShopStatType buffedStat = ShopStatType.None;
    public int buffAmount = 0;
    public int durationRooms = 1;

    [Header("Knowledge")]
    public KnowledgeType knowledgeType = KnowledgeType.None;

    [Range(0, 200)]
    public int rewardBonusPercent = 0;

    [Header("Tech")]
    public TechRarity techRarity = TechRarity.Common;
    public bool countsTowardTechLimit = true;
}