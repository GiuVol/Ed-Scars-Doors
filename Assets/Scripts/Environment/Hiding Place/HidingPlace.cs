using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingPlace : MonoBehaviour
{
    private const string HideMessageResourcesPath = "UI/HideMessage";
    private const string GetOutMessageResourcesPath = "UI/GetOutFromHidingMessage";

    #region Serialized

    [SerializeField]
    private float _timeNeededToHide;

    [SerializeField]
    private float _timeNeededToGetOut;

    #endregion

    private PlayerController _player;

    private Coroutine _hideButtonPressingCoroutine;

    private float timer;
    
    private DynamicUIComponent _uiHideMessage;

    private UIBar _hideButtonHoldingBar;

    public float HideButtonHoldingProgress
    {
        get
        {
            float value = 0;

            if (_hideButtonHoldingBar != null)
            {
                value = _hideButtonHoldingBar.CurrentValue;
            }

            return value;
        }

        set
        {
            if (_hideButtonHoldingBar == null)
            {
                return;
            }

            _hideButtonHoldingBar.UpdateValue(value);
        }
    }
    
    private DynamicUIComponent _uiGetOutMessage;

    private UIBar _getOutButtonHoldingBar;
    
    public float GetOutButtonHoldingProgress
    {
        get
        {
            float value = 0;

            if (_getOutButtonHoldingBar != null)
            {
                value = _getOutButtonHoldingBar.CurrentValue;
            }

            return value;
        }

        set
        {
            if (_getOutButtonHoldingBar == null)
            {
                return;
            }

            _getOutButtonHoldingBar.UpdateValue(value);
        }
    }
    
    void Start()
    {
        Canvas currentCanvas = FindObjectOfType<Canvas>();
        
        if (currentCanvas != null)
        {
            _uiHideMessage = Instantiate(Resources.Load<DynamicUIComponent>(HideMessageResourcesPath),
                                                         currentCanvas.transform);
            _uiHideMessage.InitializeDynamic(transform, Vector3.zero);

            _uiHideMessage.gameObject.SetActive(false);

            _hideButtonHoldingBar = _uiHideMessage.GetComponentInChildren<UIBar>();

            if (_hideButtonHoldingBar != null)
            {
                _hideButtonHoldingBar.InitializeStatic(1);
                _hideButtonHoldingBar.UpdateValueInstantly(0);
            }

            _uiGetOutMessage = Instantiate(Resources.Load<DynamicUIComponent>(GetOutMessageResourcesPath),
                                                         currentCanvas.transform);
            _uiGetOutMessage.InitializeDynamic(transform, Vector3.zero);

            _uiGetOutMessage.gameObject.SetActive(false);

            _getOutButtonHoldingBar = _uiGetOutMessage.GetComponentInChildren<UIBar>();

            if (_getOutButtonHoldingBar != null)
            {
                _getOutButtonHoldingBar.InitializeStatic(1);
                _getOutButtonHoldingBar.UpdateValueInstantly(0);
            }
        }
    }

    private void Update()
    {
        if (_player == null)
        {
            _player = FindObjectOfType<PlayerController>();
        }

        if (_player == null)
        {
            return;
        }
        
        if (_player.CurrentHidingPlace == this)
        {
            if (InputHandler.Hide("Down") && (_player.CanHide || _player.IsHidden) && _hideButtonPressingCoroutine == null)
            {
                _hideButtonPressingCoroutine = StartCoroutine(HandleHideButton());
            }
        } else
        {
            return;
        }

        EnableHideMessage(_player.CanHide);
        EnableGetOutMessage(_player.IsHidden);
    }

    private IEnumerator HandleHideButton()
    {
        while (true)
        {
            if (_player == null || _player.CurrentHidingPlace != this)
            {
                timer = 0;
                ResetHideButtonHoldingBar();
                ResetGetOutButtonHoldingBar();
                _hideButtonPressingCoroutine = null;
                yield break;
            }

            if (InputHandler.Hide())
            {
                timer += Time.deltaTime;

                if (_player.CanHide)
                {
                    HideButtonHoldingProgress = timer / _timeNeededToHide;
                }

                if (_player.IsHidden)
                {
                    GetOutButtonHoldingProgress = timer / _timeNeededToGetOut;
                }

                float timeToWait = 0;

                if (_player.CanHide)
                {
                    timeToWait = _timeNeededToHide;
                }

                if (_player.IsHidden)
                {
                    timeToWait = _timeNeededToGetOut;
                }

                if (timer >= timeToWait)
                {
                    timer = 0;
                    break;
                }
            }
            else
            {
                timer = 0;
                ResetHideButtonHoldingBar();
                ResetGetOutButtonHoldingBar();
                _hideButtonPressingCoroutine = null;
                yield break;
            }

            yield return null;
        }

        ResetHideButtonHoldingBar();
        ResetGetOutButtonHoldingBar();

        timer = 0;

        if (_player.CanHide)
        {
            _player.Hide();
        }
        else
        {
            if (_player.IsHidden)
            {
                _player.GetOutOfHiding();
            }
        }

        _hideButtonPressingCoroutine = null;
    }
    
    public void EnableHideMessage(bool enabled)
    {
        if(_uiHideMessage == null)
        {
            return;
        }

        _uiHideMessage.gameObject.SetActive(enabled);
    }

    public void EnableGetOutMessage(bool enabled)
    {
        if (_uiGetOutMessage == null)
        {
            return;
        }

        _uiGetOutMessage.gameObject.SetActive(enabled);
    }
    
    public void ResetHideButtonHoldingBar()
    {
        if (_hideButtonHoldingBar != null)
        {
            _hideButtonHoldingBar.UpdateValueInstantly(0);
        }
    }

    public void ResetGetOutButtonHoldingBar()
    {
        if (_getOutButtonHoldingBar != null)
        {
            _getOutButtonHoldingBar.UpdateValueInstantly(0);
        }
    }
}
