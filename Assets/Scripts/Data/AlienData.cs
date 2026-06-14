using UnityEngine;

[CreateAssetMenu(fileName = "NewAlien", menuName = "Game/Alien")]
public class AlienData : ScriptableObject
{
    [Header("Basic Info")]
    public string alienName;
    [TextArea] public string description;

    [Header("Core Stats")]
    public int maxHP;

    public int ATK;
    public int MATK;
    public int DFS;
    public int MDFS;

    [Range(0, 100)] public int PRC;
    [Range(0, 100)] public int FRC;

    public int INTL;

    [Header("Rewards")]
    public int xpReward = 0;
    public int goldReward = 0;

    [Header("Visual States")]
    public Sprite normalSprite;
    public Sprite lowHPSprite;

    [Header("Alien Actions")]
    public AlienActionData[] actions;
}