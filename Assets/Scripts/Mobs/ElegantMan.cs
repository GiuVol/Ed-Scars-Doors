using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElegantMan : GenericMob
{
    protected override UIBar HealthBarResource => null;

    private new void Start()
    {
        base.Start();
        Health.IsInvincible = true;
        Status.IsImmune = true;
    }

    private void FixedUpdate()
    {
        if (_player != null)
        {
            Vector2 moveDirection = (_player.transform.position - transform.position).normalized;
            moveDirection.y = 0;
            _attachedRigidbody.AddForce(moveDirection * _mass * _speed);

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
    }

    protected override IEnumerator Attack(PlayerController target)
    {
        throw new System.NotImplementedException();
    }
    
    protected override IEnumerator Die()
    {
        throw new System.NotImplementedException();
    }
}
