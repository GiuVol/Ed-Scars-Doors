/// <summary>
/// Interface <c>IStatusable</c>
/// Interface for character status management.
/// </summary>
public interface IStatusable
{
    /// <summary>
    /// Property <c>Status</c>
    /// This property returns the <c>StatusComponent</c> attached to a character.
    /// </summary>
    public StatusComponent Status { get; }
}
