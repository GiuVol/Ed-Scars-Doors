public abstract class GenericAbility
{
    public bool IsEnabled { get; protected set; }

    public void Enable(PlayerController playerController)
    {
        if (IsEnabled)
        {
            return;
        }

        IsEnabled = true;
        Setup(playerController);
    }

    public void Disable(PlayerController playerController)
    {
        if (!IsEnabled)
        {
            return;
        }
        
        IsEnabled = false;
        Takedown(playerController);
    }

    protected abstract void Setup(PlayerController playerController);
    protected abstract void Takedown(PlayerController playerController);
}
