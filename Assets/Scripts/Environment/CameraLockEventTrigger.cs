using System.Collections;
using UnityEngine;

public class CameraLockEventTrigger : EventTrigger
{
    #region Serialized

    [SerializeField]
    private Transform _cameraLockTarget;

    [SerializeField]
    private float _desiredSize;

    [SerializeField]
    private float _speedToReachPosition;
    
    [SerializeField]
    private float _speedToReachSize;

    #endregion

    protected override IEnumerator Action()
    {
        CameraController cameraController = GameObject.FindObjectOfType<CameraController>();
        Regia regia = GameObject.FindObjectOfType<Regia>();

        if (regia == null || cameraController == null)
        {
            yield break;
        }

        regia.Disabled = true;

        cameraController.LockToPosition(_cameraLockTarget.position, Mathf.Max(.1f, _speedToReachPosition));
        cameraController.LockBoundries = true;

        float startSize = cameraController.OrthographicSize;
        float lerpFactor = 0;

        do
        {
            cameraController.OrthographicSize = Mathf.Lerp(startSize, _desiredSize, lerpFactor);
            lerpFactor = Mathf.Clamp01(lerpFactor + (Time.fixedDeltaTime * Mathf.Max(.1f, _speedToReachSize)));

            yield return new WaitForFixedUpdate();

        } while (lerpFactor < 1);
    }
}
