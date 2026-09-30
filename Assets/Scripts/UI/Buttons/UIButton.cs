using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public abstract class UIButton<TButtonType> :
    UIElement,
        IPointerEnterHandler,
        IPointerExitHandler
    where TButtonType : System.Enum
{
    [Header("-- UIButton --")]
    [SerializeField] private TButtonType m_buttonType;

    private TextMeshProUGUI m_buttonText = null;
    private Button m_button = null;
    private TButtonType m_typeCpy;

    private TextMeshProUGUI buttonText
    {
        get
        {
            if (!m_buttonText)
            {
                m_buttonText = GetComponentInChildren<TextMeshProUGUI>();
            }

            return m_buttonText;
        }
    }
    private Button button
    {
        get
        {
            TextMeshProUGUI tmpText = buttonText;

            if (tmpText)
            {
                m_button = tmpText.GetComponentInParent<Button>();
            }

            return m_button;
        }
    }

    public TButtonType ButtonType
    {
        get => m_buttonType;
    }

    private void Awake()
    {
        InputSystem.onDeviceChange += OnGamepadConnectDisconnect;

        if (!button)
        {
            return;
        }

        Navigation noNav = m_button.navigation;
        noNav.mode = Navigation.Mode.None;
        m_button.navigation = noNav;

        button.onClick.AddListener(OnClick);

        UpdateButtonText();
    }

    private void OnValidate()
    {
        UpdateButtonText();
        OnButtonValidate();
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnGamepadConnectDisconnect;
    }


    private void UpdateButtonText()
    {
        if (!EqualityComparer<TButtonType>.Default.Equals(m_typeCpy, ButtonType)
            && buttonText)
        {
            m_buttonText.text =
                ButtonType
                .ToString()
                .ToLowerInvariant()
                .FirstCharacterToUpper();
            m_typeCpy = ButtonType;
        }
    }

    private void OnGamepadConnectDisconnect(InputDevice _device, InputDeviceChange _change)
    {
        if (!button || _device is not Gamepad)
        {
            return;
        }

        switch (_change)
        {
            case InputDeviceChange.Added:
            case InputDeviceChange.Enabled:
            case InputDeviceChange.Reconnected:
                {
                    button.interactable = true;
                    button.onClick?.RemoveListener(OnClick);
                }
                break;

            case InputDeviceChange.Disconnected:
            case InputDeviceChange.Disabled:
            case InputDeviceChange.Removed:
                {
                    button.interactable = false;
                    button.onClick?.AddListener(OnClick);
                }
                break;

            default:
                {
                }
                break;
        }
    }

    protected virtual void OnButtonValidate()
    {
    }


    public void OnClick()
    {
        Interact();
    }

    public void OnPointerEnter(PointerEventData _eventData)
    {
        if (InputManager.s_Instance.GamepadConnected)
        {
            return;
        }

        Select();
    }
    public void OnPointerExit(PointerEventData _eventData)
    {
        if (InputManager.s_Instance.GamepadConnected)
        {
            return;
        }

        Deselect();
    }
}
