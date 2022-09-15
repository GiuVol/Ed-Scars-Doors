using UnityEngine;

public class DynamicUIComponent : MonoBehaviour
{
    /// <summary>
    /// Stores whether the UI component is static or dynamic.
    /// </summary>
    public bool IsDynamic { get; private set; }

    /// <summary>
    /// Stores the target to follow.
    /// </summary>
    public Transform TargetToFollow { get; set; }

    /// <summary>
    /// A <c>Vector3</c> that stores the offset, relative to the <c>Target</c>, that the component should have.
    /// </summary>
    public Vector3 PositionOffset { get; set; }

    /// <summary>
    /// Initializes the UI component, to follow a specific transform.
    /// </summary>
    /// <param name="targetToFollow">The transform that the component should follow</param>
    /// <param name="positionOffset">The position offset</param>
    public void InitializeDynamic(Transform targetToFollow, Vector3 positionOffset)
    {
        IsDynamic = true;
        TargetToFollow = targetToFollow;
        PositionOffset = positionOffset;
    }

    /// <summary>
    /// Initializes a static UI component.
    /// </summary>
    public void InitializeStatic()
    {
        IsDynamic = false;
        TargetToFollow = null;
        PositionOffset = Vector3.zero;
    }
    
    protected void Update()
    {
        if (IsDynamic)
        {
            if (TargetToFollow == null)
            {
                gameObject.SetActive(false);
            } else
            {
                UpdateDynamicPosition();
            }
        }
    }

    /// <summary>
    /// Updates the position of the component, to follow the target.
    /// </summary>
    private void UpdateDynamicPosition()
    {
        if (TargetToFollow == null)
        {
            return;
        }

        Vector3 desiredPosition = TargetToFollow.position + PositionOffset;

        transform.position = Camera.main.WorldToScreenPoint(desiredPosition);

        float distance = Vector3.Distance(Camera.main.transform.position,
                                          TargetToFollow.position);

        float desiredScale = (100 / distance);
        desiredScale = Mathf.Clamp(desiredScale, .01f, 1);

        transform.localScale = Vector2.one * desiredScale;
    }
}
