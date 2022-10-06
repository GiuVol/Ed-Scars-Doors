using UnityEngine;

public class MantmareSpit : Projectile
{
    private const string ExplodeParameterName = "Explode";

    protected new void Start()
    {
        base.Start();

        Animator animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            OnHit = delegate { animator.SetTrigger(ExplodeParameterName); };
        }
    }
}
