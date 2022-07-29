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
    /// The time (in seconds) that the camera needs to reach the desired position.
    /// </summary>
    [SerializeField]
    private float _smoothTime = .1f;

    /// <summary>
    /// The standard position offset of the camera.
    /// </summary>
    [SerializeField]
    public Vector3 _standardPositionOffset;

    /// <summary>
    /// The <c>Camera</c> attached to the controller.
    /// </summary>
    private Camera _cameraComponent;
    
    /// <summary>
    /// The <c>Camera</c> attached to the controller.
    /// </summary>
    public Camera CameraComponent
    {
        get
        {
            if (_cameraComponent == null)
            {
                _cameraComponent = gameObject.GetComponentInChildren<Camera>();
            }

            return _cameraComponent;
        }
    }
    
    /// <summary>
    /// The <c>Transform</c> that the camera should follow.
    /// </summary>
    public Transform Target { get; set; }

    /// <summary>
    /// A <c>Vector3</c> that stores the offset, relative to the <c>Target</c>, 
    /// that the camera should have.
    /// </summary>
    public Vector3 PositionOffset { get; set; }
    
    /// <summary>
    /// Property to access in a controlled way to the orthographic size of the camera
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
    /// A <c>Vector3</c> that stores the position that the camera should have.
    /// </summary>
    private Vector3 _desiredPosition;

    /// <summary>
    /// Support variable that is used by the <c>Vector3.SmoothDamp()</c> method.
    /// </summary>
    private Vector3 _currentVelocity;

    private void Start()
    {
        PositionOffset = _standardPositionOffset;
        OrthographicSize = StandardOrthographicSize;

        #region Debug

        if (Target == null)
        {
            Target = GameObject.FindWithTag("Player").transform;
        }

        #endregion
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
        bool flipX = Target.transform.right.x < 0;

        float actualPositionOffsetX = flipX ? -PositionOffset.x : PositionOffset.x;
        Vector3 actualPositionOffset = new Vector3(actualPositionOffsetX, PositionOffset.y, PositionOffset.z);

        _desiredPosition = new Vector3(Target.position.x, Target.position.y) + actualPositionOffset;
        _desiredPosition.z = -5;

        transform.position = Vector3.SmoothDamp(transform.position, _desiredPosition, ref _currentVelocity, _smoothTime);
    }
}
