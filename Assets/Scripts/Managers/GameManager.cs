using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public UIManager UI { get; private set; }

    public Camera MainCamera { get; private set; }

    private void Start()
    {
        DontDestroyOnLoad(this);

        _instance = this;
        UI = gameObject.AddComponent<UIManager>();
        UI.Setup();

        MainCamera = new GameObject("Camera", typeof(Camera)).GetComponent<Camera>();
        UI.LoadMainMenu();
    }

    private void Update()
    {
        if (!UI.MainMenuIsLoaded)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                UI.SwitchHUD();
            }
        }
    }

    public IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        float activationProgress = 0;

        UI.LoadSceneLoadingInfo();

        while (!sceneLoadingOperation.isDone)
        {
            activationProgress = Mathf.Clamp01(sceneLoadingOperation.progress / .9f);
            UI.LoadingInfo.text = activationProgress.ToString();
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

        UI.LoadHUD();
    }
}
