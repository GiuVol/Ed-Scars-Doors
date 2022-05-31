using UnityEngine;

public class CameraController : MonoBehaviour
{
    /// <summary>
    /// The orthographic size that the camera has at the start.
    /// </summary>
    private const float StandardOrthographicSize = 7f;

    /// <summary>
    /// The min orthographic size that the camera can have.
    /// </summary>
    private const float MinOrthographicSize = 4.5f;

    /// <summary>
    /// The max orthographic size that the camera can have.
    /// </summary>
    private const float MaxOrthographicSize = 10;
    
    /// <summary>
    /// The <c>Camera</c> attached to the controller.
    /// </summary>
    public Camera CameraComponent { get; private set; }

    /// <summary>
    /// The <c>Transform</c> that the camera should follow.
    /// </summary>
    public Transform Target;

    /// <summary>
    /// A <c>Vector3</c> that stores the offset, relative to the <c>Target</c>, that the camera should have.
    /// </summary>
    public Vector3 Offset;

    /// <summary>
    /// The desired orthographic size that the camera should have.
    /// </summary>
    public float OrthographicSize
    {
        get
        {
            if (CameraComponent == null)
            {
                return 0;
            }

            return CameraComponent.orthographicSize;
        }
        set
        {
            if (CameraComponent == null)
            {
                return;
            }

            CameraComponent.orthographicSize = Mathf.Clamp(value, MinOrthographicSize, MaxOrthographicSize);
        }
    }
    
    /// <summary>
    /// The time (in seconds) that the camera needs to reach the desired position.
    /// </summary>
    public float SmoothTime = .1f;

    /// <summary>
    /// A <c>Vector3</c> that stores the position that the camera should have.
    /// </summary>
    private Vector3 _desiredPosition;

    /// <summary>
    /// Support variable that is used by the <c>Vector3.SmoothDamp()</c> method.
    /// </summary>
    private Vector3 _currentVelocity;

    private void Start()
    {
        CameraComponent = gameObject.GetComponentInChildren<Camera>();

        OrthographicSize = StandardOrthographicSize;
    }

    void FixedUpdate()
    {
        if (Target != null)
        {
            FollowTarget();
        }
    }

    /// <summary>
    /// Method that allows to follow the <c>Target</c>, according to <c>Offset</c> and <c>SmoothTime</c>
    /// </summary>
    private void FollowTarget()
    {
        _desiredPosition = new Vector3(Target.position.x, Target.position.y) + Offset;
        _desiredPosition.z = -5;

        transform.position = Vector3.SmoothDamp(transform.position, _desiredPosition, ref _currentVelocity, SmoothTime);
    }
}
