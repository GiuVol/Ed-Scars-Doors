using UnityEngine;

public class CameraLockEventTrigger : EventTrigger
{
    [SerializeField]
    private Transform _cameraLockTarget;

    [SerializeField]
    private float _desiredSize;

    protected override void Action()
    {
        CameraController camera = GameObject.FindObjectOfType<CameraController>();
        Regia regia = GameObject.FindObjectOfType<Regia>();

        if (regia == null || camera == null)
        {
            return;
        }

        regia.Disabled = true;

        camera.LockToPosition(_cameraLockTarget.position, .5f);
        camera.LockBoundries = true;
        camera.OrthographicSize = _desiredSize;
    }
}
