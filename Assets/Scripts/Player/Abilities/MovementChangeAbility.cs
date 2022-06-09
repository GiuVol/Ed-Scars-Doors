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
    public int NumberOfJumpsAllowed;

    #endregion

    public override void Enable(PlayerController playerController)
    {
        playerController.CurrentWalkSpeed = WalkSpeed;
        playerController.CurrentRunSpeed = RunSpeed;
        playerController.CurrentJumpForce = JumpForce;
        playerController.CurrentDashForce = DashForce;
        playerController.CurrentGravityScale = GravityScale;
        playerController.CurrentJumpInterval = JumpInterval;
        playerController.CurrentDashInterval = DashInterval;
        playerController.CurrentShootInterval = ShootInterval;
        playerController.CurrentNumberOfJumpsAllowed = NumberOfJumpsAllowed;

        if (playerController.CurrentNumberOfJumpsAllowed < 1)
        {
            playerController.CurrentNumberOfJumpsAllowed = 1;
        }
    }

    public override void Disable(PlayerController playerController)
    {
        playerController.ResetMovementValues();
    }
}
