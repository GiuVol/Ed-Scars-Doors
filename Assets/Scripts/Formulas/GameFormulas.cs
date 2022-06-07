using UnityEngine;

public class GameFormulas
{
    public const string ProjectileResourcesPath = "Projectiles/";
    public const string NormalProjectileName = "LightProjectile";
    public const string DarkProjectileName = "DarkProjectile";

    private const float MinDamageMultiplier = .1f;
    private const float MaxDamageMultiplier = 10;

    public static int Damage(float basePower, float attackerAttack, float targetDefence)
    {
        basePower = Mathf.Max(basePower, 0);
        attackerAttack = Mathf.Max(attackerAttack, 0);
        targetDefence = Mathf.Max(targetDefence, 0);

        float multiplier;

        if (attackerAttack < targetDefence)
        {
            multiplier = attackerAttack / (targetDefence + 1);
            multiplier = Mathf.Clamp(multiplier, MinDamageMultiplier, MaxDamageMultiplier);
        } else
        {
            multiplier = Mathf.Max(attackerAttack - targetDefence, 0) + 1;
            multiplier = Mathf.Log(multiplier, 3);
            multiplier = Mathf.Clamp(multiplier, MinDamageMultiplier, MaxDamageMultiplier);
        }

        float floatDamage = basePower * multiplier;
        int intDamage = Mathf.FloorToInt(floatDamage);

        return intDamage;
    }
}
