using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputManager : MySingleton<InputManager>
{
    public Keyboard CurrentKeyboard
    {
        get => Keyboard.current;
    }
    public Mouse CurrentMouse
    {
        get => Mouse.current;
    }
    public Gamepad CurrentGamepad
    {
        get => Gamepad.current;
    }

    public StickControl GamepadStick_Left
    {
        get => CurrentGamepad.leftStick;
    }
    public StickControl GamepadStick_Right
    {
        get => CurrentGamepad.rightStick;
    }

    public Vector2 MouseDelta
    {
        get => CurrentMouse.delta.ReadValue();
    }
    public Vector2 GamepadDelta_Left
    {
        get => GamepadStick_Left.ReadValue();
    }
    public Vector2 GamepadDelta_Right
    {
        get => GamepadStick_Right.ReadValue();
    }

    public int GameDeviceCount
    {
        get =>
            InputSystem.devices.Count(
                d =>
                    d is Keyboard ||
                    d is Mouse    ||
                    d is Gamepad
            );
    }

    public bool KeyboardConnected
    {
        get => (CurrentKeyboard != null);
    }
    public bool MouseConnected
    {
        get => (CurrentMouse != null);
    }
    public bool GamepadConnected
    {
        get => (CurrentGamepad != null);
    }
    public bool NoGameDeviceConnected
    {
        get => (GameDeviceCount == 0);
    }

    public bool KeyboardPress
    {
        get => CurrentKeyboard.anyKey.IsPressed();
    }
    public bool MousePress
    {
        get =>
            (CurrentMouse.allControls.Count(
                x =>
                    x is ButtonControl &&
                    x.IsPressed()
            ) > 0);
    }
    public bool GamepadPress
    {
        get =>
            (CurrentGamepad.allControls.Count(
                x =>
                    x is ButtonControl &&
                    x.IsPressed()
            ) > 0);
    }
    public bool GameDevicePress
    {
        get => (KeyboardPress || MousePress || GamepadPress);
    }

    [SerializeField] private bool m_dontDestroyOnLoad = true;

    private void Awake()
    {
        if (!InitialiseSingleton())
        {
            return;
        }

        if (m_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
