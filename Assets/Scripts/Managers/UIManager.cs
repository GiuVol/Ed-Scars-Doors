using System.Collections;
using System.Collections.Generic;
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

    public bool IsInMainMenu
    {
        get
        {
            return MainMenu != null;
        }
    }

    public TextMeshProUGUI LoadingInfos { get; private set; }

    public HUD CurrentHUD { get; private set; }

    public bool HUDIsLoaded
    {
        get
        {
            return CurrentHUD != null;
        }
    }
    
    public void Setup()
    {
        CurrentCanvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))
            .GetComponent<Canvas>();
        CurrentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CurrentCanvas.additionalShaderChannels -= AdditionalCanvasShaderChannels.Tangent;

        CurrentEventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule))
            .GetComponent<EventSystem>();

        LoadingInfos = Instantiate(Resources.Load<TextMeshProUGUI>("UI/Loading"), CurrentCanvas.transform);
        LoadingInfos.gameObject.SetActive(false);

        DontDestroyOnLoad(CurrentCanvas.gameObject);
        DontDestroyOnLoad(CurrentEventSystem.gameObject);
    }

    public void LoadMainMenu()
    {
        if (MainMenu != null)
        {
            Destroy(MainMenu);
        }

        MainMenu = Instantiate(Resources.Load<MainMenu>("UI/MainMenu"), CurrentCanvas.transform);

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
        if (HUDIsLoaded)
        {
            return;
        }

        CurrentHUD = Instantiate(Resources.Load<HUD>("UI/HUD"), CurrentCanvas.transform);
    }

    public void UnloadHUD()
    {
        if (!HUDIsLoaded)
        {
            return;
        }
        
        Destroy(CurrentHUD.gameObject);
        CurrentHUD = null;
    }
}
