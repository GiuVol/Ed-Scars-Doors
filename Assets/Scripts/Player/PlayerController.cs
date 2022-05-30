using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour, IHealthable, IStatsable, IStatusable
{
    #region Parameters

    public float WalkForce;
    public float RunForce;
    public float JumpForce;
    public float DashForce;
    public float GravityScale;
    public Transform ProjectilesSpawnPoint;

    public float DashInterval;
    public float FireInterval;

    public int NumberOfJumpsAllowed;

    #endregion

    private MovementController2D MovementController { get; set; }

    public HealthComponent Health { get; private set; }

    public StatsComponent Stats { get; private set; }

    public StatusComponent Status { get; private set; }

    private bool CanDash { get; set; }
    private bool CanShoot { get; set; }

    private int CurrentNumberOfJumps { get; set; }

    private bool _isDying;

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

        Health = new HealthComponent(100);

        Stats = new StatsComponent(100, 200, 50, 100, 200, 50, 100, 200, 50);

        Status = gameObject.GetComponent<StatusComponent>();

        Status.Setup(50, 10, 2.5f, 0, 2, 15, 0);

        CanDash = true;
        CanShoot = true;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Z) && CanDash)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.Return) && CanShoot)
        {
            StartCoroutine(Shoot());
        }
    }

    void FixedUpdate()
    {
        if (Health.IsDead && !_isDying)
        {
            _isDying = true;
            Die();
            return;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float movementForce = Input.GetKey(KeyCode.B) ? RunForce : WalkForce;

        MovementController.HandleMovement(horizontalInput, movementForce);

        MovementController.GravityScale = GravityScale;

        if (MovementController.IsGrounded)
        {
            CurrentNumberOfJumps = 0;
        }
    }

    void IHealthable.Die()
    {

    }

    void Die()
    {
        Destroy(gameObject);
    }
    
    private void Jump()
    {
        if (!MovementController.IsGrounded && CurrentNumberOfJumps >= NumberOfJumpsAllowed)
        {
            return;
        }

        Vector3 jumpDirection = Vector3.up;
        MovementController.GiveImpulse(jumpDirection, JumpForce);

        CurrentNumberOfJumps++;
    }

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

    private IEnumerator Shoot()
    {
        if (!CanShoot)
        {
            yield break;
        }

        Instantiate(Resources.Load<Projectile>("FireballPrefab"),
                    ProjectilesSpawnPoint.position, ProjectilesSpawnPoint.rotation).Power *= Stats.Attack.CurrentValue;

        CanShoot = false;

        yield return new WaitForSeconds(FireInterval);

        CanShoot = true;
    }
}
