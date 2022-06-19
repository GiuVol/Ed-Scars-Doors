using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonsSet : MonoBehaviour
{
    private static float DistanceBetweenButtons = 10;

    private GameObject ButtonPrefab { get; set; }
    private List<Button> Buttons { get; set; }

    public float Width { get; private set; }
    public float Height { get; private set; }

    private int SelectedButtonIndex { get; set; }

    public void Initialize(GameObject buttonPrefab)
    {
        ButtonPrefab = buttonPrefab;
        Buttons = new List<Button>();

        Width = 0;
        Height = 0;

        SelectedButtonIndex = 0;
    }

    public void AddButton(string text, UnityEngine.Events.UnityAction onClickEvent)
    {
        GameObject newButtonGo = 
            GameObject.Instantiate(ButtonPrefab, transform);

        int numberOfButtons = Buttons.Count;

        Vector2 position = Vector3.zero;

        if (numberOfButtons > 0)
        {
            Button lastButton = Buttons[numberOfButtons - 1];
            position = lastButton.transform.localPosition;

            float yOffset =
                -lastButton.GetComponentInChildren<RectTransform>().sizeDelta.y - 
                UIButtonsSet.DistanceBetweenButtons;
            Vector2 positionOffset = new Vector2(0, yOffset);

            position += positionOffset;
        }

        newButtonGo.transform.localPosition = position;

        newButtonGo.GetComponentInChildren<TextMeshProUGUI>().text = text;
        newButtonGo.GetComponent<Button>().onClick.AddListener(onClickEvent);
        /*
        Navigation customNavigation = new Navigation();
        customNavigation.mode = Navigation.Mode.None;
        newButtonGo.GetComponent<Button>().navigation = customNavigation;
        */
        Buttons.Add(newButtonGo.GetComponent<Button>());
    }

    public void SelectButton(int index)
    {
        if (index < 1 || index > Buttons.Count)
        {
            return;
        }

        SelectedButtonIndex = index - 1;

        Buttons[SelectedButtonIndex].Select();
    }

    /*

    public void PreviousButton()
    {
        if (this.selectedButtonIndex <= 0)
        {
            this.selectedButtonIndex = 0;
        } else
        {
            this.selectedButtonIndex--;
        }

        this.SelectButton(this.selectedButtonIndex + 1);
    }

    public void NextButton()
    {
        if (this.selectedButtonIndex >= this.buttons.Count - 1)
        {
            this.selectedButtonIndex = this.buttons.Count - 1;
        } else
        {
            this.selectedButtonIndex++;
        }

        this.SelectButton(this.selectedButtonIndex + 1);
    }

    public void ExecuteSelectedButton()
    {
        if (this.buttons.Count == 0)
        {
            return;
        }

        this.buttons[selectedButtonIndex].onClick.Invoke();
    }
    
    */
}
