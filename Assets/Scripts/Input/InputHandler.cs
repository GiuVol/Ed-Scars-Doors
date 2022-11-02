using System.Collections;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private static InputHandler _instance;

    private static InputHandler Instance
    {
        get
        {
            return _instance;
        }

        set
        {
            if (_instance == null)
            {
                _instance = value;
            }
        }
    }

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
    /// Returns the vertical input.
    /// </summary>
    public static float VerticalInput
    {
        get
        {
            return Input.GetAxis("Vertical");
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
    public static bool Run(string pressType = "")
    {
        return ButtonPressed("Run", pressType);
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
    public static bool Left(string pressType = "")
    {
        bool pressed = ButtonPressed("Left", pressType);

        if (pressType == "Down")
        {
            if (Instance != null)
            {
                pressed = pressed || (Instance.HorizontalAxisToButton == -1);
            }
        }

        return pressed;
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
    public static bool Right(string pressType = "")
    {
        bool pressed = ButtonPressed("Right", pressType);

        if (pressType == "Down")
        {
            if (Instance != null)
            {
                pressed = pressed || (Instance.HorizontalAxisToButton == 1);
            }
        }

        return pressed;
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
    public static bool Down(string pressType = "")
    {
        bool pressed = ButtonPressed("Down", pressType);

        if (pressType == "Down")
        {
            if (Instance != null)
            {
                pressed = pressed || (Instance.VerticalAxisToButton == -1);
            }
        }

        return pressed;
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
    public static bool Up(string pressType = "")
    {
        bool pressed = ButtonPressed("Up", pressType);

        if (pressType == "Down")
        {
            if (Instance != null)
            {
                pressed = pressed || (Instance.VerticalAxisToButton == 1);
            }
        }

        return pressed;
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
    public static bool Interact(string pressType = "")
    {
        return ButtonPressed("Interact", pressType);
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
    public static bool ToggleHUD(string pressType = "")
    {
        return ButtonPressed("ToggleHUD", pressType);
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
    public static bool OpenMenu(string pressType = "")
    {
        return ButtonPressed("OpenMenu", pressType);
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
    public static bool CloseMenu(string pressType = "")
    {
        return ButtonPressed("CloseMenu", pressType);
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
    public static bool Submit(string pressType = "")
    {
        return ButtonPressed("Submit", pressType);
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

    private float _horizontalAxisForButtonConversion;

    private bool _horizontalAlreadyPressed;

    private float _verticalAxisForButtonConversion;

    private bool _verticalAlreadyPressed;
    
    private int _horizontalAxisToButton;

    private int HorizontalAxisToButton
    {
        get
        {
            int actualValue;

            if (_horizontalAxisToButton >= .5f)
            {
                actualValue = 1;
            } else if (_horizontalAxisToButton <= -.5f)
            {
                actualValue = -1;
            } else
            {
                actualValue = 0;
            }

            return actualValue;
        }

        set
        {
            _horizontalAxisToButton = value;
            _horizontalAxisToButton = HorizontalAxisToButton;
        }
    }

    private int _verticalAxisToButton;

    private int VerticalAxisToButton
    {
        get
        {
            int actualValue;

            if (_verticalAxisToButton >= .5f)
            {
                actualValue = 1;
            }
            else if (_verticalAxisToButton <= -.5f)
            {
                actualValue = -1;
            }
            else
            {
                actualValue = 0;
            }

            return actualValue;
        }

        set
        {
            _verticalAxisToButton = value;
            _verticalAxisToButton = VerticalAxisToButton;
        }
    }
    
    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        _horizontalAxisForButtonConversion = HorizontalInput;
        _verticalAxisForButtonConversion = VerticalInput;

        if (!_horizontalAlreadyPressed)
        {
            if (Mathf.Abs(_horizontalAxisForButtonConversion) > .75f)
            {
                SetHorizontalPressed(_horizontalAxisForButtonConversion);
            }
        }

        if (!_verticalAlreadyPressed)
        {
            if (Mathf.Abs(_verticalAxisForButtonConversion) > .75f)
            {
                SetVerticalPressed(_verticalAxisForButtonConversion);
            }
        }
    }

    private void SetHorizontalPressed(float input)
    {
        StartCoroutine(SetHorizontalPressedCoroutine(input));
    }

    private IEnumerator SetHorizontalPressedCoroutine(float input)
    {
        int orientation;

        if (input >= .5f)
        {
            orientation = 1;
        } else if (input <= -.5f)
        {
            orientation = -1;
        } else
        {
            yield break;
        }

        _horizontalAlreadyPressed = true;
        HorizontalAxisToButton = orientation;

        yield return new WaitForEndOfFrame();

        HorizontalAxisToButton = 0;

        yield return new WaitUntil(() => _horizontalAxisForButtonConversion == 0);

        _horizontalAlreadyPressed = false;

        yield break;
    }

    private void SetVerticalPressed(float input)
    {
        StartCoroutine(SetVerticalPressedCoroutine(input));
    }

    private IEnumerator SetVerticalPressedCoroutine(float input)
    {
        int orientation;

        if (input >= .5f)
        {
            orientation = 1;
        }
        else if (input <= -.5f)
        {
            orientation = -1;
        }
        else
        {
            yield break;
        }

        _verticalAlreadyPressed = true;
        VerticalAxisToButton = orientation;

        yield return new WaitForEndOfFrame();

        VerticalAxisToButton = 0;

        yield return new WaitUntil(() => _verticalAxisForButtonConversion == 0);

        _verticalAlreadyPressed = false;

        yield break;
    }
}
