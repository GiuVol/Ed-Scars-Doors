using UnityEngine;

public class CameraController : MonoBehaviour
{
    /// <summary>
    /// The <c>Transform</c> that the camera should follow.
    /// </summary>
    public Transform Target;

    /// <summary>
    /// A <c>Vector3</c> that stores the offset, relative to the <c>Target</c>, that the camera should have.
    /// </summary>
    public Vector3 Offset;

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

    void FixedUpdate()
    {
        if (Target != null)
        {
            FollowTarget();
        }
    }

    /// <summary>
    /// Methods that follows the <c>Target</c>, according to <c>Offset</c> and <c>SmoothTime</c>
    /// </summary>
    private void FollowTarget()
    {
        _desiredPosition = 
            new Vector3(Target.position.x, Target.position.y) + Offset;

        transform.position = Vector3.SmoothDamp(transform.position, _desiredPosition, ref _currentVelocity, SmoothTime);
    }
}
