using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private UIBar _playerHealthBar;
    [SerializeField]
    private UIBar _playerBlindnessBar;
    [SerializeField]
    private UIBar _playerCorrosionBar;

    public UIBar PlayerHealthBar
    {
        get
        {
            return _playerHealthBar;
        }
    }

    public UIBar PlayerBlindnessBar
    {
        get
        {
            return _playerBlindnessBar;
        }
    }

    public UIBar PlayerCorrosionBar
    {
        get
        {
            return _playerCorrosionBar;
        }
    }
}
