using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IHealthable, IStatsable, IStatusable
{
    public const int MaxNumberOfEquippableAbilities = 4;

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
    /// The standard number of jumps allowed in the air.
    /// </summary>
    public int StandardNumberOfJumpsAllowedInAir;

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
    /// The current number of jumps allowed in the air.
    /// </summary>
    public int CurrentNumberOfJumpsAllowedInAir { get; set; }

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
    /// Stores the number of jumps executed in the air.
    /// </summary>
    public int CurrentNumberOfJumpsInTheAir { get; set; }

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
            return (MovementController.IsGrounded || CurrentNumberOfJumpsInTheAir < CurrentNumberOfJumpsAllowedInAir) && 
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

    /// <summary>
    /// The name of the projectile that the player will shoot. 
    /// It must match the name of the projectile prefab.
    /// </summary>
    public string ProjectileType { get; set; }

    /// <summary>
    /// Structure that stores the abilities currently equipped by the player.
    /// </summary>
    private List<GenericAbility> EquippedAbilities { get; set; }

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
        Status.Setup(100, 5, 5, 0, 1, 20, 0);

        CanDash = true;
        CanShoot = true;

        ProjectileType = GameFormulas.NormalProjectileName;

        EquippedAbilities = new List<GenericAbility>(MaxNumberOfEquippableAbilities);

        #region Test

        _testAbility1 = Resources.Load<GenericAbility>("Abilities/DoubleJump");
        _testAbility2 = Resources.Load<GenericAbility>("Abilities/AttackOverDefence");
        _testAbility3 = Resources.Load<GenericAbility>("Abilities/SwarmShooter");
        _testAbility4 = Resources.Load<GenericAbility>("Abilities/ImprovedDoubleJump");

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
            if (!IsEquipped(_testAbility1))
            {
                EquipAbility(_testAbility1);
            } else
            {
                UnequipAbility(_testAbility1);
            }
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!IsEquipped(_testAbility2))
            {
                EquipAbility(_testAbility2);
            }
            else
            {
                UnequipAbility(_testAbility2);
            }

            Debug.Log(Stats.Attack.CurrentValue + " " + Stats.Defence.CurrentValue);

        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (!IsEquipped(_testAbility3))
            {
                EquipAbility(_testAbility3);
            }
            else
            {
                UnequipAbility(_testAbility3);
            }
        } else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (!IsEquipped(_testAbility4))
            {
                EquipAbility(_testAbility4);
            }
            else
            {
                UnequipAbility(_testAbility4);
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
    /// This method initializes all the values related to movement.
    /// </summary>
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
        CurrentNumberOfJumpsAllowedInAir = StandardNumberOfJumpsAllowedInAir;
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

        bool isGrounded = MovementController.IsGrounded;

        if (_jumpHandlingTask == null)
        {
            _jumpHandlingTask = StartCoroutine(HandleJump());
        }

        Vector3 jumpDirection = Vector3.up;
        MovementController.GiveImpulse(jumpDirection, CurrentJumpForce);

        if (!isGrounded)
        {
            CurrentNumberOfJumpsInTheAir++;
        }
    }

    /// <summary>
    /// <c>IEnumerator</c> used to handle the jump process.
    /// </summary>
    private IEnumerator HandleJump()
    {
        yield return new WaitUntil(() => !MovementController.IsGrounded);
        
        yield return new WaitUntil(() => MovementController.IsGrounded);

        CurrentNumberOfJumpsInTheAir = 0;
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

        Projectile projectile = Projectile.InstantiateProjectile(ProjectileType, ProjectilesSpawnPoint);

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

    /// <summary>
    /// This method enables the ability given in input on the player, if it is possible to equip it.
    /// </summary>
    /// <param name="newAbility">The ability you want to equip</param>
    public void EquipAbility(GenericAbility newAbility)
    {
        if (EquippedAbilities.Count >= MaxNumberOfEquippableAbilities)
        {
            Debug.Log("You have already reached the max number of equipped abilities!");
            return;
        }

        foreach (GenericAbility equippedAbility in EquippedAbilities)
        {
            if (equippedAbility.GetType().IsEquivalentTo(newAbility.GetType()))
            {
                Debug.Log("An ability of this type is already equipped!");
                return;
            }
        }

        EquippedAbilities.Add(newAbility);
        newAbility.Enable(this);
    }

    /// <summary>
    /// This method disables the ability given in input, only if it is already equipped.
    /// </summary>
    /// <param name="ability">The ability you want to unequip</param>
    public void UnequipAbility(GenericAbility ability)
    {
        if (!IsEquipped(ability))
        {
            return;
        }

        EquippedAbilities.Remove(ability);
        ability.Disable(this);
    }

    /// <summary>
    /// Returns whether the ability given in input is equipped or not.
    /// </summary>
    /// <param name="ability">The ability you want to check</param>
    /// <returns></returns>
    public bool IsEquipped(GenericAbility ability)
    {
        return EquippedAbilities.Contains(ability);
    }
}
