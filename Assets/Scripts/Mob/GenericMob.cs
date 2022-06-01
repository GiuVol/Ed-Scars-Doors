using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HealthComponent;

public abstract class GenericMob : MonoBehaviour
{
    /// <summary>
    /// The <c>HealthComponent</c> that stores values and methods related to the health of the player.
    /// </summary>
    public HealthComponent Health { get; protected set; }

    /// <summary>
    /// The <c>StatsComponent</c> that stores values and methods related to the stats of the player.
    /// </summary>
    public StatsComponent Stats { get; protected set; }

    /// <summary>
    /// The <c>StatusComponent</c> that stores values and methods related to the status of the player.
    /// </summary>
    public StatusComponent Status { get; protected set; }

    protected MobAI MobAI;

    protected string Name;
    protected bool CanAttack;
    protected float AttackInterval;

   private void Setup()
    {
        SetupMobAI();
        SetupHealth();
        SetupStats();
        SetupStatus();
        SetName();
    }

    abstract protected void SetupMob();

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.GetComponent<StatusComponent>() == null)
        {
            gameObject.AddComponent<StatusComponent>();
        }

        if (gameObject.GetComponent<MobAI>() == null)
        {
            gameObject.AddComponent<MobAI>();
        }

        MobAI = gameObject.GetComponent<MobAI>();
        Status = gameObject.GetComponent<StatusComponent>();

        

        Setup();
    }

    // Update is called once per frame
     void Update()
    {
        StartCoroutine(Attack());
    }

    abstract public void SetupHealth();
    abstract public void SetupStatus();
    abstract public void SetupMobAI();
    abstract public void SetupStats();
    abstract public IEnumerator Attack();
    abstract public void SetName();
    public void Die()
    {
        Destroy(gameObject);
    }
}
