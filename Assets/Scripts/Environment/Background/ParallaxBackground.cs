using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    #region Inner Classes

    /// <summary>
    /// The objects of this class represent the layers that compose the background.
    /// </summary>
    [System.Serializable]
    public class BackgroundLayer
    {
        #region Serialized fields

        /// <summary>
        /// The sprite of the layer.
        /// </summary>
        [SerializeField]
        private Sprite _layerSprite;

        /// <summary>
        /// The positional offset, relative to the camera, that this layer must have.
        /// </summary>
        [SerializeField]
        private Vector2 _positionOffset;
        
        /// <summary>
        /// The scale that the transform of this layer must have.
        /// </summary>
        [SerializeField]
        private Vector2 _transformScale;

        /// <summary>
        /// The size multiplier of the sprite renderer.
        /// </summary>
        [SerializeField]
        private Vector2 _rendererSizeMultiplier;

        /// <summary>
        /// Specifies if this layer must loop horizontally.
        /// </summary>
        [SerializeField]
        private bool _loopHorizontal;

        /// <summary>
        /// Specifies if this layer must loop vertically.
        /// </summary>
        [SerializeField]
        private bool _loopVertical;

        /// <summary>
        /// Specifies how fast this layer must follow the movement of the camera.
        /// </summary>
        [SerializeField]
        private Vector2 _parallaxMultiplier;
        
        /// <summary>
        /// Stores the position in which this layer must be rendered.
        /// </summary>
        [SerializeField]
        private int _sortingLayerIndex;

        #endregion

        /// <summary>
        /// Stores the gameObject of the layer.
        /// </summary>
        public GameObject GameObject { get; private set; }

        /// <summary>
        /// The positional offset, relative to the camera, that this layer must have.
        /// </summary>
        public Vector2 PositionOffset
        {
            get
            {
                return _positionOffset;
            }
        }
        
        /// <summary>
        /// Returns the component that renders the sprite of the layer.
        /// </summary>
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

        /// <summary>
        /// Returns the sprite of the layer.
        /// </summary>
        public Sprite LayerSprite
        {
            get
            {
                return _layerSprite;
            }
        }

        /// <summary>
        /// Returns the texture of the sprite.
        /// </summary>
        public Texture2D Tex2D
        {
            get
            {
                return _layerSprite.texture;
            }
        }
        
        /// <summary>
        /// Returns the width of the texture.
        /// </summary>
        public float TextureUnitSizeX
        {
            get
            {
                return (Tex2D.width / _layerSprite.pixelsPerUnit) * GameObject.transform.localScale.x;
            }
        }

        /// <summary>
        /// Returns the height of the texture.
        /// </summary>
        public float TextureUnitSizeY
        {
            get
            {
                return (Tex2D.height / _layerSprite.pixelsPerUnit) * GameObject.transform.localScale.y;
            }
        }

        /// <summary>
        /// Returns whether this layer must loop horizontally.
        /// </summary>
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

        /// <summary>
        /// Returns whether this layer must loop vertically.
        /// </summary>
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

        /// <summary>
        /// Specifies how fast this layer must follow the movement of the camera.
        /// </summary>
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

        /// <summary>
        /// Stores the position in which this layer must be rendered.
        /// </summary>
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

        /// <summary>
        /// Initializes and instantiates the layer with its gameobject.
        /// </summary>
        /// <param name="layerName">The name of the layer</param>
        /// <param name="desiredPosition">the start position</param>
        public void Initialize(string layerName, Vector3 desiredPosition)
        {
            GameObject = new GameObject(layerName);
            GameObject.transform.localScale = new Vector3(_transformScale.x, _transformScale.y, 1);
            GameObject.transform.position = desiredPosition + (Vector3) PositionOffset;

            if (LayerSprite == null)
            {
                return;
            }

            Renderer.sprite = LayerSprite;
            Renderer.sortingOrder = SortingLayerIndex;
            Renderer.drawMode = SpriteDrawMode.Tiled;

            if (!LoopHorizontal)
            {
                _rendererSizeMultiplier.x = 1;
            }

            if (!LoopVertical)
            {
                _rendererSizeMultiplier.y = 1;
            }

            Renderer.size = new Vector2(Renderer.size.x * _rendererSizeMultiplier.x, Renderer.size.y * _rendererSizeMultiplier.y);
        }
    }

    #endregion

    #region Serialized

    /// <summary>
    /// Stores all the layers that compose this background.
    /// </summary>
    [SerializeField]
    private List<BackgroundLayer> _layers;

    /// <summary>
    /// The z position of the game objects that will compose the background.
    /// </summary>
    [SerializeField]
    private int _zPosition;

    #endregion

    /// <summary>
    /// Returns all the layers that compose this background in a controlled manner.
    /// </summary>
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

    /// <summary>
    /// The camera that is currently rendering the scene.
    /// </summary>
    private Camera _camera;

    /// <summary>
    /// The position that the camera had on the previous frame.
    /// </summary>
    private Vector3 _lastCameraPosition;
    
    /// <summary>
    /// Initializes all the layers of this background.
    /// </summary>
    /// <param name="startPosition">The start position of the layers of this background</param>
    public void Initialize(Vector3 startPosition, Camera camera)
    {
        int counter = 0;

        foreach (BackgroundLayer layer in Layers)
        {
            counter++;
            layer.Initialize("BackgroundLayer" + counter,
                             new Vector3(startPosition.x, startPosition.y, _zPosition));
        }

        _camera = camera;
        _lastCameraPosition = _camera.transform.position;
    }

    private void LateUpdate()
    {
        Move();
    }

    /// <summary>
    /// Moves the background, relatively to the camera.
    /// </summary>
    public void Move()
    {
        if (Layers.Count <= 0 || _camera == null)
        {
            return;
        }

        Vector2 deltaPosition = _camera.transform.position - _lastCameraPosition;

        foreach (BackgroundLayer layer in Layers)
        {
            layer.GameObject.transform.position += new Vector3(deltaPosition.x, 0) * layer.ParallaxMultiplier.x;
            layer.GameObject.transform.position += new Vector3(0, deltaPosition.y) * layer.ParallaxMultiplier.y;

            if (layer.LayerSprite != null)
            {
                if (layer.LoopHorizontal)
                {
                    if (Mathf.Abs(_camera.transform.position.x - (layer.GameObject.transform.position.x + layer.PositionOffset.x))
                        >= layer.TextureUnitSizeX)
                    {
                        float offsetPositionX = (_camera.transform.position.x - layer.GameObject.transform.position.x) %
                            layer.TextureUnitSizeX;
                        layer.GameObject.transform.position =
                            new Vector3(_camera.transform.position.x + offsetPositionX,
                                        layer.GameObject.transform.position.y,
                                        _zPosition);
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
                            new Vector3(layer.GameObject.transform.position.x,
                                        _camera.transform.position.y + offsetPositionY,
                                        _zPosition);
                    }
                }
            }
        }

        _lastCameraPosition = _camera.transform.position;
    }
}
