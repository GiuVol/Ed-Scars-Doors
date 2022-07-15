using UnityEngine;

public class GameMenu : UITabMenu
{
    // Update is called once per frame
    void Update()
    {
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
