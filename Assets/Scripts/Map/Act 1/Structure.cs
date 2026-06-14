using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Combat,
    Heal,
    Shop,
    Recruit,
    Random,
    Boss
}

public enum EnemyType
{
    Grunt,
    Assassin,
    Drone,
    Heavy,
    Warlock
}

[System.Serializable]
public class RoomData
{
    public RoomType roomType;
    public EnemyType[] enemies;
    public string flavorText;
}

[System.Serializable]
public class RoomOfferSet
{
    public List<RoomData> options = new List<RoomData>();
}