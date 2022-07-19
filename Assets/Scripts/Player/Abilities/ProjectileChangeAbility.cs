public class ProjectileChangeAbility : GenericAbility
{
    /// <summary>
    /// The name of the projectile that this ability allows to shoot.
    /// </summary>
    public string ProjectileName;

    public override void Enable(PlayerController playerController)
    {
        if (playerController == null)
        {
            return;
        }
        
        playerController.ProjectileType = ProjectileName;
    }

    public override void Disable(PlayerController playerController)
    {
        if (playerController == null)
        {
            return;
        }
        
        playerController.ProjectileType = GameFormulas.NormalProjectileName;
    }
}
