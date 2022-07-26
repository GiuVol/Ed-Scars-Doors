using UnityEngine;

public class GameFormulas
{
    private const float MinDamageMultiplier = .1f;
    private const float MaxDamageMultiplier = 10;

    private const float SecondsToDoublePowerWithCharge = 5;

    public const string ObstacleLayerName = "Obstacle";

    public static int Damage(float power, float attackerAttack, float targetDefence)
    {
        power = Mathf.Max(power, 0);
        attackerAttack = Mathf.Max(attackerAttack, 0);
        targetDefence = Mathf.Max(targetDefence, 0);

        float multiplier;

        if (attackerAttack < targetDefence)
        {
            multiplier = attackerAttack / (targetDefence + 1);
            multiplier = Mathf.Clamp(multiplier, MinDamageMultiplier, MaxDamageMultiplier);
        } else
        {
            int desiredBase = 3;
            multiplier = Mathf.Max(attackerAttack - targetDefence, 0) + desiredBase;
            multiplier = Mathf.Log(multiplier, desiredBase);
            multiplier = Mathf.Clamp(multiplier, MinDamageMultiplier, MaxDamageMultiplier);
        }

        float floatDamage = power * multiplier;
        int intDamage = Mathf.FloorToInt(floatDamage);

        return intDamage;
    }

    public static float ChargedAttackPower(float basePower, float chargeTime)
    {
        float multiplier = chargeTime / SecondsToDoublePowerWithCharge;

        float power = basePower * (1 + multiplier);

        return power;
    }

    public static float ChargedProjectileSize(float chargeTime, float maxChargeTime, 
                                              float minScale, float maxScale)
    {
        float desiredScale = 1;

        if (chargeTime >= .25f && maxChargeTime >= .25f)
        {
            float x = (chargeTime / maxChargeTime) * (maxScale - 1);
            //float y = (0.533f * Mathf.Pow(x, 3)) - (2 * Mathf.Pow(x, 2)) + (2.47f * x);
            desiredScale = Mathf.Clamp(1 + x, minScale, maxScale);
        }

        return desiredScale;
    }
}
