using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElegantMan : GenericMob
{
    private const float MaxSpeed = 7;

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
        if (_isAttacking)
        {
            return;
        }

        if (_player != null)
        {
            float distance = Vector2.Distance(transform.position, _player.transform.position);

            if (distance > _attackRange)
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
            } else
            {
                if (_canAttack)
                {
                    StartCoroutine(HandleAttack(_player));
                }
            }
        }

        Vector2 localSpaceVelocity = transform.InverseTransformDirection(_attachedRigidbody.velocity);
        float normalizedSpeed = localSpaceVelocity.x / MaxSpeed;

        if (normalizedSpeed < .2f)
        {
            normalizedSpeed = 0;
        }

        AnimController.SetFloat(SpeedParameterName, normalizedSpeed);
    }

    protected override IEnumerator Attack(PlayerController target)
    {
        Vector2 lookDirection = (target.transform.position - transform.position).normalized;
        lookDirection.y = 0;

        #region Rotating

        if (lookDirection.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (lookDirection.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }

        #endregion

        AnimController.SetTrigger(AttackParameterName);

        yield return new WaitUntil(() => AnimController.GetCurrentAnimatorStateInfo(0).IsName(AttackStateName));

        AnimatorStateInfo info = AnimController.GetCurrentAnimatorStateInfo(0);

        float animationDuration = info.length / info.speed;

        yield return new WaitForSeconds(animationDuration * AttackDamagingPhasePercentage);

        yield return new WaitUntil(() => !AnimController.GetCurrentAnimatorStateInfo(0).IsName(AttackStateName));
    }

    private void InflictDamage(Collider2D collision, float power)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer(PlayerController.PlayerLayerName))
        {
            return;
        }

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player == null)
        {
            player = collision.gameObject.GetComponentInChildren<PlayerController>();
        }

        if (player == null)
        {
            player = collision.gameObject.GetComponentInParent<PlayerController>();
        }

        if (player != null)
        {
            HealthComponent healthComponent = player.Health;
            StatsComponent statsComponent = player.Stats;
            int damage = GameFormulas.Damage(power, Stats.Attack.CurrentValue, statsComponent.Defence.CurrentValue);
            healthComponent.Decrease(damage);
        }
    }
    
    protected override IEnumerator Die()
    {
        yield break;
    }
}
