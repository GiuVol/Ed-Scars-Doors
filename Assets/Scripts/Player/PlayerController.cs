using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IHealthable, IStatsable, IStatusable
{
    #region Movement Parameters

    /// <summary>
    /// The standard speed that the character should reach when walking.
    /// </summary>
    public float StandardWalkSpeed;

    /// <summary>
    /// The standard speed that the character should reach when running.
    /// </summary>
    public float StandardRunSpeed;

    /// <summary>
    /// The standard force with which the character should jump.
    /// </summary>
    public float StandardJumpForce;

    /// <summary>
    /// The standard force with which the character should dash.
    /// </summary>
    public float StandardDashForce;

    /// <summary>
    /// The standard gravity scale of the attached rigidbody.
    /// </summary>
    public float StandardGravityScale;

    /// <summary>
    /// The standard time that elapses between different series of jumps.
    /// </summary>
    public float StandardJumpInterval;

    /// <summary>
    /// The standard time that elapses between different dashes.
    /// </summary>
    public float StandardDashInterval;

    /// <summary>
    /// The standard time that elapses between different shots.
    /// </summary>
    public float StandardShootInterval;

    /// <summary>
    /// The standard number of consecutive jumps allowed.
    /// </summary>
    public int StandardNumberOfJumpsAllowed;

    /// <summary>
    /// The transform from which the projectiles must be instantiated.
    /// </summary>
    public Transform ProjectilesSpawnPoint;

    #endregion

    #region MovementValues

    /// <summary>
    /// The current speed that the character reaches when walking.
    /// </summary>
    public float CurrentWalkSpeed { get; set; }

    /// <summary>
    /// The current speed that the character reaches when running.
    /// </summary>
    public float CurrentRunSpeed { get; set; }

    /// <summary>
    /// The current force with which the character jumps.
    /// </summary>
    public float CurrentJumpForce { get; set; }

    /// <summary>
    /// The current force with which the character dashes.
    /// </summary>
    public float CurrentDashForce { get; set; }

    /// <summary>
    /// The current gravity scale of the attached rigidbody.
    /// </summary>
    public float CurrentGravityScale { get; set; }

    /// <summary>
    /// The current time that elapses between different series of jumps.
    /// </summary>
    public float CurrentJumpInterval { get; set; }

    /// <summary>
    /// The current time that elapses between different dashes.
    /// </summary>
    public float CurrentDashInterval { get; set; }

    /// <summary>
    /// The current time that elapses between different shots.
    /// </summary>
    public float CurrentShootInterval { get; set; }

    /// <summary>
    /// The current number of consecutive jumps allowed.
    /// </summary>
    public int CurrentNumberOfJumpsAllowed { get; set; }

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
            int actualNumberOfJumpsAllowed = Mathf.Max(CurrentNumberOfJumpsAllowed, 1);

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

    public List<GenericAbility> EquippedAbilities { get; private set; }

    #region Test

    private GenericAbility _testAbility1;
    private GenericAbility _testAbility2;
    private GenericAbility _testAbility3;
    private GenericAbility _testAbility4;

    #endregion

    void Start()
    {
        if (gameObject.GetComponent<MovementController2D>() == null)
        {
            gameObject.AddComponent<MovementController2D>();
        }

        if (gameObject.GetComponent<StatusComponent>() == null)
        {
            gameObject.AddComponent<StatusComponent>();
        }

        ResetMovementValues();
        
        MovementController = gameObject.GetComponent<MovementController2D>();

        Health = new HealthComponent(100, Die);

        Stats = new StatsComponent(100, 50, 500, 100, 50, 500);

        Status = gameObject.GetComponent<StatusComponent>();

        Status.Setup(50, 10, 2.5f, 0, 2, 15, 0);

        CanDash = true;
        CanShoot = true;

        ProjectileType = GameFormulas.NormalProjectileName;

        EquippedAbilities = new List<GenericAbility>();

        #region Test

        _testAbility1 = Resources.Load<MovementChangeAbility>("Abilities/DoubleJump");
        _testAbility2 = new ProjectileChangeAbility(GameFormulas.DarkProjectileName);
        _testAbility3 = new StatChangeAbility(1, 2);
        _testAbility4 = Resources.Load<MovementChangeAbility>("Abilities/ImprovedDoubleJump");

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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (EquippedAbilities.Contains(_testAbility1))
            {
                _testAbility1.Disable(this);
            } else
            {
                _testAbility1.Enable(this);
            }
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (EquippedAbilities.Contains(_testAbility2))
            {
                _testAbility2.Disable(this);
            }
            else
            {
                _testAbility2.Enable(this);
            }
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (EquippedAbilities.Contains(_testAbility3))
            {
                _testAbility3.Disable(this);
            }
            else
            {
                _testAbility3.Enable(this);
            }
        } else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (EquippedAbilities.Contains(_testAbility4))
            {
                _testAbility4.Disable(this);
            }
            else
            {
                _testAbility4.Enable(this);
            }
        }

        #endregion
    }

    void FixedUpdate()
    {
        float horizontalInput = InputHandler.HorizontalInput;
        float movementSpeed = Input.GetKey(KeyCode.B) ? CurrentRunSpeed : CurrentWalkSpeed;

        MovementController.HandleMovementWithSpeed(horizontalInput, movementSpeed);

        MovementController.GravityScale = CurrentGravityScale;

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
        MovementController.GiveImpulse(jumpDirection, CurrentJumpForce);

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
        _timeToWaitToJump = CurrentJumpInterval;
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

        MovementController.GiveImpulse(transform.right, CurrentDashForce);

        CanDash = false;

        yield return new WaitForSeconds(CurrentDashInterval);

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

        yield return new WaitForSeconds(CurrentShootInterval);

        CanShoot = true;
    }

    /// <summary>
    /// This procedure is called when the health of the player reaches 0.
    /// </summary>
    void Die()
    {
        Destroy(gameObject);
    }

    public void ResetMovementValues()
    {
        CurrentWalkSpeed = StandardWalkSpeed;
        CurrentRunSpeed = StandardRunSpeed;
        CurrentJumpForce = StandardJumpForce;
        CurrentDashForce = StandardDashForce;
        CurrentGravityScale = StandardGravityScale;
        CurrentJumpInterval = StandardJumpInterval;
        CurrentDashInterval = StandardDashInterval;
        CurrentShootInterval = StandardShootInterval;
        CurrentNumberOfJumpsAllowed = Mathf.Max(StandardNumberOfJumpsAllowed, 1);
    }
}
