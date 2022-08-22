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
        private float _cameraOffset;

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

        public float CameraOffset
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

    private Camera _camera;
    private Vector3 _lastCameraPosition;

    private bool _initialized;

    private void Start()
    {
        StartRegia(Camera.main);
    }

    public void StartRegia(Camera camera)
    {
        if (camera == null)
        {
            return;
        }

        _camera = camera;
        _lastCameraPosition = _camera.transform.position;

        if (_background != null)
        {
            _background.Initialize(_camera.transform.position);
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

        }
    }

    private void HandleBackground()
    {
        Vector3 deltaPosition = _camera.transform.position - _lastCameraPosition;

        foreach (ParallaxBackground.BackgroundLayer layer in _background.Layers)
        {
            Vector3 offset = deltaPosition;

            layer.GameObject.transform.position += new Vector3(offset.x, 0) * layer.ParallaxMultiplier.x;
            layer.GameObject.transform.position += new Vector3(0, offset.y) * layer.ParallaxMultiplier.y;

            if (layer.LoopHorizontal)
            {
                if (Mathf.Abs(_camera.transform.position.x - (layer.GameObject.transform.position.x + layer.PositionOffset.x)) 
                    >= layer.TextureUnitSizeX)
                {
                    float offsetPositionX = (_camera.transform.position.x - layer.GameObject.transform.position.x) %
                        layer.TextureUnitSizeX;
                    layer.GameObject.transform.position =
                        new Vector3(_camera.transform.position.x + offsetPositionX, layer.GameObject.transform.position.y);
                }
            }

            if (layer.LoopVertical)
            {
                if (Mathf.Abs(_camera.transform.position.y - (layer.GameObject.transform.position.y + layer.PositionOffset.y)) 
                    >= layer.TextureUnitSizeY)
                {
                    float offsetPositionY = (_camera.transform.position.y - layer.GameObject.transform.position.y) %
                        layer.TextureUnitSizeY;
                    layer.GameObject.transform.position =
                        new Vector3(layer.GameObject.transform.position.x, _camera.transform.position.y + offsetPositionY);
                }
            }
        }

        _lastCameraPosition = _camera.transform.position;
    }
}
