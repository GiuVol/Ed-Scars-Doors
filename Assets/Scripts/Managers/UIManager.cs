using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIManager();
            }

            return _instance;
        }
    }

    public Canvas CurrentCanvas { get; private set; }

    public EventSystem CurrentEventSystem { get; private set; }

    public MainMenu MainMenu { get; private set; }

    public bool MainMenuIsLoaded
    {
        get
        {
            return MainMenu != null;
        }
    }

    public HUD CurrentHUD { get; private set; }

    public bool HUDIsLoaded
    {
        get
        {
            return CurrentHUD != null;
        }
    }

    public TextMeshProUGUI LoadingInfo { get; private set; }
    
    private bool _initialized;
    
    public void Setup()
    {
        CurrentCanvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))
            .GetComponent<Canvas>();
        CurrentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CurrentCanvas.additionalShaderChannels -= AdditionalCanvasShaderChannels.Tangent;

        CurrentEventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule))
            .GetComponent<EventSystem>();

        DontDestroyOnLoad(CurrentCanvas.gameObject);
        DontDestroyOnLoad(CurrentEventSystem.gameObject);

        _initialized = true;
    }

    public void LoadMainMenu()
    {
        if (!_initialized)
        {
            return;
        }

        if (MainMenu != null)
        {
            Destroy(MainMenu);
        }

        MainMenu = Instantiate(Resources.Load<MainMenu>(GameFormulas.MainMenuResourcesPath), CurrentCanvas.transform);

        MainMenu.PlayDemoButton.onClick.AddListener(
            delegate {
                Destroy(MainMenu.gameObject);
                MainMenu = null;
                StartCoroutine(GameManager.Instance.LoadScene("DemoInsects"));
            }
            );
    }

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

    public void LoadHUD()
    {
        if (!_initialized || HUDIsLoaded)
        {
            return;
        }

        CurrentHUD = Instantiate(Resources.Load<HUD>(GameFormulas.HUDResourcesPath), CurrentCanvas.transform);
    }

    public void UnloadHUD()
    {
        if (!_initialized || !HUDIsLoaded)
        {
            return;
        }
        
        Destroy(CurrentHUD.gameObject);
        CurrentHUD = null;
    }

    public void LoadSceneLoadingInfo()
    {
        if (!_initialized || LoadingInfo != null)
        {
            return;
        }
        
        LoadingInfo = 
            Instantiate(Resources.Load<TextMeshProUGUI>(GameFormulas.SceneLoadingInfoResourcesPath), CurrentCanvas.transform);
    }

    public void UnloadSceneLoadingInfo()
    {
        if (!_initialized || LoadingInfo == null)
        {
            return;
        }

        Destroy(LoadingInfo.gameObject);
        LoadingInfo = null;
    }
}
