using UnityEngine;

public class GameFormulas
{
    private const float MinDecreaseMultiplier = .1f;
    private const float MaxDecreaseMultiplier = 10;

    private const float MaxIncrease = .1f;
    private const float MinIncrease = 10;

    private const int Exponent = 1;

    private const float SecondsToDoublePowerWithCharge = 5;

    public const string TerrainLayerName = "Terrain";
    public const string ObstacleLayerName = "Obstacle";
    public const string HidingPlaceLayerName = "HidingPlace";
    public const string ItemLayerName = "Item";
    public const string CollidesWithGroundOnlyLayerName = "CollidesWithGroundOnly";
    public const string DoorLayerName = "Door";
    public const string ScreenBoundriesLayerName = "ScreenBoundries";

    public static int Damage(float power, float attackerAttack, float targetDefence)
    {
        power = Mathf.Max(power, 0);
        attackerAttack = Mathf.Max(attackerAttack, 0);
        targetDefence = Mathf.Max(targetDefence, 0);
        float difference = attackerAttack - targetDefence;

        float floatDamage;

        if (difference < 0)
        {
            float decreaseMultiplier;
            decreaseMultiplier = attackerAttack / (targetDefence + 1);

            decreaseMultiplier = Mathf.Clamp(decreaseMultiplier, MinDecreaseMultiplier, MaxDecreaseMultiplier);
            floatDamage = power * decreaseMultiplier;
        }
        else if (difference == 0)
        {
            floatDamage = power;
        }
        else
        {
            int desiredBase = 3;
            float increase = Mathf.Max(difference, 0) + desiredBase;
            increase = Mathf.Log(increase, desiredBase);
            increase = Mathf.Clamp(increase, MinIncrease, MaxIncrease);
            floatDamage = power + Mathf.Pow(increase, Exponent);
        }

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
