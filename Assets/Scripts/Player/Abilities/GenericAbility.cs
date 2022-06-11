using UnityEngine;

public abstract class GenericAbility : ScriptableObject
{
    /// <summary>
    /// The name of the ability.
    /// </summary>
    public string Name;

    /// <summary>
    /// The description of tha ability.
    /// </summary>
    public string Description;

    /// <summary>
    /// This method enables the ability on the <c>PlayerController</c> in input.
    /// </summary>
    /// <param name="playerController">The <c>PlayerController</c> on which you want to enable the ability</param>
    public abstract void Enable(PlayerController playerController);

    /// <summary>
    /// This method disables the ability on the <c>PlayerController</c> in input.
    /// </summary>
    /// <param name="playerController">The <c>PlayerController</c> on which you want to disable the ability</param>
    public abstract void Disable(PlayerController playerController);
}
