using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /// <summary>
    /// The orthographic size that the camera has at the start.
    /// </summary>
    private const float StandardOrthographicSize = 10f;

    /// <summary>
    /// The min orthographic size that the camera can have.
    /// </summary>
    private const float MinOrthographicSize = 4.5f;

    /// <summary>
    /// The max orthographic size that the camera can have.
    /// </summary>
    private const float MaxOrthographicSize = 15;

    /// <summary>
    /// The time (in seconds) that the camera needs to reach the desired position.
    /// </summary>
    [SerializeField]
    private float _smoothTime = .1f;

    /// <summary>
    /// The standard position offset of the camera.
    /// </summary>
    [SerializeField]
    private Vector3 _standardPositionOffset;

    /// <summary>
    /// Stores whether the x component of the position offset must be inverted when the player 
    /// changes direction.
    /// </summary>
    [SerializeField]
    private bool _canFlipX;

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

    /// <summary>
    /// Stores whether the camera should stop following the target.
    /// </summary>
    private bool _fixed;

    /// <summary>
    /// Stores the coroutine that is currently locking the camera to a position.
    /// </summary>
    private Coroutine LTPCoroutine;

    /// <summary>
    /// Stores whether the screen boundries must be locked or not.
    /// </summary>
    private bool _lockBoundries;

    /// <summary>
    /// A property to access in a controlled way to the _lockBoundries field, 
    /// which stores whether the screen boundries must be locked or not.
    /// </summary>
    public bool LockBoundries
    {
        get
        {
            return _lockBoundries;
        }

        set
        {
            _lockBoundries = value;
        }
    }
    
    /// <summary>
    /// Stores the edge collider 2D that can lock the screen boundries.
    /// </summary>
    private EdgeCollider2D _edgeCollider;

    private void Start()
    {
        PositionOffset = _standardPositionOffset;
        //OrthographicSize = StandardOrthographicSize;

        _edgeCollider = new GameObject("EdgeCollider").AddComponent<EdgeCollider2D>();
        _edgeCollider.transform.position = transform.position;
        _edgeCollider.transform.parent = transform;
        _edgeCollider.transform.localPosition = Vector3.zero;
        _edgeCollider.enabled = false;
        _edgeCollider.gameObject.AddComponent<ScreenBoundriesCollisionManager>();

        #region Debug

        LockToPosition(transform.position, 2);
        _lockBoundries = true;
        
        if (Target == null)
        {
            Target = GameObject.FindWithTag("Player").transform;
        }

        #endregion
    }

    void FixedUpdate()
    {
        if (Target != null && !_fixed)
        {
            FollowTarget();
        }

        if (_fixed && _lockBoundries)
        {
            if (!_edgeCollider.enabled)
            {
                _edgeCollider.enabled = true;
            }

            float xMin = CameraComponent.ViewportToWorldPoint(new Vector3(0, 0, 0)).x - CameraComponent.transform.position.x;
            float xMax = CameraComponent.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - CameraComponent.transform.position.x;
            float yMin = CameraComponent.ViewportToWorldPoint(new Vector3(0, 0, 0)).y - CameraComponent.transform.position.y;
            float yMax = CameraComponent.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - CameraComponent.transform.position.y;

            Vector3 point1 = new Vector3(xMin, yMax);
            Vector3 point2 = new Vector3(xMax, yMax);
            Vector3 point3 = new Vector3(xMax, yMin);
            Vector3 point4 = new Vector3(xMin, yMin);

            Vector2[] tempArray = new Vector2[] { point1, point2, point3, point4, point1};

            _edgeCollider.points = tempArray;
        } else
        {
            if (_edgeCollider.enabled)
            {
                _edgeCollider.enabled = false;
            }
        }
    }

    /// <summary>
    /// Method that allows to follow the <c>Target</c>, according to <c>Offset</c> and <c>SmoothTime</c>
    /// </summary>
    private void FollowTarget()
    {
        bool flipX = _canFlipX && Target.transform.right.x < 0;

        float actualPositionOffsetX = flipX ? -PositionOffset.x : PositionOffset.x;
        Vector3 actualPositionOffset = new Vector3(actualPositionOffsetX, PositionOffset.y, PositionOffset.z);

        _desiredPosition = new Vector3(Target.position.x, Target.position.y) + actualPositionOffset;
        _desiredPosition.z = -5;

        transform.position = Vector3.SmoothDamp(transform.position, _desiredPosition, ref _currentVelocity, _smoothTime);
    }

    /// <summary>
    /// Method that locks the camera to a position. Preventing it from following the target.
    /// </summary>
    /// <param name="position">The position in which the camera should be</param>
    /// <param name="speed">How fast the camera should lerp the position</param>
    public void LockToPosition(Vector3 position, float speed)
    {
        LTPCoroutine = StartCoroutine(LockToPositionCoroutine(position, speed));
    }

    /// <summary>
    /// The IEnumerator that handles the camera locking on a position.
    /// </summary>
    /// <param name="position">The position in which the camera should be</param>
    /// <param name="speed">How fast the camera should lerp the position</param>
    private IEnumerator LockToPositionCoroutine(Vector3 position, float speed)
    {
        _fixed = true;

        Vector3 startPosition = transform.position;
        float lerpFactor = 0;

        do
        {
            transform.position = Vector3.Lerp(startPosition, position, lerpFactor);
            lerpFactor = Mathf.Clamp01(lerpFactor + Time.fixedDeltaTime * speed);
            yield return null;
        } while (lerpFactor < 1);
    }

    /// <summary>
    /// Method that makes sure that the camera follows the target.
    /// </summary>
    public void Unlock()
    {
        if (LTPCoroutine != null)
        {
            StopCoroutine(LTPCoroutine);
            LTPCoroutine = null;
        }

        _fixed = false;
    }
}
