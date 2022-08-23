using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regia : MonoBehaviour
{
    [System.Serializable]
    private class CameraPresetValue : IComparable
    {
        [SerializeField]
        private float _xPosition;

        [SerializeField]
        private float _cameraSize;

        [SerializeField]
        private Vector2 _cameraOffset;

        public float XPosition
        {
            get
            {
                return _xPosition;
            }
        }

        public float CameraSize
        {
            get
            {
                return _cameraSize;
            }
        }

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

    [SerializeField]
    private ParallaxBackground _background;

    [SerializeField]
    private List<CameraPresetValue> _cameraValues;

    private CameraController _cameraController;
    private Vector3 _lastCameraPosition;

    private PlayerController _playerController;

    private bool _initialized;

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

    public void StartRegia(CameraController cameraController, PlayerController playerController)
    {
        if (cameraController == null)
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
            HandleCamera();
        }
    }

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

    private void HandleCamera()
    {
        if (_cameraValues == null || _cameraValues.Count <= 0)
        {
            return;
        }

        float playerXPosition = _playerController.transform.position.x;

        float cameraSize = 10;
        Vector2 cameraOffset = Vector2.zero;

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

        _cameraController.CameraComponent.orthographicSize = cameraSize;
        _cameraController.PositionOffset = cameraOffset;
    }
}
