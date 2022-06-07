using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IHealthable, IStatsable, IStatusable
{
    #region Parameters

    /// <summary>
    /// The speed that the character should reach when it's walking.
    /// </summary>
    public float WalkSpeed;

    /// <summary>
    /// The speed that the character should reach when it's running.
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
    /// The transform from which the projectiles must be instantiated.
    /// </summary>
    public Transform ProjectilesSpawnPoint;

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

    /// <summary>
    /// The <c>MovementController2D</c> that moves the player.
    /// </summary>
    private MovementController2D MovementController { get; set; }

    /// <summary>
    /// The <c>HealthComponent</c> that stores values and methods related to the health of the player.
    /// </summary>
    public HealthComponent Health { get; private set; }

    /// <summary>
    /// The <c>StatsComponent</c> that stores values and methods related to the stats of the player.
    /// </summary>
    public StatsComponent Stats { get; private set; }

    /// <summary>
    /// The <c>StatusComponent</c> that stores values and methods related to the status of the player.
    /// </summary>
    public StatusComponent Status { get; private set; }

    /// <summary>
    /// Stores the number of jumps executed consecutively without touching the ground.
    /// </summary>
    public int CurrentNumberOfJumps { get; set; }

    /// <summary>
    /// Stores the coroutine that handles the jump process.
    /// </summary>
    private Coroutine _jumpHandlingTask;

    /// <summary>
    /// Stores the time to wait to jump again.
    /// </summary>
    private float _timeToWaitToJump;
    
    /// <summary>
    /// Returns whether the player can jump or not.
    /// </summary>
    private bool CanJump
    {
        get
        {
            int actualNumberOfJumpsAllowed = Mathf.Max(NumberOfJumpsAllowed, 1);

            return (MovementController.IsGrounded || CurrentNumberOfJumps < actualNumberOfJumpsAllowed) && 
                _timeToWaitToJump <= 0;
        }
    }

    /// <summary>
    /// Represents whether the player can dash or not.
    /// </summary>
    private bool CanDash { get; set; }

    /// <summary>
    /// Represents whether the player can shoot or not.
    /// </summary>
    private bool CanShoot { get; set; }

    public string ProjectileType { get; set; }

    #region Test

    private ProjectileChangeAbility _testAbility;

    #endregion

    void Start()
    {
        NumberOfJumpsAllowed = Mathf.Max(NumberOfJumpsAllowed, 1);
        
        if (gameObject.GetComponent<MovementController2D>() == null)
        {
            gameObject.AddComponent<MovementController2D>();
        }

        if (gameObject.GetComponent<StatusComponent>() == null)
        {
            gameObject.AddComponent<StatusComponent>();
        }

        MovementController = gameObject.GetComponent<MovementController2D>();

        Health = new HealthComponent(100, Die);

        Stats = new StatsComponent(100, 50, 250, 100, 50, 250);

        Status = gameObject.GetComponent<StatusComponent>();

        Status.Setup(50, 10, 2.5f, 0, 2, 15, 0);

        CanDash = true;
        CanShoot = true;

        ProjectileType = GameFormulas.NormalProjectileName;

        #region Test

        _testAbility = new ProjectileChangeAbility(GameFormulas.DarkProjectileName);

        #endregion
    }

    void Update()
    {
        if (InputHandler.Jump("Down"))
        {
            Jump();
        }

        if (InputHandler.Dash("Down") && CanDash)
        {
            StartCoroutine(Dash());
        }

        if (InputHandler.Shoot("Down") && CanShoot)
        {
            StartCoroutine(Shoot());
        }

        #region Test

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (_testAbility.IsEnabled)
            {
                _testAbility.Disable(this);
            } else
            {
                _testAbility.Enable(this);
            }
        }

        #endregion
    }

    void FixedUpdate()
    {
        float horizontalInput = InputHandler.HorizontalInput;
        float movementSpeed = Input.GetKey(KeyCode.B) ? RunSpeed : WalkSpeed;

        MovementController.HandleMovementWithSpeed(horizontalInput, movementSpeed);

        MovementController.GravityScale = GravityScale;

        _timeToWaitToJump = Mathf.Max(_timeToWaitToJump - Time.fixedDeltaTime, 0);
    }

    /// <summary>
    /// Allows the player to jump, if it is possible.
    /// </summary>
    private void Jump()
    {
        if (!CanJump)
        {
            return;
        }

        if (_jumpHandlingTask == null)
        {
            _jumpHandlingTask = StartCoroutine(HandleJump());
        }

        Vector3 jumpDirection = Vector3.up;
        MovementController.GiveImpulse(jumpDirection, JumpForce);

        CurrentNumberOfJumps++;
    }

    /// <summary>
    /// <c>IEnumerator</c> used to handle the jump process.
    /// </summary>
    private IEnumerator HandleJump()
    {
        yield return new WaitUntil(() => !MovementController.IsGrounded);
        
        yield return new WaitUntil(() => MovementController.IsGrounded);

        CurrentNumberOfJumps = 0;
        _timeToWaitToJump = JumpInterval;
        _jumpHandlingTask = null;
    }

    /// <summary>
    /// Allows the player to dash, if it is possible.
    /// </summary>
    private IEnumerator Dash()
    {
        if (!CanDash)
        {
            yield break;
        }

        MovementController.GiveImpulse(transform.right, DashForce);

        CanDash = false;

        yield return new WaitForSeconds(DashInterval);

        CanDash = true;
    }

    /// <summary>
    /// Allows the player to shoot, if it is possible.
    /// </summary>
    private IEnumerator Shoot()
    {
        if (!CanShoot)
        {
            yield break;
        }

        Projectile projectile = Instantiate(Resources.Load<Projectile>(GameFormulas.ProjectileResourcesPath + ProjectileType), 
                                            ProjectilesSpawnPoint.position, ProjectilesSpawnPoint.rotation);

        projectile.AttackerAttack = Stats.Attack.CurrentValue;

        CanShoot = false;

        yield return new WaitForSeconds(ShootInterval);

        CanShoot = true;
    }

    /// <summary>
    /// This procedure is called when the health of the player reaches 0.
    /// </summary>
    void Die()
    {
        Destroy(gameObject);
    }
}
