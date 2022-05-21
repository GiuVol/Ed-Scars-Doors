using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MovementController2D : MonoBehaviour
{
    #region Debug

    public float MovementForce;
    public float JumpForce;
    public float DashForce;
    public float DesiredGravityScale;
    public bool Grounded;
    public LayerMask ToCast;

    #endregion

    public Rigidbody2D AttachedRigidbody { get; private set; }
    public float GravityScale
    {
        get
        {
            return AttachedRigidbody.gravityScale;
        }
        set
        {
            AttachedRigidbody.gravityScale = value;
        }
    }
    public bool IsGrounded
    {
        get
        {
            LayerMask toCast = ~(1 << gameObject.layer);

            #region Debug

            ToCast = toCast;

            #endregion

            Vector3 positionOffset = Vector3.up * .1f;
            Vector3 offsettedPosition = transform.position + positionOffset;

            float range = 1.15f;

            RaycastHit2D hit =
                Physics2D.Raycast(offsettedPosition, Vector3.down, range, toCast);

            return hit;
        }
    }

    private PhysicsMaterial2D _zeroFriction;
    private PhysicsMaterial2D _maxFriction;

    void Start()
    {
        if (!gameObject.GetComponent<Rigidbody2D>())
        {
            gameObject.AddComponent<Rigidbody2D>();
        }

        AttachedRigidbody = gameObject.GetComponent<Rigidbody2D>();
        AttachedRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        _zeroFriction = new PhysicsMaterial2D();
        _zeroFriction.friction = 0;

        _maxFriction = new PhysicsMaterial2D();
        _maxFriction.friction = 1;
    }

    #region Debug

    void Update()
    {
        GravityScale = DesiredGravityScale;

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            GiveImpulse(transform.up, JumpForce);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            GiveImpulse(transform.right, DashForce);
        }
        
        Grounded = IsGrounded;
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        HandleMovement(h, MovementForce);
    }

    #endregion

    public void HandleMovement(float horizontalInput, float movementForce)
    {
        AttachedRigidbody.drag = 4;

        if (Mathf.Abs(horizontalInput) > 0)
        {
            AttachedRigidbody.sharedMaterial = _zeroFriction;

            Rotate(horizontalInput);

            movementForce = Mathf.Abs(horizontalInput) * movementForce;
            Move(transform.right, movementForce);
        } else
        {
            AttachedRigidbody.sharedMaterial = _maxFriction;
        }
    }

    private void Rotate(float horizontalInput)
    {
        float yRotation;

        if (horizontalInput > 0)
        {
            yRotation = 0;
        } else if (horizontalInput < 0)
        {
            yRotation = 180;
        } else
        {
            yRotation = transform.eulerAngles.y;
        }

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, 
                                              yRotation, 
                                              transform.eulerAngles.z);
    }
    
    private void Move(Vector3 direction, float movementForce)
    {
        Vector3 forceToApply = direction.normalized * movementForce;
        AttachedRigidbody.AddForce(forceToApply * AttachedRigidbody.mass);
    }

    public void GiveImpulse(Vector2 direction, float force)
    {
        Vector3 forceToApply = direction.normalized * force;
        AttachedRigidbody.AddForce(forceToApply * AttachedRigidbody.mass, 
                                   ForceMode2D.Impulse);
    }
}
