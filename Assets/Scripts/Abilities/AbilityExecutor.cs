using UnityEngine;

public class AbilityExecutor : MonoBehaviour
{
    public bool TryUseAbility(HeroUnit caster, AbilityData ability, BattleUnit target)
    {
        if (caster == null)
        {
            Debug.LogWarning("Caster is null.");
            return false;
        }

        if (ability == null)
        {
            Debug.LogWarning("Ability is null.");
            return false;
        }

        if (target == null)
        {
            Debug.LogWarning("Target is null.");
            return false;
        }

        if (caster.IsDead)
        {
            Debug.LogWarning($"{caster.UnitName} is dead and cannot act.");
            return false;
        }

        if (target.IsDead)
        {
            Debug.LogWarning($"{target.UnitName} is already dead.");
            return false;
        }

        if (!caster.CanUseAbility(ability))
        {
            Debug.LogWarning($"{caster.UnitName} does not have enough mana to use {ability.abilityName}.");
            return false;
        }

        switch (ability.abilityType)
        {
            case AbilityType.Damage:
                ExecuteDamageAbility(caster, ability, target);
                break;

            case AbilityType.Heal:
                ExecuteHealAbility(caster, ability, target);
                break;

            case AbilityType.Buff:
                ExecuteBuffAbility(caster, ability, target);
                break;

            default:
                Debug.LogWarning("Unhandled ability type.");
                return false;
        }

        return true;
    }

    private void ExecuteDamageAbility(HeroUnit caster, AbilityData ability, BattleUnit target)
    {
        DamageType damageType = ability.damageType;

        DamageRequest request = new DamageRequest
        {
            Attacker = caster,
            Target = target,
            BaseDamage = caster.GetAttackValue() + ability.basePower,
            DamageType = damageType,
            CanBeDodged = ability.canBeDodged,
            IgnoresDefense = false,
            SourceName = ability.abilityName
        };

        DamageResult result = DamageResolver.Resolve(request);

        if (result.Dodged)
        {
            //Debug.Log($"{target.UnitName} dodged {ability.abilityName} from {caster.UnitName}.");
            return;
        }

        //Debug.Log($"{caster.UnitName} used {ability.abilityName} on {target.UnitName} for {result.FinalDamage} damage.");

        if (ability.appliesBurn && ability.burnDamagePerTurn > 0 && ability.burnDuration > 0)
        {
            StatusEffect burnEffect = new StatusEffect(
                ability.abilityName + " (Burn)",
                0,
                0,
                ability.burnDuration,
                true,
                ability.burnDamagePerTurn
            );

            target.AddStatusEffect(burnEffect);

            //Debug.Log($"{target.UnitName} is burning for {ability.burnDamagePerTurn} damage over {ability.burnDuration} turns.");
        }
    }

    private void ExecuteHealAbility(HeroUnit caster, AbilityData ability, BattleUnit target)
    {
        int healAmount = ability.basePower;
        target.Heal(healAmount);

        //Debug.Log($"{caster.UnitName} used {ability.abilityName} on {target.UnitName}, healing {healAmount} HP.");
    }

    private void ExecuteBuffAbility(HeroUnit caster, AbilityData ability, BattleUnit target)
    {
        StatusEffect newEffect = new StatusEffect(
            ability.abilityName,
            ability.damageBuffAmount,
            ability.defenseBuffAmount,
            ability.buffDuration
        );

        target.AddStatusEffect(newEffect);

        //string effectWord = ability.isDebuff ? "inflicting" : "granting";

        //Debug.Log(
          //  $"{caster.UnitName} used {ability.abilityName} on {target.UnitName}, " +
          //  $"{effectWord} {ability.damageBuffAmount} ATK and {ability.defenseBuffAmount} DEF " +
          //  $"for {ability.buffDuration} turns."
        //);
    }
}