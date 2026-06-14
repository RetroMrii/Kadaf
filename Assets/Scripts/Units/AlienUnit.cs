using UnityEngine;

public class AlienUnit : BattleUnit
{
    [Header("Alien Data")]
    public AlienData alienData;

    protected override void Awake()
    {
        base.Awake();
    }

    private void InitializeFromData()
    {
        unitName = alienData.alienName;
        maxHP = alienData.maxHP;
        attack = alienData.ATK;
        defense = alienData.DFS;

        fireResistancePercent = alienData.FRC;
        dodgeChancePercent = alienData.DOG;
        extraTurnChancePercent = alienData.DCH;

        currentHP = maxHP;
        isDead = false;
        activeEffects.Clear();

        SetupDefaultStatVisibility();
    }

    private void SetupDefaultStatVisibility()
    {
        RevealStat(StatType.HP);
        HideStat(StatType.ATK);
        HideStat(StatType.DFS);
        HideStat(StatType.FRC);
        HideStat(StatType.DOG);
        HideStat(StatType.DCH);
    }

    public void InitializeFromAssignedData()
    {
        if (alienData == null)
        {
            Debug.LogError("AlienUnit.InitializeFromAssignedData: alienData is null.");
            return;
        }

        unitName = alienData.alienName;
        maxHP = alienData.maxHP;
        attack = alienData.ATK;
        defense = alienData.DFS;

        fireResistancePercent = alienData.FRC;
        dodgeChancePercent = alienData.DOG;
        extraTurnChancePercent = alienData.DCH;

        currentHP = maxHP;
        isDead = false;

        activeEffects.Clear();
        SetupDefaultStatVisibility();
    }

    public void TakeTurn(HeroUnit targetHero, CombatManager combatManager)
    {
        if (isDead || targetHero == null || targetHero.IsDead)
            return;

        AlienActionData chosenAction = ChooseAction();
        if (chosenAction == null)
        {
            if (combatManager != null)
                combatManager.AddLog($"{unitName} has no valid actions.");
            return;
        }

        ExecuteAction(chosenAction, targetHero, combatManager);
    }

    private AlienActionData ChooseAction()
    {
        if (alienData == null || alienData.actions == null || alienData.actions.Length == 0)
            return null;

        int totalWeight = 0;

        for (int i = 0; i < alienData.actions.Length; i++)
        {
            AlienActionData action = alienData.actions[i];
            if (action != null)
                totalWeight += action.weight;
        }

        if (totalWeight <= 0)
            return null;

        int roll = Random.Range(0, totalWeight);
        int runningTotal = 0;

        for (int i = 0; i < alienData.actions.Length; i++)
        {
            AlienActionData action = alienData.actions[i];
            if (action == null)
                continue;

            runningTotal += action.weight;

            if (roll < runningTotal)
                return action;
        }

        return alienData.actions[0];
    }

    private void ExecuteAction(AlienActionData action, HeroUnit targetHero, CombatManager combatManager)
    {
        switch (action.actionType)
        {
            case AlienActionType.Damage:
                ExecuteDamageAction(action, targetHero, combatManager);
                break;

            case AlienActionType.BuffSelf:
                ExecuteBuffSelfAction(action, combatManager);
                break;

            case AlienActionType.DebuffHero:
                ExecuteDebuffHeroAction(action, targetHero, combatManager);
                break;

            default:
                if (combatManager != null)
                    combatManager.AddLog($"{unitName} tried to use an unsupported action.");
                break;
        }
    }

    private void ExecuteDamageAction(AlienActionData action, HeroUnit targetHero, CombatManager combatManager)
    {
        int rawDamage = GetAttackValue() + action.power;

        DamageRequest request = new DamageRequest
        {
            Attacker = this,
            Target = targetHero,
            BaseDamage = rawDamage,
            DamageType = DamageType.Physical,
            CanBeDodged = action.canBeDodged,
            IgnoresDefense = false,
            SourceName = action.actionName
        };

        DamageResult result = DamageResolver.Resolve(request);

        if (combatManager != null)
        {
            if (result.Dodged)
            {
                combatManager.AddLog(
                    $"{unitName} used {action.actionName} on {targetHero.UnitName}, but {targetHero.UnitName} dodged."
                );
            }
            else
            {
                combatManager.AddLog(
                    $"{unitName} used {action.actionName} on {targetHero.UnitName} for {result.FinalDamage} damage."
                );
            }
        }
    }

    private void ExecuteBuffSelfAction(AlienActionData action, CombatManager combatManager)
    {
        StatusEffect effect = new StatusEffect(
            action.actionName,
            action.damageBuffAmount,
            action.defenseBuffAmount,
            action.buffDuration
        );

        AddStatusEffect(effect);

        if (combatManager != null)
        {
            combatManager.AddLog(
                $"{unitName} used {action.actionName} and gained +{action.damageBuffAmount} ATK, +{action.defenseBuffAmount} DEF."
            );
        }
    }

    private void ExecuteDebuffHeroAction(AlienActionData action, HeroUnit targetHero, CombatManager combatManager)
    {
        StatusEffect effect = new StatusEffect(
            action.actionName,
            action.damageBuffAmount,
            action.defenseBuffAmount,
            action.buffDuration
        );

        targetHero.AddStatusEffect(effect);

        if (combatManager != null)
        {
            string atkText = action.damageBuffAmount >= 0
                ? $"+{action.damageBuffAmount}"
                : action.damageBuffAmount.ToString();

            string defText = action.defenseBuffAmount >= 0
                ? $"+{action.defenseBuffAmount}"
                : action.defenseBuffAmount.ToString();

            combatManager.AddLog(
                $"{unitName} used {action.actionName} on {targetHero.UnitName}, " +
                $"inflicting {atkText} ATK and {defText} DEF for {action.buffDuration} turns."
            );
        }
    }
}