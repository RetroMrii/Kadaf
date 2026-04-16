using System.Collections.Generic;

[System.Serializable]
public class UnitKnowledgeProfile
{
    private readonly Dictionary<StatType, StatVisibilityLevel> statVisibility =
        new Dictionary<StatType, StatVisibilityLevel>();

    public void SetVisibility(StatType statType, StatVisibilityLevel visibility)
    {
        statVisibility[statType] = visibility;
    }

    public bool IsRevealed(StatType statType)
    {
        return statVisibility.TryGetValue(statType, out StatVisibilityLevel visibility) &&
               visibility == StatVisibilityLevel.Revealed;
    }
}