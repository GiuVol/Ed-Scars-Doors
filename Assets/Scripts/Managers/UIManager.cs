using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static Vector2 ReferenceResolution = new Vector2(1024, 768);

    private const string MainMenuResourcesPath = "UI/MainMenu";
    private const string GameMenuResourcesPath = "UI/GameMenu";
    private const string HUDResourcesPath = "UI/HUD";
    private const string SceneLoadingInfoResourcesPath = "UI/SceneLoadingInfo";
    private const string GameOverMenuResourcesPath = "UI/GameOverMenu";

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
    /// The game menu. This field is null if the game menu is not loaded.
    /// </summary>
    public GameMenu GameMenu { get; private set; }

    /// <summary>
    /// This property returns whether the main menu is loaded or not.
    /// </summary>
    public bool GameMenuIsLoaded
    {
        get
        {
            return GameMenu != null;
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

    private bool _wantsHudLoaded;

    /// <summary>
    /// The UI component that shows informations about the progress of the loading of a new scene.
    /// This field is null if no scene is loading.
    /// </summary>
    public TextMeshProUGUI SceneLoadingInfo { get; private set; }

    public bool PromptIsLoaded { get; set; }

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
            Destroy(this);
            return;
        }

        CurrentCanvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))
            .GetComponent<Canvas>();
        CurrentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler canvasScaler = CurrentCanvas.GetComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        canvasScaler.referenceResolution = ReferenceResolution;

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

        MainMenu mainMenuResource = Resources.Load<MainMenu>(MainMenuResourcesPath);

        if (mainMenuResource != null)
        {
            MainMenu = Instantiate(mainMenuResource, CurrentCanvas.transform);
        }

        if (MainMenu != null && MainMenu.PlayDemoButton != null)
        {
            MainMenu.PlayDemoButton.onClick.AddListener(
                delegate {
                    Destroy(MainMenu.gameObject);
                    MainMenu = null;

                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.StartCoroutine(GameManager.Instance.LoadScene("Corridor"));
                    }
                }
                );
        }
    }

    /// <summary>
    /// This method enables/disables the game menu if it is disabled/enabled.
    /// </summary>
    /// <returns>The current state of the game menu, if it's active or not</returns>
    public bool SwitchGameMenu()
    {
        if (!_initialized)
        {
            return false;
        }

        if (GameMenuIsLoaded)
        {
            UnloadGameMenu();
        } else
        {
            LoadGameMenu();
        }

        return GameMenuIsLoaded;
    }

    /// <summary>
    /// This method loads the game menu inside the canvas, if it's not already loaded.
    /// </summary>
    public void LoadGameMenu()
    {
        if (!_initialized)
        {
            return;
        }

        if (GameMenuIsLoaded)
        {
            return;
        }

        ClearCanvas();

        GameMenu = Instantiate(Resources.Load<GameMenu>(GameMenuResourcesPath), CurrentCanvas.transform);
    }

    /// <summary>
    /// This method unloads the game menu, if it's already loaded.
    /// </summary>
    public void UnloadGameMenu()
    {
        if (!_initialized || !GameMenuIsLoaded)
        {
            return;
        }

        Destroy(GameMenu.gameObject);
        GameMenu = null;

        if (_wantsHudLoaded)
        {
            LoadHUD();
        } else
        {
            UnloadHUD();
        }
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
        if (!_initialized || HUDIsLoaded || GameMenuIsLoaded)
        {
            return;
        }

        CurrentHUD = Instantiate(Resources.Load<HUD>(HUDResourcesPath), CurrentCanvas.transform);

        if (GameManager.Instance != null)
        {
            PlayerController player = GameManager.Instance.Player;

            if (CurrentHUD != null && player != null)
            {
                if (CurrentHUD.PlayerHealthBar != null)
                {
                    CurrentHUD.PlayerHealthBar.InitializeStatic(player.Health.MaxHealth, "HP");
                    CurrentHUD.PlayerHealthBar.UpdateValueInstantly(player.Health.CurrentHealth);
                }

                if (CurrentHUD.PlayerCorrosionBar != null)
                {
                    CurrentHUD.PlayerCorrosionBar.InitializeStatic(player.Status.MaxCorrosionTime, "CR");
                    CurrentHUD.PlayerCorrosionBar.UpdateValueInstantly(player.Status.CorrosionTimeLeft);
                }

                if (CurrentHUD.PlayerBlindnessBar != null)
                {
                    CurrentHUD.PlayerBlindnessBar.InitializeStatic(player.Status.MaxBlindnesslevel, "BL");
                    CurrentHUD.PlayerBlindnessBar.UpdateValueInstantly(player.Status.CurrentBlindnesslevel);
                }
            }
        }

        _wantsHudLoaded = true;
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

        _wantsHudLoaded = false;
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
        
        SceneLoadingInfo = Instantiate(Resources.Load<TextMeshProUGUI>(SceneLoadingInfoResourcesPath), CurrentCanvas.transform);
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
    /// This method loads the gameover menu inside the canvas, if it's not already loaded.
    /// </summary>
    /// <param name="textValue">The message that will appear on the gameover menu</param>
    public void LoadGameOverMenu(string textValue)
    {
        if (!_initialized)
        {
            return;
        }

        ClearCanvas();

        GameOverMenu gameOverMenuResource = Resources.Load<GameOverMenu>(GameOverMenuResourcesPath);

        if (gameOverMenuResource != null)
        {
            GameOverMenu gameOverMenu = Instantiate(gameOverMenuResource, CurrentCanvas.transform);
            gameOverMenu.GameOverTextValue = textValue;
        }
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

        if (MainMenu != null)
        {
            Destroy(MainMenu.gameObject);
            MainMenu = null;
        }

        if (GameMenu != null)
        {
            Destroy(GameMenu.gameObject);
            GameMenu = null;
        }

        if (CurrentHUD != null)
        {
            Destroy(CurrentHUD.gameObject);
            CurrentHUD = null;
        }

        if (SceneLoadingInfo != null)
        {
            Destroy(SceneLoadingInfo.gameObject);
            SceneLoadingInfo = null;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null)
        {
            PlayerController player = GameManager.Instance.Player;

            if (HUDIsLoaded && player != null)
            {
                if (CurrentHUD.PlayerHealthBar != null)
                {
                    CurrentHUD.PlayerHealthBar.UpdateValue(player.Health.CurrentHealth);
                }

                if (CurrentHUD.PlayerCorrosionBar != null)
                {
                    if (player.Status.CorrosionTimeLeft <= 0)
                    {
                        if (CurrentHUD.PlayerCorrosionBar.gameObject.activeSelf)
                        {
                            CurrentHUD.PlayerCorrosionBar.gameObject.SetActive(false);
                        }
                    } else
                    {
                        if (!CurrentHUD.PlayerCorrosionBar.gameObject.activeSelf)
                        {
                            CurrentHUD.PlayerCorrosionBar.gameObject.SetActive(true);
                        }
                    }

                    CurrentHUD.PlayerCorrosionBar.UpdateValue(player.Status.CorrosionTimeLeft);
                }

                if (CurrentHUD.PlayerBlindnessBar != null)
                {
                    if (player.Status.CurrentBlindnesslevel <= 0)
                    {
                        if (CurrentHUD.PlayerBlindnessBar.gameObject.activeSelf)
                        {
                            CurrentHUD.PlayerBlindnessBar.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        if (!CurrentHUD.PlayerBlindnessBar.gameObject.activeSelf)
                        {
                            CurrentHUD.PlayerBlindnessBar.gameObject.SetActive(true);
                        }
                    }
                    
                    CurrentHUD.PlayerBlindnessBar.UpdateValue(player.Status.CurrentBlindnesslevel);
                }
            }
        }
    }
}
