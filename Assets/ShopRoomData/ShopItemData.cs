using UnityEngine;

public enum ShopItemType
{
    Heal,
    RoundBuff,
    ActivatedBuff,
    Revive
}

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Game/Shop Item")]
public class ShopItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    [TextArea] public string description;
    public ShopItemType itemType;
    public int price = 10;

    [Header("Heal")]
    public int healPercent = 0;

    [Header("Buff")]
    public int bonusDMGAmount = 0;
    public int defenseAmount = 0;
    public int durationRounds = 0;

    [Header("Activated Buff")]
    public bool isActivatedBuff = false;

    [Header("Revive")]
    public bool revivesToOneHP = false;
}