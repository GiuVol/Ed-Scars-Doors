public class ProjectileChangeAbility : GenericAbility
{
    private readonly string _projectileName;

    public ProjectileChangeAbility(string projectileName)
    {
        _projectileName = projectileName;
    }

    protected override void Setup(PlayerController playerController)
    {
        playerController.ProjectileType = _projectileName;
    }

    protected override void Takedown(PlayerController playerController)
    {
        playerController.ProjectileType = GameFormulas.NormalProjectileName;
    }
}
