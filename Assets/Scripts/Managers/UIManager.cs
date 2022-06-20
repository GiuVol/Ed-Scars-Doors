using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// The only admissible instance of this singleton class.
    /// </summary>
    private static UIManager _instance;

    /// <summary>
    /// Property that provides access in a controlled manner to the instance of <c>UIManager</c>.
    /// </summary>
    public static UIManager Instance
    {
        get
        {
            return _instance;
        }
    }

    /// <summary>
    /// The <c>Canvas</c> that shows the UI elements.
    /// </summary>
    public Canvas CurrentCanvas { get; private set; }

    /// <summary>
    /// The <c>EventSystem</c> that allows to interact with the UI.
    /// </summary>
    public EventSystem CurrentEventSystem { get; private set; }

    /// <summary>
    /// The main menu. This field is null if the main menu is not loaded.
    /// </summary>
    public MainMenu MainMenu { get; private set; }

    /// <summary>
    /// This property returns whether the main menu is loaded or not.
    /// </summary>
    public bool MainMenuIsLoaded
    {
        get
        {
            return MainMenu != null;
        }
    }

    /// <summary>
    /// The HUD. This field is null if the HUD is not loaded.
    /// </summary>
    public HUD CurrentHUD { get; private set; }

    /// <summary>
    /// This property returns whether the HUD is loaded or not.
    /// </summary>
    public bool HUDIsLoaded
    {
        get
        {
            return CurrentHUD != null;
        }
    }

    /// <summary>
    /// The UI component that shows informations about the progress of the loading of a new scene.
    /// This field is null if no scene is loading.
    /// </summary>
    public TextMeshProUGUI SceneLoadingInfo { get; private set; }
    
    /// <summary>
    /// Stores whether the UIManager is initialized or not.
    /// </summary>
    private bool _initialized;
    
    /// <summary>
    /// This method initializes the UIManager.
    /// </summary>
    public void Setup()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        CurrentCanvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))
            .GetComponent<Canvas>();
        CurrentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CurrentCanvas.additionalShaderChannels -= AdditionalCanvasShaderChannels.Tangent;

        CurrentEventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule))
            .GetComponent<EventSystem>();

        DontDestroyOnLoad(CurrentCanvas.gameObject);
        DontDestroyOnLoad(CurrentEventSystem.gameObject);

        _instance = this;
        _initialized = true;
    }

    /// <summary>
    /// This method loads the main menu inside the canvas, if it's not already loaded.
    /// </summary>
    public void LoadMainMenu()
    {
        if (!_initialized)
        {
            return;
        }

        if (MainMenuIsLoaded)
        {
            return;
        }

        ClearCanvas();

        MainMenu = Instantiate(Resources.Load<MainMenu>(GameFormulas.MainMenuResourcesPath), CurrentCanvas.transform);

        MainMenu.PlayDemoButton.onClick.AddListener(
            delegate {
                Destroy(MainMenu.gameObject);
                MainMenu = null;
                StartCoroutine(GameManager.Instance.LoadScene("DemoInsects"));
            }
            );
    }

    /// <summary>
    /// This method enables/disables the HUD if it is disabled/enabled.
    /// </summary>
    public void SwitchHUD()
    {
        if (!_initialized)
        {
            return;
        }
        
        if (HUDIsLoaded)
        {
            UnloadHUD();
        } else
        {
            LoadHUD();
        }
    }

    /// <summary>
    /// This method loads the HUD inside the canvas, if it's not already loaded.
    /// </summary>
    public void LoadHUD()
    {
        if (!_initialized || HUDIsLoaded)
        {
            return;
        }

        CurrentHUD = Instantiate(Resources.Load<HUD>(GameFormulas.HUDResourcesPath), CurrentCanvas.transform);
    }

    /// <summary>
    /// This method deletes the HUD, if it is loaded.
    /// </summary>
    public void UnloadHUD()
    {
        if (!_initialized || !HUDIsLoaded)
        {
            return;
        }
        
        Destroy(CurrentHUD.gameObject);
        CurrentHUD = null;
    }

    /// <summary>
    /// This method loads the UI Component that shows the progress of a loading scene inside the canvas, if it's not already loaded.
    /// </summary>
    public void LoadSceneLoadingInfo()
    {
        if (!_initialized || SceneLoadingInfo != null)
        {
            return;
        }
        
        SceneLoadingInfo = Instantiate(Resources.Load<TextMeshProUGUI>(GameFormulas.SceneLoadingInfoResourcesPath), CurrentCanvas.transform);
    }

    /// <summary>
    /// This method deletes the UI Component that shows the progress of a loading scene inside the canvas, if it's not already loaded.
    /// </summary>
    public void UnloadSceneLoadingInfo()
    {
        if (!_initialized || SceneLoadingInfo == null)
        {
            return;
        }

        Destroy(SceneLoadingInfo.gameObject);
        SceneLoadingInfo = null;
    }

    /// <summary>
    /// This method deletes all the gameObjects inside the Canvas.
    /// </summary>
    public void ClearCanvas()
    {
        if (!_initialized)
        {
            return;
        }

        foreach (Transform canvasChild in CurrentCanvas.transform)
        {
            Destroy(canvasChild.gameObject);
        }

        MainMenu = null;
        CurrentHUD = null;
        SceneLoadingInfo = null;
    }
}
