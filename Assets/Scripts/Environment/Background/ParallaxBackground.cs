using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundLayer
    {
        [SerializeField]
        private Sprite _layerSprite;

        [SerializeField]
        private Vector2 _positionOffset;
        
        [SerializeField]
        private Vector2 _transformScale;

        [SerializeField]
        private Vector2 _rendererSizeMultiplier;

        [SerializeField]
        private bool _loopHorizontal;

        [SerializeField]
        private bool _loopVertical;

        [SerializeField]
        private Vector2 _parallaxMultiplier;

        [SerializeField]
        private int _sortingLayerIndex;
        
        public GameObject GameObject { get; private set; }

        public Vector2 PositionOffset
        {
            get
            {
                return _positionOffset;
            }
        }
        
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

        public Vector2 BaseRendererSize { get; private set; }

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
            GameObject.transform.position = desiredPosition + (Vector3) PositionOffset;
            Renderer.sprite = LayerSprite;
            Renderer.sortingOrder = SortingLayerIndex;
            Renderer.drawMode = SpriteDrawMode.Tiled;
            BaseRendererSize = Renderer.size;

            if (!LoopHorizontal)
            {
                _rendererSizeMultiplier.x = 1;
            }

            if (!LoopVertical)
            {
                _rendererSizeMultiplier.y = 1;
            }

            Renderer.size = new Vector2(BaseRendererSize.x * _rendererSizeMultiplier.x, BaseRendererSize.y * _rendererSizeMultiplier.y);
        }
    }

    [SerializeField]
    private List<BackgroundLayer> _layers;
    [SerializeField]
    private int _zPosition;

    public List<BackgroundLayer> Layers
    {
        get
        {
            if (_layers == null)
            {
                _layers = new List<BackgroundLayer>();
            }

            return _layers;
        }
    }
    
    public void Initialize(Vector3 startPosition)
    {
        int counter = 0;

        foreach (BackgroundLayer layer in _layers)
        {
            counter++;
            layer.Initialize("BackgroundLayer" + counter,
                             new Vector3(startPosition.x, startPosition.y, _zPosition));
        }
    }
}
