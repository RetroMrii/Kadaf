using UnityEngine;

[CreateAssetMenu(fileName = "NewAlien", menuName = "Game/Alien")]
public class AlienData : ScriptableObject
{
    [Header("Basic Info")]
    public string alienName;
    [TextArea] public string description;

    [Header("Base Stats")]
    public int maxHP = 50;
    public int attack = 8;
    public int defense = 2;

    [Header("Advanced Stats")]
    public int fireResistancePercent = 0;
    public int dodgeChancePercent = 0;
    public int extraTurnChancePercent = 0;

    [Header("Alien Actions")]
    public AlienActionData[] actions;
}