using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    private class BackgroundLayer
    {
        public GameObject GameObject { get; private set; }

        public SpriteRenderer Renderer
        {
            get
            {
                SpriteRenderer renderer = null;

                if (GameObject != null)
                {
                    if (GameObject.GetComponent<SpriteRenderer>() == null)
                    {
                        GameObject.AddComponent<SpriteRenderer>();
                    }

                    renderer = GameObject.GetComponent<SpriteRenderer>();
                }

                return renderer;
            }
        }

        [SerializeField]
        private Sprite _layerSprite;

        public Vector2 BaseSize { get; private set; }

        [SerializeField]
        private Vector2 _transformScale;
        
        [SerializeField]
        private Vector2 _rendererSizeMultiplier;

        [SerializeField]
        private bool _loopHorizontal;

        public bool LoopHorizontal
        {
            get
            {
                return _loopHorizontal;
            }

            set
            {
                _loopHorizontal = value;
            }
        }

        [SerializeField]
        private bool _loopVertical;

        public bool LoopVertical
        {
            get
            {
                return _loopVertical;
            }

            set
            {
                _loopVertical = value;
            }
        }
        
        public Texture2D Tex2D
        {
            get
            {
                return _layerSprite.texture;
            }
        }

        public float TextureUnitSizeX
        {
            get
            {
                return (Tex2D.width / _layerSprite.pixelsPerUnit) * GameObject.transform.localScale.x;
            }
        }

        public float TextureUnitSizeY
        {
            get
            {
                return (Tex2D.height / _layerSprite.pixelsPerUnit) * GameObject.transform.localScale.y;
            }
        }
        
        public Sprite LayerSprite
        {
            get
            {
                return _layerSprite;
            }
        }

        [SerializeField]
        private Vector2 _parallaxMultiplier;

        public Vector2 ParallaxMultiplier
        {
            get
            {
                return _parallaxMultiplier;
            }

            set
            {
                _parallaxMultiplier = value;
            }
        }

        [SerializeField]
        private int _sortingLayerIndex;

        public int SortingLayerIndex
        {
            get
            {
                return _sortingLayerIndex;
            }

            set
            {
                _sortingLayerIndex = value;
            }
        }

        public void Initialize(string layerName, Vector3 desiredPosition)
        {
            GameObject = new GameObject(layerName);
            GameObject.transform.localScale = new Vector3(_transformScale.x, _transformScale.y, 1);
            GameObject.transform.position = desiredPosition;
            Renderer.sprite = LayerSprite;
            Renderer.sortingOrder = SortingLayerIndex;
            Renderer.drawMode = SpriteDrawMode.Tiled;
            BaseSize = Renderer.size;

            if (!LoopHorizontal)
            {
                _rendererSizeMultiplier.x = 1;
            }

            if (!LoopVertical)
            {
                _rendererSizeMultiplier.y = 1;
            }

            Renderer.size = new Vector2(BaseSize.x * _rendererSizeMultiplier.x, BaseSize.y * _rendererSizeMultiplier.y);
        }
    }

    [SerializeField]
    private List<BackgroundLayer> _layers;
    [SerializeField]
    private int _zPosition;
    
    private Transform _cameraTransform;
    private Vector3 _lastCameraPosition;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _lastCameraPosition = _cameraTransform.position;

        int counter = 0;

        foreach (BackgroundLayer layer in _layers)
        {
            counter++;
            layer.Initialize("BackgroundLayer" + counter, 
                             new Vector3(_cameraTransform.position.x, _cameraTransform.position.y, _zPosition));
        }
    }

    void LateUpdate()
    {
        Vector3 deltaPosition = _cameraTransform.position - _lastCameraPosition;

        foreach (BackgroundLayer layer in _layers)
        {
            Vector3 offset = deltaPosition;
            
            layer.GameObject.transform.position += new Vector3(offset.x, 0) * layer.ParallaxMultiplier.x;
            layer.GameObject.transform.position += new Vector3(0, offset.y) * layer.ParallaxMultiplier.y;

            Vector2 layerSize = layer.BaseSize;

            if (layer.LoopHorizontal)
            {
                layerSize.x = 3 * layer.BaseSize.x * Camera.main.orthographicSize;

                if (Mathf.Abs(_cameraTransform.position.x - layer.GameObject.transform.position.x) >= layer.TextureUnitSizeX)
                {
                    float offsetPositionX = (_cameraTransform.position.x - layer.GameObject.transform.position.x) %
                        layer.TextureUnitSizeX;
                    layer.GameObject.transform.position =
                        new Vector3(_cameraTransform.position.x + offsetPositionX, layer.GameObject.transform.position.y);
                }
            }

            if (layer.LoopVertical)
            {
                layerSize.y = 3 * layer.BaseSize.y * Camera.main.orthographicSize;

                if (Mathf.Abs(_cameraTransform.position.y - layer.GameObject.transform.position.y) >= layer.TextureUnitSizeY)
                {
                    float offsetPositionY = (_cameraTransform.position.y - layer.GameObject.transform.position.y) %
                        layer.TextureUnitSizeY;
                    layer.GameObject.transform.position =
                        new Vector3(layer.GameObject.transform.position.x, _cameraTransform.position.y + offsetPositionY);
                }
            }

            layer.GameObject.GetComponent<SpriteRenderer>().size = layerSize;
        }
        
        _lastCameraPosition = _cameraTransform.position;
    }
}
