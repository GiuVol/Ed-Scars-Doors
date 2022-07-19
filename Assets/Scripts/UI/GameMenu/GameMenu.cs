using UnityEngine;

public class GameMenu : UITabMenu
{
    public bool HasControl { get; set; }

    protected new void Start()
    {
        base.Start();
        HasControl = true;
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
    }
}
