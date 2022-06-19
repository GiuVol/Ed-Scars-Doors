using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEditor;

public class GameManager : MonoBehaviour
{
    private const string SceneResourcesFolder = "Scenes";

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
        if (!UI.IsInMainMenu)
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

        UI.LoadingInfos.gameObject.SetActive(true);

        while (!sceneLoadingOperation.isDone)
        {
            activationProgress = Mathf.Clamp01(sceneLoadingOperation.progress / .9f);
            UI.LoadingInfos.text = activationProgress.ToString();
            yield return null;
        }

        UI.LoadingInfos.gameObject.SetActive(false);
        
        Vector3 playerPosition = GameObject.Find("PlayerStartPosition").transform.position;
        Vector3 cameraPosition = GameObject.Find("CameraStartPosition").transform.position;

        PlayerController playerController = 
            Instantiate(Resources.Load<PlayerController>("Player/Player"), playerPosition, Quaternion.identity);
        CameraController cameraController = 
            Instantiate(Resources.Load<CameraController>("Player/MainCamera"), cameraPosition, Quaternion.identity);

        cameraController.Target = playerController.transform;
    }
}
