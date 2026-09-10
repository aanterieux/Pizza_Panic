using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

// Needs an instance for some features
public class InputManager : MonoBehaviour
{
    public static Keyboard s_CurrentKeyboard
    {
        get => Keyboard.current;
    }
    public static Mouse s_CurrentMouse
    {
        get => Mouse.current;
    }
    public static Gamepad s_CurrentGamepad
    {
        get => Gamepad.current;
    }

    public static StickControl s_GamepadStick_Left
    {
        get => s_CurrentGamepad.leftStick;
    }
    public static StickControl s_GamepadStick_Right
    {
        get => s_CurrentGamepad.rightStick;
    }

    public static Vector2 s_MouseDelta
    {
        get => s_CurrentMouse.delta.ReadValue();
    }
    public static Vector2 s_GamepadDelta_Left
    {
        get => s_GamepadStick_Left.ReadValue();
    }
    public static Vector2 s_GamepadDelta_Right
    {
        get => s_GamepadStick_Right.ReadValue();
    }

    public static int s_GameDeviceCount
    {
        get =>
            InputSystem.devices.Count(
                d =>
                    d is Keyboard ||
                    d is Mouse    ||
                    d is Gamepad
            );
    }

    public static bool s_KeyboardConnected
    {
        get => (s_CurrentKeyboard != null);
    }
    public static bool s_MouseConnected
    {
        get => (s_CurrentMouse != null);
    }
    public static bool s_GamepadConnected
    {
        get => (s_CurrentGamepad != null);
    }
    public static bool s_NoGameDeviceConnected
    {
        get => (s_GameDeviceCount == 0);
    }

    public static bool s_KeyboardPress
    {
        get => s_CurrentKeyboard.anyKey.IsPressed();
    }
    public static bool s_MousePress
    {
        get =>
            (s_CurrentMouse.allControls.Count(
                x =>
                    x is ButtonControl &&
                    x.IsPressed()
            ) > 0);
    }
    public static bool s_GamepadPress
    {
        get =>
            (s_CurrentGamepad.allControls.Count(
                x =>
                    x is ButtonControl &&
                    x.IsPressed()
            ) > 0);
    }
    public static bool s_GameDevicePress
    {
        get => (s_KeyboardPress || s_MousePress || s_GamepadPress);
    }

    [SerializeField] private bool m_dontDestroyOnLoad = true;

    private void Awake()
    {
        if (m_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        if (!s_MouseConnected)
        {
            return;
        }

        Vector2 screenCenter = 0.5f * new Vector2(
            Screen.width,
            Screen.height
        );

        s_CurrentMouse.WarpCursorPosition(screenCenter);

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
