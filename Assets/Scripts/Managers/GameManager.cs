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
        UI.Setup();
        UI.LoadMainMenu();

        MainCamera = new GameObject("Camera", typeof(Camera)).GetComponent<Camera>();
        MainCamera.backgroundColor = Color.black;
    }

    private void Update()
    {
        if (!UI.MainMenuIsLoaded && !UI.PromptIsLoaded)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                UI.SwitchHUD();
            }

            if (!UI.GameMenuIsLoaded && Input.GetKeyDown(KeyCode.M))
            {
                UI.LoadGameMenu();
            }
        }
    }

    /// <summary>
    /// This method loads the scene named <c>sceneName</c>, loading player and camera prefabs too.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load</param>
    public IEnumerator LoadScene(string sceneName)
    {
        int buildIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);

        if (buildIndex < 0)
        {
            yield break;
        }

        AsyncOperation sceneLoadingOperation = 
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        float activationProgress;

        UI.LoadSceneLoadingInfo();

        while (!sceneLoadingOperation.isDone)
        {
            activationProgress = Mathf.Clamp01(sceneLoadingOperation.progress / .9f);

            if (UI.SceneLoadingInfo != null)
            {
                UI.SceneLoadingInfo.text = activationProgress.ToString();
            }

            yield return null;
        }

        UI.UnloadSceneLoadingInfo();

        Vector3 playerPosition = Vector3.zero;
        Vector3 cameraPosition = Vector3.zero;

        GameObject playerSpawn = GameObject.Find(PlayerStartPositionName);
        GameObject cameraSpawn = GameObject.Find(CameraStartPositionName);
        
        if (playerSpawn != null)
        {
            playerPosition = playerSpawn.transform.position;

            if (cameraSpawn != null)
            {
                cameraPosition = cameraSpawn.transform.position;
            }
        }

        PlayerController playerController = 
            Instantiate(Resources.Load<PlayerController>(PlayerResourcesPath), playerPosition, Quaternion.identity);
        CameraController cameraController = 
            Instantiate(Resources.Load<CameraController>(CameraResourcesPath), cameraPosition, Quaternion.identity);

        cameraController.Target = playerController.transform;

        Player = playerController;
        Player.Setup();
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
                proceduralGridMover.target = playerController.transform;
            }
        }

        UI.LoadHUD();
    }
}
