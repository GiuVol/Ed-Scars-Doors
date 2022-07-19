public class MovementChangeAbility : GenericAbility
{
    #region Movement Parameters

    /// <summary>
    /// The speed that the character should reach when walking.
    /// </summary>
    public float WalkSpeed;

    /// <summary>
    /// The speed that the character should reach when running.
    /// </summary>
    public float RunSpeed;

    /// <summary>
    /// The force with which the character should jump.
    /// </summary>
    public float JumpForce;

    /// <summary>
    /// The force with which the character should dash.
    /// </summary>
    public float DashForce;

    /// <summary>
    /// The gravity scale of the attached rigidbody.
    /// </summary>
    public float GravityScale;

    /// <summary>
    /// The time that elapses between different series of jumps.
    /// </summary>
    public float JumpInterval;

    /// <summary>
    /// The time that elapses between different dashes.
    /// </summary>
    public float DashInterval;

    /// <summary>
    /// The time that elapses between different shots.
    /// </summary>
    public float ShootInterval;

    /// <summary>
    /// The number of consecutive jumps allowed.
    /// </summary>
    public int NumberOfJumpsAllowedInAir;

    #endregion

    public override void Enable(PlayerController playerController)
    {
        if (playerController == null)
        {
            return;
        }

        playerController.CurrentWalkSpeed = (WalkSpeed > 0) ? WalkSpeed : playerController.StandardWalkSpeed;
        playerController.CurrentRunSpeed = (RunSpeed > 0) ? RunSpeed : playerController.StandardRunSpeed;
        playerController.CurrentJumpForce = (JumpForce > 0) ? JumpForce : playerController.StandardJumpForce;
        playerController.CurrentDashForce = (DashForce > 0) ? DashForce : playerController.StandardDashForce;
        playerController.CurrentGravityScale = (GravityScale > 0) ? GravityScale : playerController.StandardGravityScale;
        playerController.CurrentJumpInterval = (JumpInterval > 0) ? JumpInterval : playerController.StandardJumpInterval;
        playerController.CurrentDashInterval = (DashInterval > 0) ? DashInterval : playerController.StandardDashInterval;
        playerController.CurrentShootInterval = (ShootInterval > 0) ? ShootInterval : playerController.StandardShootInterval;
        playerController.CurrentNumberOfJumpsAllowedInAir = NumberOfJumpsAllowedInAir;
    }

    public override void Disable(PlayerController playerController)
    {
        if (playerController == null)
        {
            return;
        }
        
        playerController.ResetMovementValues();
    }
}
