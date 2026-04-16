using UnityEngine;

public enum AlienActionType
{
    Damage,
    BuffSelf,
    DebuffHero
}

[CreateAssetMenu(fileName = "NewAlienAction", menuName = "Game/Alien Action")]
public class AlienActionData : ScriptableObject
{
    [Header("Basic Info")]
    public string actionName;
    [TextArea] public string description;

    [Header("Action Rules")]
    public AlienActionType actionType = AlienActionType.Damage;
    public int power = 0;
    public bool canBeDodged = true;

    [Header("Effect Settings")]
    public int damageBuffAmount = 0;
    public int defenseBuffAmount = 0;
    public int buffDuration = 0;

    [Header("AI Weight")]
    [Min(1)] public int weight = 1;
}