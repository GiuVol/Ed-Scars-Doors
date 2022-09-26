using UnityEngine;

public class GameMenu : UITabMenu
{
    public bool HasControl { get; set; }

    protected new void Start()
    {
        base.Start();
        HasControl = true;

        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            GameManager.Instance.Player.HasControl = false;
        }

        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!HasControl)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SelectedTab--;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SelectedTab++;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            GameManager.Instance.UI.UnloadGameMenu();
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            GameManager.Instance.Player.HasControl = true;
        }

        Time.timeScale = 1;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            GameManager.Instance.Player.HasControl = true;
        }
        
        Time.timeScale = 1;
    }
}
