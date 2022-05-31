using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HealthComponent;

public abstract class GenericMob : MonoBehaviour
{
    /// <summary>
    /// The <c>HealthComponent</c> that stores values and methods related to the health of the player.
    /// </summary>
    public HealthComponent Health { get; private set; }

    /// <summary>
    /// The <c>StatsComponent</c> that stores values and methods related to the stats of the player.
    /// </summary>
    public StatsComponent Stats { get; private set; }

    /// <summary>
    /// The <c>StatusComponent</c> that stores values and methods related to the status of the player.
    /// </summary>
    public StatusComponent Status { get; private set; }

    EnemyAI EnemyAI;

   private void Setup()
    {
        SetupEnemyAI();
        SetupHealth();
        SetupStats();
        SetupStatus();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.GetComponent<StatusComponent>() == null)
        {
            gameObject.AddComponent<StatusComponent>();
        }

        if (gameObject.GetComponent<EnemyAI>() == null)
        {
            gameObject.AddComponent<EnemyAI>();
        }

        EnemyAI = gameObject.GetComponent<EnemyAI>();
        Status = gameObject.GetComponent<StatusComponent>();

        

        Setup();
    }

    // Update is called once per frame
     void Update()
    {
        if (CanAttack())
        {
            Attack();
        }
    }

    abstract public void SetupHealth();
    abstract public void SetupStatus();
    abstract public void SetupEnemyAI();
    abstract public void SetupStats();
    abstract public void Attack();
    abstract public bool CanAttack();
    public void Die()
    {
        Destroy(gameObject);
    }
}
