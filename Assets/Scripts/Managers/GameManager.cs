using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The only admissible instance of this singleton class.
    /// </summary>
    private static GameManager _instance;

    /// <summary>
    /// Property that provides access in a controlled manner to the instance of <c>GameManager</c>.
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    /// <summary>
    /// The <c>UIManager</c> component that handles the UI of the game.
    /// </summary>
    public UIManager UI { get; private set; }

    /// <summary>
    /// Returns the Player.
    /// </summary>
    public PlayerController Player { get; private set; }

    /// <summary>
    /// The main Camera.
    /// </summary>
    public Camera MainCamera { get; private set; }

    /// <summary>
    /// Returns whether the player is in Game Menu or not.
    /// </summary>
    public bool IsInGameMenu
    {
        get
        {
            return UI.GameMenuIsLoaded;
        }
    }

    private void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(this);

        UI = gameObject.AddComponent<UIManager>();

        MainCamera = new GameObject("Camera", typeof(Camera)).GetComponent<Camera>();
        MainCamera.backgroundColor = Color.black;
        UI.Setup();
        UI.LoadMainMenu();
    }

    private void Update()
    {
        if (UI.CurrentCanvas.worldCamera == null)
        {
            UI.CurrentCanvas.worldCamera = MainCamera;
        }

        if (!UI.MainMenuIsLoaded)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                UI.SwitchHUD();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                UI.SwitchGameMenu();
            }
        }

        if (Player != null)
        {
            Player.HasControl = !IsInGameMenu;
        }

        if (IsInGameMenu)
        {
            Time.timeScale = 0;
        } else
        {
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// This method loads the scene named <c>sceneName</c>, loading player and camera prefabs too.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    public IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        float activationProgress;

        UI.LoadSceneLoadingInfo();

        while (!sceneLoadingOperation.isDone)
        {
            activationProgress = Mathf.Clamp01(sceneLoadingOperation.progress / .9f);
            UI.SceneLoadingInfo.text = activationProgress.ToString();
            yield return null;
        }

        UI.UnloadSceneLoadingInfo();

        Vector3 playerPosition = GameObject.Find(GameFormulas.PlayerStartPositionName).transform.position;
        Vector3 cameraPosition = GameObject.Find(GameFormulas.CameraStartPositionName).transform.position;

        PlayerController playerController = 
            Instantiate(Resources.Load<PlayerController>(GameFormulas.PlayerResourcesPath), playerPosition, Quaternion.identity);
        CameraController cameraController = 
            Instantiate(Resources.Load<CameraController>(GameFormulas.CameraResourcesPath), cameraPosition, Quaternion.identity);

        cameraController.Target = playerController.transform;

        MainCamera = cameraController.CameraComponent;

        UI.LoadHUD();

        Player = playerController;
    }
}
