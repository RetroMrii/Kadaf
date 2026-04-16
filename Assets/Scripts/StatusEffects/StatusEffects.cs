using UnityEngine;

[System.Serializable]
public class StatusEffect
{
    public string effectName;
    public int damageBonus;
    public int defenseBonus;
    public int remainingTurns;

    public bool isBurn;
    public int burnDamagePerTurn;

    public StatusEffect(
        string effectName,
        int damageBonus,
        int defenseBonus,
        int remainingTurns,
        bool isBurn = false,
        int burnDamagePerTurn = 0)
    {
        this.effectName = effectName;
        this.damageBonus = damageBonus;
        this.defenseBonus = defenseBonus;
        this.remainingTurns = remainingTurns;
        this.isBurn = isBurn;
        this.burnDamagePerTurn = burnDamagePerTurn;
    }

    public void TickDown()
    {
        remainingTurns--;
    }

    public bool IsExpired()
    {
        return remainingTurns <= 0;
    }
}