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
            if (SelectedTab > 1)
            {
                AudioClipHandler.PlayAudio("Audio/SelectTab", 0, transform.position);
                SelectedTab--;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (SelectedTab < NumberOfTabs)
            {
                AudioClipHandler.PlayAudio("Audio/SelectTab", 0, transform.position);
                SelectedTab++;
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            AudioClipHandler.PlayAudio("Audio/SelectButton", 0, transform.position);
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
