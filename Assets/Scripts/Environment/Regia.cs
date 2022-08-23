using System;
using System.Collections.Generic;
using UnityEngine;

public class Regia : MonoBehaviour
{
    /// <summary>
    /// The objects of this class specify the configuration that the camera controller must have 
    /// when the player reaches a certain position on the x axis.
    /// </summary>
    [System.Serializable]
    private class CameraPresetValue : IComparable
    {
        /// <summary>
        /// The x position at which this preset has the max weight.
        /// </summary>
        [SerializeField]
        private float _xPosition;

        /// <summary>
        /// The ortographic size that the camera must have when the player reaches the specified x position.
        /// </summary>
        [SerializeField]
        private float _cameraSize;

        /// <summary>
        /// The position, relative to the player, that the camera must have when the player 
        /// reaches the specified x position.
        /// </summary>
        [SerializeField]
        private Vector2 _cameraOffset;

        /// <summary>
        /// The x position at which this preset has the max weight.
        /// </summary>
        public float XPosition
        {
            get
            {
                return _xPosition;
            }
        }

        /// <summary>
        /// The ortographic size that the camera must have when the player reaches the specified x position.
        /// </summary>
        public float CameraSize
        {
            get
            {
                return _cameraSize;
            }
        }

        /// <summary>
        /// The position, relative to the player, that the camera must have when the player 
        /// reaches the specified x position.
        /// </summary>
        public Vector2 CameraOffset
        {
            get
            {
                return _cameraOffset;
            }
        }

        public int CompareTo(object other)
        {
            if (!(other is CameraPresetValue))
            {
                return -1;
            }

            CameraPresetValue otherCPV = (CameraPresetValue) other;
            return _xPosition.CompareTo(otherCPV._xPosition);
        }
    }

    #region Serialized

    /// <summary>
    /// The current parallax background, that the regia must handle.
    /// </summary>
    [SerializeField]
    private ParallaxBackground _background;

    /// <summary>
    /// The camera configuration presets of the stage.
    /// </summary>
    [SerializeField]
    private List<CameraPresetValue> _cameraValues;

    #endregion

    /// <summary>
    /// The camera controller that is currently controlling the camera.
    /// </summary>
    private CameraController _cameraController;

    /// <summary>
    /// The position that the camera had on the previous frame.
    /// </summary>
    private Vector3 _lastCameraPosition;

    /// <summary>
    /// The current player controller.
    /// </summary>
    private PlayerController _playerController;

    /// <summary>
    /// Specifies whether the regia component is initialized.
    /// </summary>
    private bool _initialized;

    #region Debug

    private void Update()
    {
        if (_initialized)
        {
            return;
        }

        CameraController cc = GameObject.FindObjectOfType<CameraController>();
        PlayerController pc = GameObject.FindObjectOfType<PlayerController>();

        if (cc != null && pc != null)
        {
            StartRegia(cc, pc);
        }
    }

    #endregion

    /// <summary>
    /// Method that initializes the regia component.
    /// </summary>
    /// <param name="cameraController">The camera controller that will be active on the stage</param>
    /// <param name="playerController">the current player controller</param>
    public void StartRegia(CameraController cameraController, PlayerController playerController)
    {
        if (cameraController == null || playerController == null)
        {
            return;
        }

        _cameraController = cameraController;
        _lastCameraPosition = _cameraController.transform.position;

        _playerController = playerController;

        if (_background != null)
        {
            _background.Initialize(_cameraController.transform.position);
        }

        if (_cameraValues != null && _cameraValues.Count > 0)
        {
            _cameraValues.Sort();
        }

        _initialized = true;
    }

    void LateUpdate()
    {
        if (!_initialized)
        {
            return;
        }

        if (_background != null)
        {
            HandleBackground();
        }

        if (_cameraValues != null && _cameraValues.Count > 0)
        {
            HandleCamera(_playerController.transform.position.x);
        }
    }

    /// <summary>
    /// Moves the background, relatively to the camera.
    /// </summary>
    private void HandleBackground()
    {
        Vector3 deltaPosition = _cameraController.transform.position - _lastCameraPosition;

        foreach (ParallaxBackground.BackgroundLayer layer in _background.Layers)
        {
            Vector3 offset = deltaPosition;

            layer.GameObject.transform.position += new Vector3(offset.x, 0) * layer.ParallaxMultiplier.x;
            layer.GameObject.transform.position += new Vector3(0, offset.y) * layer.ParallaxMultiplier.y;

            if (layer.LoopHorizontal)
            {
                if (Mathf.Abs(_cameraController.transform.position.x - (layer.GameObject.transform.position.x + layer.PositionOffset.x)) 
                    >= layer.TextureUnitSizeX)
                {
                    float offsetPositionX = (_cameraController.transform.position.x - layer.GameObject.transform.position.x) %
                        layer.TextureUnitSizeX;
                    layer.GameObject.transform.position =
                        new Vector3(_cameraController.transform.position.x + offsetPositionX, layer.GameObject.transform.position.y);
                }
            }

            if (layer.LoopVertical)
            {
                if (Mathf.Abs(_cameraController.transform.position.y - (layer.GameObject.transform.position.y + layer.PositionOffset.y)) 
                    >= layer.TextureUnitSizeY)
                {
                    float offsetPositionY = (_cameraController.transform.position.y - layer.GameObject.transform.position.y) %
                        layer.TextureUnitSizeY;
                    layer.GameObject.transform.position =
                        new Vector3(layer.GameObject.transform.position.x, _cameraController.transform.position.y + offsetPositionY);
                }
            }
        }

        _lastCameraPosition = _cameraController.transform.position;
    }

    /// <summary>
    /// This procedure modifies the camera controller's configuration according to the camera presets values serialized.
    /// </summary>
    /// <param name="playerXPosition">
    /// The current x position of the player.
    /// </param>
    private void HandleCamera(float playerXPosition)
    {
        if (_cameraValues == null || _cameraValues.Count <= 0)
        {
            return;
        }

        float cameraSize = _cameraController.OrthographicSize;
        Vector2 cameraOffset = _cameraController.PositionOffset;

        #region Calculating size and offset

        if (playerXPosition <= _cameraValues[0].XPosition)
        {
            cameraSize = _cameraValues[0].CameraSize;
            cameraOffset = _cameraValues[0].CameraOffset;
        } else if (playerXPosition >= _cameraValues[_cameraValues.Count - 1].XPosition)
        {
            cameraSize = _cameraValues[_cameraValues.Count - 1].CameraSize;
            cameraOffset = _cameraValues[_cameraValues.Count - 1].CameraOffset;
        } else
        {
            for (int i = 0; i < _cameraValues.Count - 1; i++)
            {
                CameraPresetValue current = _cameraValues[i];
                CameraPresetValue next = _cameraValues[i + 1];

                if (playerXPosition >= current.XPosition && playerXPosition <= next.XPosition)
                {
                    float relativePosition = playerXPosition - current.XPosition;
                    float interval = Mathf.Abs(next.XPosition - current.XPosition);

                    float lerpFactor = relativePosition / interval;

                    cameraSize = 
                        Mathf.Lerp(current.CameraSize, next.CameraSize, lerpFactor);
                    cameraOffset =
                        Vector2.Lerp(current.CameraOffset, next.CameraOffset, lerpFactor);

                    break;
                }
            }
        }

        #endregion

        _cameraController.OrthographicSize = cameraSize;
        _cameraController.PositionOffset = cameraOffset;
    }
}
