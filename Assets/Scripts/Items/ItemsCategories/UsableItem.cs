public abstract class UsableItem : Item
{
    /// <summary>
    /// Procedure that is called when an item is used on the player.
    /// </summary>
    /// <param name="player">the player controller on which the item is used</param>
    public abstract void Use(PlayerController player);
}
