using UnityEngine;

public class HUD : MonoBehaviour
{
    /// <summary>
    /// The bar, shown in the HUD, that shows the health of the player.
    /// </summary>
    [SerializeField]
    private UIBar _playerHealthBar;

    /// <summary>
    /// The bar, shown in the HUD, that shows the blindness level of the player.
    /// </summary>
    [SerializeField]
    private UIBar _playerBlindnessBar;

    /// <summary>
    /// The bar, shown in the HUD, that shows the corrosion time of the player.
    /// </summary>
    [SerializeField]
    private UIBar _playerCorrosionBar;

    /// <summary>
    /// This property provides access to <c>_playerHealthBar</c> in a controlled way.
    /// </summary>
    public UIBar PlayerHealthBar
    {
        get
        {
            return _playerHealthBar;
        }
    }

    /// <summary>
    /// This property provides access to <c>_playerBlindnessBar</c> in a controlled way.
    /// </summary>
    public UIBar PlayerBlindnessBar
    {
        get
        {
            return _playerBlindnessBar;
        }
    }

    /// <summary>
    /// This property provides access to <c>_playerCorrosionBar</c> in a controlled way.
    /// </summary>
    public UIBar PlayerCorrosionBar
    {
        get
        {
            return _playerCorrosionBar;
        }
    }

    public void ResetBars()
    {
        if (PlayerHealthBar != null)
        {
            PlayerHealthBar.UpdateValueInstantly(0);
        }

        if (PlayerBlindnessBar != null)
        {
            PlayerBlindnessBar.UpdateValueInstantly(0);
        }

        if (PlayerCorrosionBar != null)
        {
            PlayerCorrosionBar.UpdateValueInstantly(0);
        }
    }
}
