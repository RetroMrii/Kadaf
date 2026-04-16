using UnityEngine;
using UnityEngine.InputSystem;

public class CombatTester : MonoBehaviour
{
    [Header("References")]
    public CombatManager combatManager;
    public AbilityExecutor abilityExecutor;

    private void Update()
    {
        if (combatManager == null || abilityExecutor == null)
            return;

        if (combatManager.battleEnded)
            return;

        if (!combatManager.playerTurn)
            return;

        HeroUnit activeHero = combatManager.GetActiveHero();
        if (activeHero == null || activeHero.IsDead)
            return;

        AbilityData[] abilities = activeHero.GetAbilities();
        if (abilities == null || abilities.Length == 0)
            return;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            HandleAbilityInput(activeHero, abilities, 0);
        }

        if (keyboard.digit2Key.wasPressedThisFrame)
        {
            HandleAbilityInput(activeHero, abilities, 1);
        }

        if (keyboard.digit3Key.wasPressedThisFrame)
        {
            HandleAbilityInput(activeHero, abilities, 2);
        }

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Player ended turn manually.");
            combatManager.EndPlayerTurn();
        }
    }

    private void HandleAbilityInput(HeroUnit activeHero, AbilityData[] abilities, int index)
    {
        if (index < 0 || index >= abilities.Length)
        {
            Debug.LogWarning($"Ability index {index} is out of range.");
            return;
        }

        AbilityData selectedAbility = abilities[index];
        if (selectedAbility == null)
        {
            Debug.LogWarning($"Ability at index {index} is null.");
            return;
        }

        BattleUnit target = GetTargetForAbility(selectedAbility, activeHero);
        if (target == null)
        {
            Debug.LogWarning(
                $"No valid target found for {selectedAbility.abilityName}. " +
                $"TargetType = {selectedAbility.targetType}"
            );
            return;
        }

        bool success = abilityExecutor.TryUseAbility(activeHero, selectedAbility, target);

        if (!success)
            return;

        activeHero.UseMana(selectedAbility.manaCost);

        Debug.Log($"{activeHero.UnitName} used {selectedAbility.abilityName}. Ending player turn.");
        combatManager.CheckBattleEnd();

        if (!combatManager.battleEnded)
        {
            combatManager.EndPlayerTurn();
        }
    }

    private BattleUnit GetTargetForAbility(AbilityData ability, HeroUnit caster)
    {
        switch (ability.targetType)
        {
            case TargetType.Self:
                return caster;

            case TargetType.SingleEnemy:
                return combatManager.GetFirstAliveAlien();

            case TargetType.SingleAlly:
                return combatManager.GetFirstAliveHero();

            default:
                Debug.LogWarning($"Target type {ability.targetType} is not implemented yet.");
                return null;
        }
    }
}