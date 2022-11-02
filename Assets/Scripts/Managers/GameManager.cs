using Pathfinding;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string PlayerResourcesPath = "Player/Player";
    private const string CameraResourcesPath = "Player/MainCamera";

    private const string PlayerStartPositionName = "PlayerStartPosition";
    private const string CameraStartPositionName = "CameraStartPosition";

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
    /// Returns the Player.
    /// </summary>
    public PlayerController Player { get; private set; }

    /// <summary>
    /// The main Camera.
    /// </summary>
    private Camera _mainCamera;

    /// <summary>
    /// A property that allows access in a controlled way to the main camera.
    /// </summary>
    public Camera MainCamera
    {
        get
        {
            return _mainCamera;
        }

        private set
        {
            _mainCamera = value;

            if (UI != null)
            {
                if (UI.CurrentCanvas != null)
                {
                    UI.CurrentCanvas.worldCamera = _mainCamera;
                }
            }
        }
    }

    /// <summary>
    /// The <c>UIManager</c> component that handles the UI of the game.
    /// </summary>
    public UIManager UI { get; private set; }

    private AudioManager _audioManager;

    public AudioManager AudioManager
    {
        get
        {
            if (_audioManager == null)
            {
                _audioManager = gameObject.AddComponent<AudioManager>();
            }

            return _audioManager;
        }
    }

    /// <summary>
    /// Property that stores the current Pathfinder.
    /// </summary>
    private AstarPath AstarManager { get; set; }

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

    public string LastLevelLoaded { get; private set; }

    private void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(this);

        gameObject.AddComponent<InputHandler>();
        
        UI = gameObject.AddComponent<UIManager>();
        UI.Setup();
        UI.LoadMainMenu();
        AudioManager.PlayOst("Audio/Ost/MainMenuOst", .7f);

        MainCamera = new GameObject("Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
        MainCamera.backgroundColor = Color.black;
    }

    private void Update()
    {
        if (!UI.MainMenuIsLoaded && !UI.PromptIsLoaded)
        {
            if (Player != null)
            {
                if (Player.Health.CurrentHealth > 0)
                {
                    if (InputHandler.ToggleHUD("Down"))
                    {
                        AudioClipHandler.PlayAudio("Audio/SelectButton", 0, transform.position, false, .5f);
                        UI.SwitchHUD();
                    }

                    if (!UI.GameMenuIsLoaded && InputHandler.OpenMenu("Down"))
                    {
                        AudioClipHandler.PlayAudio("Audio/SelectButton", 0, transform.position, false, .5f);
                        UI.LoadGameMenu();
                    }
                }
            }
        }
    }

    /// <summary>
    /// This method loads the scene named <c>sceneName</c>, loading player and camera prefabs too.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    /// <param name="timeToWait">The time to wait before to load the scene</param>
    /// <returns></returns>
    public IEnumerator LoadLevel(string sceneName, float timeToWait = 0, NullableVector3 playerPosition = null, NullableVector3 cameraPosition = null)
    {
        if (timeToWait > 0)
        {
            yield return new WaitForSeconds(Mathf.Max(timeToWait, 0));
        }

        int buildIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);

        if (buildIndex < 0)
        {
            yield break;
        }

        LastLevelLoaded = sceneName;

        if (Player != null)
        {
            Player.gameObject.SetActive(false);
        }

        AsyncOperation sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        float activationProgress;

        UI.LoadSceneLoadingInfo();

        Canvas canvas = UI.CurrentCanvas;

        if (canvas != null)
        {
            foreach (Transform transform in canvas.transform)
            {
                Destroy(transform.gameObject);
            }
        }

        while (!sceneLoadingOperation.isDone)
        {
            activationProgress = Mathf.Clamp01(sceneLoadingOperation.progress / .9f);

            if (UI.SceneLoadingInfo != null)
            {
                UI.SceneLoadingInfo.text = (Mathf.RoundToInt(activationProgress * 100f)).ToString() + "%";
            }

            yield return null;
        }

        UI.UnloadSceneLoadingInfo();

        GameObject playerSpawn = GameObject.Find(PlayerStartPositionName);
        GameObject cameraSpawn = GameObject.Find(CameraStartPositionName);
        
        if (playerSpawn != null)
        {
            if (playerPosition == null)
            {
                playerPosition = playerSpawn.transform.position;
            }

            if (cameraSpawn != null)
            {
                if (cameraPosition == null)
                {
                    cameraPosition = cameraSpawn.transform.position;
                }
            }
        }

        if (Player != null)
        {
            Player.transform.position = playerPosition;
            Player.gameObject.SetActive(true);
        } else
        {
            Player = Instantiate(Resources.Load<PlayerController>(PlayerResourcesPath), playerPosition, Quaternion.identity);
            Player.Setup();
            DontDestroyOnLoad(Player.gameObject);
        }

        CameraController cameraController = 
            Instantiate(Resources.Load<CameraController>(CameraResourcesPath), cameraPosition, Quaternion.identity);

        cameraController.Target = Player.transform;
        MainCamera = cameraController.CameraComponent;

        GameObject astarManagerPrefab = Resources.Load<GameObject>("AI/AstarGrid");

        if (astarManagerPrefab != null)
        {
            GameObject pathFindingObject = Instantiate(astarManagerPrefab, Vector3.zero, Quaternion.identity);

            AstarPath astarManager = pathFindingObject.GetComponent<AstarPath>();

            Pathfinding.ProceduralGridMover proceduralGridMover = 
                pathFindingObject.GetComponent<Pathfinding.ProceduralGridMover>();

            if (astarManager != null)
            {
                AstarManager = astarManager;
            }

            if (proceduralGridMover != null)
            {
                proceduralGridMover.target = Player.transform;
            }
        }

        Regia regia = FindObjectOfType<Regia>();

        if (regia != null)
        {
            AudioManager.PlayOst(regia.OstClip, regia.OstVolume);
            AudioManager.PlayAmbience(regia.AmbienceClip, regia.AmbienceVolume);
        }

        UI.LoadHUD();
    }

    /// <summary>
    /// This method loads the scene named <c>sceneName</c>, loading player and camera prefabs too.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    /// <param name="timeToWait">The time to wait before to load the scene</param>
    /// <returns></returns>
    public IEnumerator LoadMainMenu()
    {
        int buildIndex = SceneUtility.GetBuildIndexByScenePath("Empty");

        if (buildIndex < 0)
        {
            yield break;
        }

        if (Player != null)
        {
            Destroy(Player.gameObject);
            Player = null;
        }

        AsyncOperation sceneLoadingOperation = SceneManager.LoadSceneAsync("Empty", LoadSceneMode.Single);
        float activationProgress;

        UI.LoadSceneLoadingInfo();

        while (!sceneLoadingOperation.isDone)
        {
            activationProgress = Mathf.Clamp01(sceneLoadingOperation.progress / .9f);

            if (UI.SceneLoadingInfo != null)
            {
                UI.SceneLoadingInfo.text = (Mathf.RoundToInt(activationProgress * 100f)).ToString() + "%";
            }

            yield return null;
        }

        UI.UnloadSceneLoadingInfo();

        Canvas canvas = UI.CurrentCanvas;

        if (canvas == null)
        {
            yield break;
        }

        foreach (Transform transform in canvas.transform)
        {
            Destroy(transform.gameObject);
        }

        UI.LoadMainMenu();
        AudioManager.PlayOst("Audio/Ost/MainMenuOst", .7f);

        if (MainCamera == null)
        {
            MainCamera = new GameObject("Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
            MainCamera.backgroundColor = Color.black;
        }
    }
}
