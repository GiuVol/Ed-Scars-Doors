using System;
using System.Collections.Generic;
using UnityEngine;

public class Regia : MonoBehaviour
{
    #region Inner Classes

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

    #endregion

    #region Serialized

    /// <summary>
    /// The current parallax background, that the regia must handle.
    /// </summary>
    [SerializeField]
    private ParallaxBackground _backgroundPrefab;

    /// <summary>
    /// The camera configuration presets of the stage.
    /// </summary>
    [SerializeField]
    private List<CameraPresetValue> _cameraValues;

    /// <summary>
    /// The ost of the level.
    /// </summary>
    [SerializeField]
    private AudioClip _ostClip;

    /// <summary>
    /// The ost of the level.
    /// </summary>
    public AudioClip OstClip
    {
        get
        {
            return _ostClip;
        }
    }

    /// <summary>
    /// The ambience sound of the level.
    /// </summary>
    [SerializeField]
    private AudioClip _ambienceClip;

    /// <summary>
    /// The ambience sound of the level.
    /// </summary>
    public AudioClip AmbienceClip
    {
        get
        {
            return _ambienceClip;
        }
    }

    [SerializeField]
    private float _ostVolume;

    public float OstVolume
    {
        get
        {
            return _ostVolume;
        }
    }

    [SerializeField]
    private float _ambienceVolume;

    public float AmbienceVolume
    {
        get
        {
            return _ambienceVolume;
        }
    }

    #endregion

    /// <summary>
    /// The camera controller that is currently controlling the camera.
    /// </summary>
    private CameraController _cameraController;

    /// <summary>
    /// The current player controller.
    /// </summary>
    private PlayerController _playerController;

    /// <summary>
    /// The current instance of the background prefab.
    /// </summary>
    private ParallaxBackground Background { get; set; }

    /// <summary>
    /// Returns the camera configuration presets of the stage in a controlled manner.
    /// </summary>
    private List<CameraPresetValue> CameraValues
    {
        get
        {
            if (_cameraValues == null)
            {
                _cameraValues = new List<CameraPresetValue>();
            }

            return _cameraValues;
        }
    }

    /// <summary>
    /// Specifies whether the regia component is initialized.
    /// </summary>
    private bool _initialized;

    /// <summary>
    /// Specifies whether the regia component is disabled.
    /// </summary>
    private bool _disabled;

    /// <summary>
    /// Specifies whether the regia component is disabled.
    /// </summary>
    public bool Disabled
    {
        get
        {
            return _disabled;
        }

        set
        {
            _disabled = value;
        }
    }
    
    #region Debug

    private void Update()
    {
        if (_initialized)
        {
            return;
        }

        CameraController cc = GameObject.FindObjectOfType<CameraController>();
        Camera camera = null;

        if (cc != null)
        {
            camera = cc.CameraComponent;
        }

        PlayerController pc = GameObject.FindObjectOfType<PlayerController>();

        if (cc != null && camera != null && pc != null)
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
        _playerController = playerController;

        if (_backgroundPrefab != null)
        {
            Background = Instantiate(_backgroundPrefab, transform.position, Quaternion.identity);
            Background.name = "ParallaxBackground";
            Background.transform.parent = transform;
            Background.transform.localPosition = Vector3.zero;
            Background.Initialize(_cameraController.transform.position, _cameraController.CameraComponent);
        }

        if (CameraValues.Count > 0)
        {
            CameraValues.Sort();
        }

        _initialized = true;

        ElegantManSpawner[] emSpawners = FindObjectsOfType<ElegantManSpawner>();

        if (emSpawners.Length > 0)
        {
            int selectedSpawner = UnityEngine.Random.Range(0, emSpawners.Length);

            for (int i = 0; i < emSpawners.Length; i++)
            {
                if (i != selectedSpawner)
                {
                    emSpawners[i].Disabled = true;
                    Destroy(emSpawners[i].gameObject);
                }
            }
        }
    }

    void LateUpdate()
    {
        if (!_initialized || _disabled || _playerController == null || _playerController.Health.IsDead)
        {
            return;
        }

        if (CameraValues.Count > 0)
        {
            HandleCamera(_playerController.transform.position.x);
        }
    }

    /// <summary>
    /// This procedure modifies the camera controller's configuration according to the camera presets values serialized.
    /// </summary>
    /// <param name="playerXPosition">
    /// The current x position of the player.
    /// </param>
    private void HandleCamera(float playerXPosition)
    {
        if (CameraValues.Count <= 0)
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

                    if (interval == 0)
                    {
                        cameraSize = next.CameraSize;
                        cameraOffset = next.CameraOffset;
                        break;
                    }

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
