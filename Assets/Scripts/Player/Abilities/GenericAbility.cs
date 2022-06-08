using UnityEngine;

public abstract class GenericAbility : ScriptableObject
{
    /// <summary>
    /// This method enables the ability on the <c>PlayerController</c> in input.
    /// </summary>
    /// <param name="playerController">The <c>PlayerController</c> on which you want to enable the ability</param>
    public void Enable(PlayerController playerController)
    {
        foreach (GenericAbility equippedAbility in playerController.EquippedAbilities)
        {
            if (equippedAbility.GetType().IsEquivalentTo(this.GetType()))
            {
                Debug.Log("An ability of this type is already equipped!");
                return;
            }
        }

        playerController.EquippedAbilities.Add(this);
        Setup(playerController);
    }

    /// <summary>
    /// This method disables the ability on the <c>PlayerController</c> in input.
    /// </summary>
    /// <param name="playerController">The <c>PlayerController</c> on which you want to disable the ability</param>
    public void Disable(PlayerController playerController)
    {
        if (!playerController.EquippedAbilities.Contains(this))
        {
            return;
        }

        playerController.EquippedAbilities.Remove(this);
        Takedown(playerController);
    }

    protected abstract void Setup(PlayerController playerController);
    protected abstract void Takedown(PlayerController playerController);
}
