using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputManager : MySingleton<InputManager>
{
    [SerializeField] private bool m_dontDestroyOnLoad = true;

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
        get => CurrentGamepad?.leftStick;
    }
    public StickControl GamepadStick_Right
    {
        get => CurrentGamepad?.rightStick;
    }

    public Vector2 MouseDelta
    {
        get =>
            (MouseConnected)
                ? CurrentMouse.delta.ReadValue()
                : Vector2.zero;
    }
    public Vector2 GamepadDelta_Left
    {
        get =>
            (GamepadConnected)
                ? CurrentGamepad.leftStick.ReadValue()
                : Vector2.zero;
    }
    public Vector2 GamepadDelta_Right
    {
        get =>
            (GamepadConnected)
                ? CurrentGamepad.rightStick.ReadValue()
                : Vector2.zero;
    }

    public int KeyboardCount
    {
        get => InputSystem.devices.Count(d => d is Keyboard);
    }
    public int MouseCount
    {
        get => InputSystem.devices.Count(d => d is Mouse);
    }
    public int GamepadCount
    {
        get => InputSystem.devices.Count(d => d is Gamepad);
    }
    public int GameDeviceCount
    {
        get => KeyboardCount + MouseCount + GamepadCount;
    }

    public bool KeyboardConnected
    {
        get => (KeyboardCount > 0);
    }
    public bool MouseConnected
    {
        get => (MouseCount > 0);
    }
    public bool GamepadConnected
    {
        get => (GamepadCount > 0);
    }
    public bool NoGameDeviceConnected
    {
        get => (GameDeviceCount == 0);
    }

    public bool KeyboardReady
    {
        get => (KeyboardConnected && CurrentKeyboard != null);
    }
    public bool MouseReady
    {
        get => (MouseConnected && CurrentMouse != null);
    }
    public bool GamepadReady
    {
        get => (GamepadConnected && CurrentGamepad != null);
    }

    public bool KeyboardPress
    {
        get =>
            (KeyboardReady &&
             CurrentKeyboard.anyKey.IsPressed());
    }
    public bool MousePress
    {
        get =>
            (MouseReady && CurrentMouse.allControls.Count(
                x =>
                    x is ButtonControl &&
                    x.IsPressed()
            ) > 0);
    }
    public bool GamepadPress
    {
        get =>
            (GamepadReady && CurrentGamepad.allControls.Count(
                x =>
                    x is ButtonControl &&
                    x.IsPressed()
            ) > 0);
    }
    public bool GameDevicePress
    {
        get => (KeyboardPress || MousePress || GamepadPress);
    }

    protected override void OnSingletonAwake()
    {
        if (m_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
