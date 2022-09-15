using UnityEngine;

public class InputHandler
{
    /// <summary>
    /// Returns the horizontal input.
    /// </summary>
    public static float HorizontalInput
    {
        get
        {
            return Input.GetAxis("Horizontal");
        }
    }

    /// <summary>
    /// Returns whether an event occurs on the Jump key.
    /// </summary>
    /// <param name="pressType">
    /// This value specifies the type of the event you want to intercept.
    /// </param>
    /// <returns>
    /// This method returns a boolean that is true if the desired event occurs on the specified key, false otherwise.
    /// </returns>
    public static bool Jump(string pressType = "")
    {
        return ButtonPressed("Jump", pressType);
    }

    /// <summary>
    /// Returns whether an event occurs on the Dash key.
    /// </summary>
    /// <param name="pressType">
    /// This value specifies the type of the event you want to intercept.
    /// </param>
    /// <returns>
    /// This method returns a boolean that is true if the desired event occurs on the specified key, false otherwise.
    /// </returns>
    public static bool Dash(string pressType = "")
    {
        return ButtonPressed("Dash", pressType);
    }

    /// <summary>
    /// Returns whether an event occurs on the Shoot key.
    /// </summary>
    /// <param name="pressType">
    /// This value specifies the type of the event you want to intercept.
    /// </param>
    /// <returns>
    /// This method returns a boolean that is true if the desired event occurs on the specified key, false otherwise.
    /// </returns>
    public static bool Shoot(string pressType = "")
    {
        return ButtonPressed("Shoot", pressType);
    }

    /// <summary>
    /// Returns whether an event occurs on the Hide key.
    /// </summary>
    /// <param name="pressType">
    /// This value specifies the type of the event you want to intercept.
    /// </param>
    /// <returns>
    /// This method returns a boolean that is true if the desired event occurs on the specified key, false otherwise.
    /// </returns>
    public static bool Hide(string pressType = "")
    {
        return ButtonPressed("Hide", pressType);
    }

    /// <summary>
    /// Returns whether an event occurs on a certain key.
    /// </summary>
    /// <param name="pressType">
    /// This value specifies the type of the event you want to intercept.
    /// </param>
    /// <returns>
    /// This method returns a boolean that is true if the desired event occurs on the specified key, false otherwise.
    /// </returns>
    private static bool ButtonPressed(string buttonName, string pressType)
    {
        bool buttonPressed;

        switch (pressType)
        {
            case "Down":
                buttonPressed = Input.GetButtonDown(buttonName);
                break;
            case "Up":
                buttonPressed = Input.GetButtonUp(buttonName);
                break;
            default:
                buttonPressed = Input.GetButton(buttonName);
                break;
        }

        return buttonPressed;
    }
}
