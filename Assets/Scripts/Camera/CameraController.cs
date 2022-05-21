using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset;
    public float SmoothTime = .1f;

    private Vector3 _desiredPosition;
    private Vector3 _currentVelocity;

    void FixedUpdate()
    {
        if (Target != null)
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        _desiredPosition = 
            new Vector3(Target.position.x, Target.position.y) + Offset;

        transform.position = Vector3.SmoothDamp(transform.position, _desiredPosition, ref _currentVelocity, SmoothTime);
    }
}
