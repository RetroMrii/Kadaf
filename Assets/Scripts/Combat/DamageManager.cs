using UnityEngine;

public static class DamageResolver
{
    public static DamageResult Resolve(DamageRequest request)
    {
        DamageResult result = new DamageResult
        {
            Hit = true,
            Dodged = false,
            RawDamage = request.BaseDamage,
            MitigatedDamage = 0,
            FinalDamage = 0,
            DamageType = request.DamageType
        };

        if (request.Target == null)
            return result;

        if (request.CanBeDodged && RollDodge(request.Target))
        {
            result.Hit = false;
            result.Dodged = true;
            return result;
        }

        int damage = request.BaseDamage;

        if (!request.IgnoresDefense && request.DamageType != DamageType.True)
        {
            int defense = request.Target.GetDefenseValue();
            int damageBeforeDefense = damage;

            damage -= defense;
            damage = Mathf.Max(0, damage);

            result.MitigatedDamage = damageBeforeDefense - damage;
        }

        if (request.DamageType == DamageType.Fire)
        {
            int fireResPercent = request.Target.FireResistancePercent;
            damage = Mathf.RoundToInt(damage * (100 - fireResPercent) / 100f);
        }

        damage = Mathf.Max(0, damage);
        result.FinalDamage = damage;

        //Debug.Log(
    //$"[DamageResolver] Source={request.SourceName}, " +
    //$"Type={request.DamageType}, " +
    //$"Target={request.Target.UnitName}, " +
    //$"Raw={request.BaseDamage}, " +
    //$"Mitigated={result.MitigatedDamage}, " +
    //$"Final={damage}, " +
    //$"IgnoresDefense={request.IgnoresDefense}, " +
    //$"CanBeDodged={request.CanBeDodged}, " +
    //$"FireRes={request.Target.FireResistancePercent}, " +
    //$"Defense={request.Target.GetDefenseValue()}"
//);

        request.Target.ApplyFinalDamage(damage);

        return result;
    }

    private static bool RollDodge(BattleUnit target)
    {
        int dodgeChancePercent = target.DodgeChancePercent;
        return Random.value < (dodgeChancePercent / 100f);
    }
}