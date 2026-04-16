using UnityEngine;

[System.Serializable]
public class RunHeroState
{
    public HeroData heroData;
    public int currentHP;
    public int currentMana;
    public int maxHP;
    public bool isAvailable = true;
    public bool hasJoinedRun = true;
}

public static class CombatState
{
    public static EnemyType[] currentEnemies;
}