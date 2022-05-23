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

    /// <summary>
    /// The <c>Rigidbody2D</c> component attached to the gameObject. 
    /// It handles the movement of the gameObject according to several parameters that can be fine tuned. 
    /// It's created (if it doesn't exist) and initialized on <c>Start()</c>.
    /// </summary>
    public Rigidbody2D AttachedRigidbody { get; private set; }

    /// <summary>
    /// A property that provides access to the <c>gravityScale</c> of the <c>AttachedRigidbody</c>, according to the needs.
    /// </summary>
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

    /// <summary>
    /// A property that provides access to the <c>drag</c> of the <c>AttachedRigidbody</c>, according to the needs.
    /// </summary>
    public float Drag
    {
        get
        {
            return AttachedRigidbody.drag;
        }
        set
        {
            AttachedRigidbody.drag = value;
        }
    }

    /// <summary>
    /// A boolean property (get only) that returns whether the character has something beneath him or not.
    /// </summary>
    public bool IsGrounded
    {
        get
        {
            LayerMask toCast = ~(1 << gameObject.layer);

            Vector3 positionOffset = Vector3.up * .1f;
            Vector3 offsettedPosition = transform.position + positionOffset;

            float range = 1.15f;

            RaycastHit2D hit =
                Physics2D.Raycast(offsettedPosition, Vector3.down, range, toCast);

            return hit;
        }
    }

    /// <summary>
    /// <c>PhysicsMaterial2D</c> used by the <c>AttachedRigidbody</c> when the character is trying to move. 
    /// It has low friction, to handle smooth movement.
    /// </summary>
    private PhysicsMaterial2D _zeroFriction;

    /// <summary>
    /// <c>PhysicsMaterial2D</c> used by the <c>AttachedRigidbody</c> when the character is trying to stop. 
    /// It has high friction, to stop the movement quickly.
    /// </summary>
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

    /// <summary>
    /// Method that rotates and moves the character, according to <c>horizontalInput</c> and <c>drivingForce</c>.
    /// </summary>
    /// <param name="horizontalInput">
    /// pre: this value should be clamped between -1 and 1.
    /// It represents the verse (on x-axis) and the intensity of the desired movement.
    /// </param>
    /// <param name="drivingForce">
    /// pre: this value should be > 0.
    /// It represents the driving force of the desired movement.
    /// </param>
    public void HandleMovement(float horizontalInput, float drivingForce)
    {
        horizontalInput = Mathf.Clamp(horizontalInput, -1, 1);
        drivingForce = Mathf.Abs(horizontalInput) * Mathf.Abs(drivingForce);

        AttachedRigidbody.drag = 4;

        if (Mathf.Abs(horizontalInput) > 0)
        {
            AttachedRigidbody.sharedMaterial = _zeroFriction;

            Rotate(horizontalInput);
            Move(transform.right, drivingForce);
        } else
        {
            AttachedRigidbody.sharedMaterial = _maxFriction;
        }
    }

    /// <summary>
    /// Rotates the character in the correct verse, according to the <c>horizontalInput</c> parameter.
    /// </summary>
    /// <param name="horizontalInput">
    /// pre: this value should be clamped between -1 and 1.
    /// It's used to change the verse (on x-axis) of the character.
    /// </param>
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

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
    }

    /// <summary>
    /// It moves continuously the character on <c>direction</c> with the <c>drivingForce</c>.
    /// </summary>
    /// <param name="direction">
    /// pre: this vector should be normalized.
    /// It's used to determine the correct direction of the force to apply continuously to the <c>AttachedRigidbody</c>.
    /// </param>
    /// It's used to determine the intensity of the force to apply continuously to the <c>AttachedRigidbody</c>.
    /// <param name="drivingForce"></param>
    private void Move(Vector2 direction, float drivingForce)
    {
        Vector2 forceToApply = direction.normalized * drivingForce;
        AttachedRigidbody.AddForce(forceToApply * AttachedRigidbody.mass);
    }

    /// <summary>
    /// This method gives an instant impulse to the character on <c>direction</c> with the <c>drivingForce</c>.
    /// </summary>
    /// <param name="direction">
    /// pre: this vector should be normalized.
    /// It's used to determine the correct direction of the instant impulse to apply to the <c>AttachedRigidbody</c>.
    /// </param>
    /// <param name="force">
    /// It's used to determine the intensity of the instant impulse to apply to the <c>AttachedRigidbody</c>.
    /// </param>
    public void GiveImpulse(Vector2 direction, float force)
    {
        Vector3 forceToApply = direction.normalized * force;
        AttachedRigidbody.AddForce(forceToApply * AttachedRigidbody.mass, ForceMode2D.Impulse);
    }
}
