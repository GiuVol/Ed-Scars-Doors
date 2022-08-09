using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mantmare : GenericMob
{
    protected override UIBar HealthBarResource
    {
        get
        {
            return Resources.Load<UIBar>("UI/MantmareHealthbar");
        }
    }
    
    private float AttackIntervalByStage
    {
        get
        {
            float attackInterval;

            int currentHealth = Health.CurrentHealth;
            int maxHealth = Health.MaxHealth;

            if (currentHealth > Mathf.RoundToInt(0.6f * maxHealth))
            {
                attackInterval = Random.Range(4, 6);
            } else if (currentHealth > Mathf.RoundToInt(0.4f * maxHealth))
            {
                attackInterval = Random.Range(2, 4);
            } else
            {
                attackInterval = Random.Range(1.5f, 3.5f);
            }

            return attackInterval;
        }
    }

    private bool IsOnScreen
    {
        get
        {
            bool isOnScreen = false;

            if (transform.position.x > Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x && 
                transform.position.x < Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x && 
                transform.position.y > Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y &&
                transform.position.y < Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y)
            {
                isOnScreen = true;
            }

            return isOnScreen;
        }
    }

    private float _timeOnScreen;

    protected new void Start()
    {
        base.Start();

        AnimController = GetComponentInChildren<Animator>();

        _attackInterval = AttackIntervalByStage;

        if (IsOnScreen)
        {
            _timeOnScreen = 2;
        }
    }

    private void FixedUpdate()
    {
        Vector3 localSpaceXVelocity = transform.InverseTransformDirection(_attachedRigidbody.velocity);
        float normalizedXSpeed = localSpaceXVelocity.x / (_speed / _attachedRigidbody.drag);

        Vector3 localSpaceYVelocity = transform.InverseTransformDirection(_attachedRigidbody.velocity);
        float normalizedYSpeed = localSpaceYVelocity.y / (_speed / _attachedRigidbody.drag);

        AnimController.SetFloat("HorizontalSpeed", normalizedXSpeed);
        AnimController.SetFloat("VerticalSpeed", normalizedYSpeed);

        if (HealthBar != null)
        {
            HealthBar.UpdateCurrentValue(Health.CurrentHealth);
        }

        if (IsOnScreen)
        {
            _timeOnScreen = Mathf.Clamp(_timeOnScreen + Time.deltaTime, 0, 100);
        } else
        {
            _timeOnScreen = 0;
        }

        if (_timeOnScreen < 2)
        {
            ReachPosition(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2)), 100);
            return;
        }

        if(_player == null)
        {
            return;
        }

        if (!_canAttack)
        {

        }
    }

    protected override void SetupHealthBar()
    {
        if (HealthBarResource != null)
        {
            HealthBar = Instantiate(HealthBarResource, GameObject.FindObjectOfType<Canvas>().transform);
            HealthBar.InitializeStatic(Health.MaxHealth);
        }
    }

    protected override void SetupLayers()
    {
        base.SetupLayers();
    }

    private void ReachPosition(Vector3 positionToReach, float speed = 0)
    {
        if (speed <= 0)
        {
            speed = _speed;
        }

        Vector3 direction = (positionToReach - transform.position).normalized;

        _attachedRigidbody.AddForce(direction * _mass * speed);

        Quaternion desiredRotation = transform.rotation;

        if (direction.x > 0)
        {
            desiredRotation = Quaternion.Euler(0, 0, 0);
        } else if (direction.x < 0)
        {
            desiredRotation = Quaternion.Euler(0, -180, 0);
        }

        transform.rotation = desiredRotation;
    }

    private void RandomWander()
    {

    }

    protected override IEnumerator Attack(PlayerController target)
    {
        switch (Random.Range(1, 4))
        {
            case 1:
                yield return Pattern1();
                break;
            case 2:
                yield return Pattern2();
                break;
            case 3:
                yield return Pattern3();
                break;
            default:
                yield return Pattern1();
                break;
        }

        _attackInterval = AttackIntervalByStage;
    }

    /// <summary>
    /// The 1st attack pattern of Mantmare.
    /// </summary>
    private IEnumerator Pattern1()
    {
        yield break;
    }

    /// <summary>
    /// The 2nd attack pattern of Mantmare.
    /// </summary>
    private IEnumerator Pattern2()
    {
        yield break;
    }

    /// <summary>
    /// The 3rd attack pattern of Mantmare.
    /// </summary>
    private IEnumerator Pattern3()
    {
        yield break;
    }
    
    protected override IEnumerator Die()
    {
        Destroy(gameObject);

        yield break;
    }

    private bool GetRandomPointOnScreen(out Vector3 position)
    {
        if (Camera.main == null)
        {
            position = Vector3.zero;
            return false;
        }

        float randomX = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);
        float randomY = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);

        position = new Vector3(randomX, randomY);

        return true;
    }
}
