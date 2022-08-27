using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElegantMan : GenericMob
{
    /// <summary>
    /// Consts useful for the Animator's handling.
    /// </summary>
    #region Animator's consts

    private const string LocomotionCycleName = "LocomotionCycle";
    private const string AttackStateName = "Attack";

    private const string SpeedParameterName = "Speed";
    private const string AttackParameterName = "Attack";

    private const float AttackDamagingPhasePercentage = .2f;

    #endregion
    
    protected override UIBar HealthBarResource => null;

    private new void Start()
    {
        base.Start();
        Health.IsInvincible = true;
        Status.IsImmune = true;

        AnimController = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        if (_player != null)
        {
            Vector2 moveDirection = (_player.transform.position - transform.position).normalized;
            moveDirection.y = 0;
            float forceFactor = _speed;
            _attachedRigidbody.AddForce(moveDirection * forceFactor);

            #region Rotating

            if (moveDirection.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (moveDirection.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, -180, 0);
            }

            #endregion
        }

        Vector2 localSpaceVelocity = transform.InverseTransformDirection(_attachedRigidbody.velocity);
        float normalizedSpeed = localSpaceVelocity.x / (_speed / _attachedRigidbody.drag);

        if (normalizedSpeed < .2f)
        {
            normalizedSpeed = 0;
        }

        AnimController.SetFloat(SpeedParameterName, normalizedSpeed);
    }

    protected override IEnumerator Attack(PlayerController target)
    {
        throw new System.NotImplementedException();
    }
    
    protected override IEnumerator Die()
    {
        yield break;
    }
}
