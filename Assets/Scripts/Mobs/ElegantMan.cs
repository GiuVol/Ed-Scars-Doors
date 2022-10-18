using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElegantMan : GenericMob
{
    private const float MaxSpeed = 7;
    private const string ElegantManLayerName = "ElegantMan";

    [SerializeField]
    private TriggerCaster _armTriggerCaster;

    [SerializeField]
    private float _ttl;

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

    private float _ingameTime;

    private bool _isDisappearing;
    
    protected override UIBar HealthBarResource => null;

    protected override UIBar BlindnessBarResource => null;

    protected override UIBar CorrosionBarResource => null;

    private new void Start()
    {
        Appear();

        base.Start();
        Health.IsInvincible = true;
        Status.IsImmune = true;

        AnimController = GetComponentInChildren<Animator>();

        _ingameTime = 0;
        _isDisappearing = false;
    }

    protected override void SetupLayers()
    {
        CustomUtilities.SetLayerRecursively(gameObject, LayerMask.NameToLayer(ElegantManLayerName));
    }

    private void FixedUpdate()
    {
        Vector2 localSpaceVelocity = transform.InverseTransformDirection(_attachedRigidbody.velocity);
        float normalizedSpeed = localSpaceVelocity.x / MaxSpeed;

        if (normalizedSpeed < .2f)
        {
            normalizedSpeed = 0;
        }

        AnimController.SetFloat(SpeedParameterName, normalizedSpeed);

        if (_isDisappearing)
        {
            return;
        }

        _ingameTime += Time.fixedDeltaTime;

        if (_ingameTime > _ttl)
        {
            Disappear();
            return;
        }

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

        if (_armTriggerCaster != null)
        {
            foreach (Collider2D collider in _armTriggerCaster.GetComponents<Collider2D>())
            {
                collider.enabled = false;
            }
        }

        yield return new WaitForSeconds(animationDuration * AttackDamagingPhasePercentage);

        if (_armTriggerCaster != null)
        {
            _armTriggerCaster.TriggerFunction = collider => {
                AudioClipHandler.PlayAudio("Audio/Damage", 0, transform.position, false, .15f);
                InflictDamage(collider, 1);
            };

            foreach (Collider2D collider in _armTriggerCaster.GetComponents<Collider2D>())
            {
                collider.enabled = true;
            }
        }
        
        yield return new WaitUntil(() => !AnimController.GetCurrentAnimatorStateInfo(0).IsName(AttackStateName));

        if (_armTriggerCaster != null)
        {
            _armTriggerCaster.TriggerFunction = null;
        }
    }

    private void InflictDamage(Collider2D collision, float healthPercentage)
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
            healthComponent.DecreasePercentage(healthPercentage);
        }
    }
    
    protected override IEnumerator Die()
    {
        yield break;
    }

    private void Appear()
    {
        StartCoroutine(AppearCoroutine());
    }

    private IEnumerator AppearCoroutine()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        if (renderers.Length <= 0)
        {
            yield break;
        }

        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0);
        }
        
        float timeToAppear = 1;
        float timePassed = 0;

        if (timeToAppear > 0)
        {
            float alphaValue;

            do
            {
                yield return new WaitForFixedUpdate();

                timePassed += Time.fixedDeltaTime;

                alphaValue = Mathf.Clamp01(timePassed / timeToAppear);

                foreach (SpriteRenderer renderer in renderers)
                {
                    renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alphaValue);
                }

            } while (alphaValue < 1);
        } else
        {
            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1);
            }
        }
    }

    private void Disappear()
    {
        if (_isDisappearing)
        {
            return;
        }

        _isDisappearing = true;
        StartCoroutine(DisappearCoroutine());
    }

    private IEnumerator DisappearCoroutine()
    {
        float timeToFade = 1;
        float timePassed = 0;

        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        if (timeToFade > 0)
        {
            if (renderers.Length > 0)
            {
                float alphaValue = renderers[0].color.a;
                float startingAlphaValue = alphaValue;

                do
                {
                    yield return new WaitForFixedUpdate();

                    timePassed += Time.fixedDeltaTime;

                    alphaValue = startingAlphaValue * (1 - Mathf.Clamp01(timePassed / timeToFade));

                    alphaValue = Mathf.Max(alphaValue, 0);

                    foreach (SpriteRenderer renderer in renderers)
                    {
                        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alphaValue);
                    }

                } while (alphaValue > 0);
            }
        }

        Destroy(gameObject);
    }
}
