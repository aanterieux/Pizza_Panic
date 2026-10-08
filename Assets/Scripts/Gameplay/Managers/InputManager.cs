using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MySingleton<InputManager>
{
    [SerializeField] private bool m_dontDestroyOnLoad = true;
    [Space]
    [SerializeField] private string m_menuActionMapName = "Menu";
    [SerializeField] private string m_playerActionMapName = "Player";
    [SerializeField] [Range(1, 600)]
     private int m_maxPlayerControllerFetchAttempts = 100;

    private PlayerInput m_playerInput = null;
    private InputActionAsset m_gameIA = null;
    private InputActionMap m_menuActionMap = null;
    private InputActionMap m_playerActionMap = null;
    private PlayerController m_playerController = null;
    private Coroutine m_playerInputsCoroutine = null;

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

        m_playerInput = GetComponent<PlayerInput>();
        m_playerInput.notificationBehavior = PlayerNotifications.InvokeUnityEvents;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (GameManager.s_Instance)
        {
            GameManager.s_Instance.m_OnGamemodeChanged += OnGamemodeChanged;
        }
        else
        {
            LogUtils.LogWarning(
                "Could not subscribe to OnGamemodeChanged: " +
                "GameManager.s_Instance is null");
        }

        if (m_playerInput)
        {
            m_gameIA = m_playerInput.actions;
        }
    }

    protected override void OnSingletonDestroy()
    {
        if (GameManager.s_Instance)
        {
            GameManager.s_Instance.m_OnGamemodeChanged -= OnGamemodeChanged;
        }
        else
        {
            LogUtils.LogWarning(
                "Could not unsubscribe to OnGamemodeChanged: " +
                "GameManager.s_Instance is null");
        }

        DisablePlayerInputs();
        DisableMenuInputs();

        StopAllCoroutines();
    }


    private void OnSceneLoaded(Scene _scene, LoadSceneMode _loadSceneMode)
    {
        if (m_playerInputsCoroutine != null)
        {
            StopCoroutine(m_playerInputsCoroutine);
            m_playerInputsCoroutine = null;
        }

        m_playerInputsCoroutine = StartCoroutine(PlayerInputsCoroutine());
    }

    private IEnumerator PlayerInputsCoroutine()
    {
        int fetchAttempts = 0;
        while (!m_playerController
            && fetchAttempts < m_maxPlayerControllerFetchAttempts)
        {
            m_playerController = FindAnyObjectByType<PlayerController>();
            fetchAttempts++;

            yield return null;
        }

        if (m_playerController)
        {
            if (GameManager.s_Instance.Gamemode == GameManager.GameMode.SURVIVAL)
            {
                EnablePlayerInputs();
            }
            else
            {
                DisablePlayerInputs();
            }
        }

        if (m_playerInputsCoroutine != null)
        {
            StopCoroutine(m_playerInputsCoroutine);
            m_playerInputsCoroutine = null;
        }
    }


    private void EnableMenuInputs()
    {
        if (m_menuActionMap == null)
        {
            LogUtils.LogWarning("Cannot enable menu inputs: m_menuActionMap is null");
            return;
        }

        // SelectNext
        InputAction selectNextAction = m_menuActionMap.FindAction("SelectNext");
        if (selectNextAction != null)
        {
            selectNextAction.started += MenuController.s_Instance.OnSelectNext;
            selectNextAction.Enable();
        }
        else
        {
            LogUtils.LogWarning("Cannot enable SelectNext menu action: selectNextAction is null");
        }

        // SelectPrevious
        InputAction selectPreviousAction = m_menuActionMap.FindAction("SelectPrevious");
        if (selectPreviousAction != null)
        {
            selectPreviousAction.started += MenuController.s_Instance.OnSelectPrevious;
            selectPreviousAction.Enable();
        }
        else
        {
            LogUtils.LogWarning("Cannot enable SelectPrevious menu action: selectPreviousAction is null");
        }

        // Interact
        InputAction interactAction = m_menuActionMap.FindAction("Interact");
        if (interactAction != null)
        {
            interactAction.started += MenuController.s_Instance.OnInteract;
            interactAction.Enable();
        }
        else
        {
            LogUtils.LogWarning("Cannot enable Interact menu action: interactAction is null");
        }
    }
    private void DisableMenuInputs()
    {
        if (m_menuActionMap == null)
        {
            LogUtils.LogWarning("Cannot disable menu inputs: m_menuActionMap is null");
            return;
        }

        // Interact
        InputAction interactAction = m_menuActionMap.FindAction("Interact");
        if (interactAction != null)
        {
            interactAction.started -= MenuController.s_Instance.OnInteract;
            interactAction.Disable();
        }
        else
        {
            LogUtils.LogWarning("Cannot disable Interact menu action: interactAction is null");
        }

        // SelectPrevious
        InputAction selectPreviousAction = m_menuActionMap.FindAction("SelectPrevious");
        if (selectPreviousAction != null)
        {
            selectPreviousAction.started -= MenuController.s_Instance.OnSelectPrevious;
            selectPreviousAction.Disable();
        }
        else
        {
            LogUtils.LogWarning("Cannot disable SelectPrevious menu action: selectPreviousAction is null");
        }

        // SelectNext
        InputAction selectNextAction = m_menuActionMap.FindAction("SelectNext");
        if (selectNextAction != null)
        {
            selectNextAction.started -= MenuController.s_Instance.OnSelectNext;
            selectNextAction.Disable();
        }
        else
        {
            LogUtils.LogWarning("Cannot disable SelectNext menu action: selectNextAction is null");
        }
    }

    private void EnablePlayerInputs()
    {
        if (m_playerActionMap == null)
        {
            LogUtils.LogWarning("Cannot enable player inputs: m_playerActionMap is null");
            return;
        }

        // PlayerController
        if (m_playerController)
        {
            // Move
            InputAction moveAction = m_playerActionMap.FindAction("Move");
            if (moveAction != null)
            {
                moveAction.performed += m_playerController.OnMove;
                moveAction.canceled += m_playerController.OnMove;
                moveAction.Enable();
            }
            else
            {
                LogUtils.LogWarning("Could not enable Move action: moveAction is null");
            }

            // Jump
            InputAction jumpAction = m_playerActionMap.FindAction("Jump");
            if (jumpAction != null)
            {
                jumpAction.performed += m_playerController.OnJump;
                jumpAction.Enable();
            }
            else
            {
                LogUtils.LogWarning("Could not enable Jump action: jumpAction is null");
            }

            // Run
            InputAction runAction = m_playerActionMap.FindAction("Run");
            if (runAction != null)
            {
                runAction.started += m_playerController.OnRun;
                runAction.performed += m_playerController.OnRun;
                runAction.canceled += m_playerController.OnRun;
                runAction.Enable();
            }
            else
            {
                LogUtils.LogWarning("Could not enable Run action: runAction is null");
            }
        }
        else
        {
            LogUtils.LogWarning("Could not enable " +
                "Move, Jump and Run actions: " +
                "m_playerController is null");
        }

        // PlayerActionManager
        PlayerActionManager playerActionManager = FindAnyObjectByType<PlayerActionManager>();
        if (playerActionManager)
        {
            // Primary
            InputAction primaryAction = m_playerActionMap.FindAction("PrimaryAction");
            if (primaryAction != null)
            {
                primaryAction.started += playerActionManager.OnPrimaryAction;
                primaryAction.performed += playerActionManager.OnPrimaryAction;
                primaryAction.canceled += playerActionManager.OnPrimaryAction;
                primaryAction.Enable();
            }
            else
            {
                LogUtils.LogWarning("Could not enable PrimaryAction action: primaryAction is null");
            }

            // Secondary
            InputAction secondaryAction = m_playerActionMap.FindAction("SecondaryAction");
            if (secondaryAction != null)
            {
                secondaryAction.started += playerActionManager.OnSecondaryAction;
                secondaryAction.Enable();
            }
            else
            {
                LogUtils.LogWarning("Could not enable SecondaryAction action: secondaryAction is null");
            }

            // Reload
            InputAction reloadAction = m_playerActionMap.FindAction("Reload");
            if (reloadAction != null)
            {
                reloadAction.started += playerActionManager.OnReload;
                reloadAction.Enable();
            }
            else
            {
                LogUtils.LogWarning("Could not enable Reload action: reloadAction is null");
            }
        }
        else
        {
            LogUtils.LogWarning("Could not enable " +
                "PrimaryAction, SecondaryAction and Reload actions: " +
                "playerActionManager is null");
        }

        // GameManager
        if (GameManager.s_Instance)
        {
            InputAction pauseGameAction = m_playerActionMap.FindAction("PauseGame");
            if (pauseGameAction != null)
            {
                pauseGameAction.started += GameManager.s_Instance.OnPauseGame;
                pauseGameAction.Enable();
            }
            else
            {
                LogUtils.LogWarning("Could not enable PauseGame action: pauseGameAction is null");
            }
        }
        else
        {
            LogUtils.LogWarning(
                "Could not enable PauseGame action: " +
                "GameManager.s_Instance is null");
        }
    }
    private void DisablePlayerInputs()
    {
        if (m_playerActionMap == null)
        {
            LogUtils.LogWarning("Cannot disable player inputs: m_playerActionMap is null");
            return;
        }

        // GameManager
        if (GameManager.s_Instance)
        {
            InputAction pauseGameAction = m_playerActionMap.FindAction("PauseGame");
            if (pauseGameAction != null)
            {
                pauseGameAction.started -= GameManager.s_Instance.OnPauseGame;
                pauseGameAction.Disable();
            }
            else
            {
                LogUtils.LogWarning("Could not disable PauseGame action: pauseGameAction is null");
            }
        }
        else
        {
            LogUtils.LogWarning(
                "Could not disable PauseGame action: " +
                "GameManager.s_Instance is null");
        }

        // PlayerActionManager
        PlayerActionManager playerActionManager = FindAnyObjectByType<PlayerActionManager>();
        if (playerActionManager)
        {
            // Primary
            InputAction primaryAction = m_playerActionMap.FindAction("PrimaryAction");
            if (primaryAction != null)
            {
                primaryAction.started -= playerActionManager.OnPrimaryAction;
                primaryAction.performed -= playerActionManager.OnPrimaryAction;
                primaryAction.canceled -= playerActionManager.OnPrimaryAction;
                primaryAction.Disable();
            }
            else
            {
                LogUtils.LogWarning("Could not disable PrimaryAction action: primaryAction is null");
            }

            // Secondary
            InputAction secondaryAction = m_playerActionMap.FindAction("SecondaryAction");
            if (secondaryAction != null)
            {
                secondaryAction.started -= playerActionManager.OnSecondaryAction;
                secondaryAction.Disable();
            }
            else
            {
                LogUtils.LogWarning("Could not disable SecondaryAction action: secondaryAction is null");
            }

            // Reload
            InputAction reloadAction = m_playerActionMap.FindAction("Reload");
            if (reloadAction != null)
            {
                reloadAction.started -= playerActionManager.OnReload;
                reloadAction.Disable();
            }
            else
            {
                LogUtils.LogWarning("Could not disable Reload action: reloadAction is null");
            }
        }
        else
        {
            LogUtils.LogWarning("Could not disable " +
                "PrimaryAction, SecondaryAction and Reload actions: " +
                "playerActionManager is null");
        }

        // PlayerController
        PlayerController m_playerController = FindAnyObjectByType<PlayerController>();
        if (m_playerController)
        {
            // Move
            InputAction moveAction = m_playerActionMap.FindAction("Move");
            if (moveAction != null)
            {
                moveAction.performed -= m_playerController.OnMove;
                moveAction.canceled -= m_playerController.OnMove;
                moveAction.Disable();
            }
            else
            {
                LogUtils.LogWarning("Could not disable Move action: moveAction is null");
            }

            // Jump
            InputAction jumpAction = m_playerActionMap.FindAction("Jump");
            if (jumpAction != null)
            {
                jumpAction.performed -= m_playerController.OnJump;
                jumpAction.Disable();
            }
            else
            {
                LogUtils.LogWarning("Could not disable Jump action: jumpAction is null");
            }

            // Run
            InputAction runAction = m_playerActionMap.FindAction("Run");
            if (runAction != null)
            {
                runAction.started -= m_playerController.OnRun;
                runAction.performed -= m_playerController.OnRun;
                runAction.canceled -= m_playerController.OnRun;
                runAction.Disable();
            }
            else
            {
                LogUtils.LogWarning("Could not disable Run action: runAction is null");
            }
        }
        else
        {
            LogUtils.LogWarning("Could not disable " +
                "Move, Jump and Run actions: " +
                "m_playerController is null");
        }
    }

    private void OnGamemodeChanged()
    {
        if (!GameManager.s_Instance)
        {
            LogUtils.LogWarning(
                "Could not adapt inputs to new game mode: " +
                "GameManager.s_Instance is null");
            return;
        }

        if (m_menuActionMap == null)
        {
            m_menuActionMap = m_gameIA.FindActionMap(m_menuActionMapName);
        }

        if (m_playerActionMap == null)
        {
            m_playerActionMap = m_gameIA.FindActionMap(m_playerActionMapName);
        }

        m_playerInputsCoroutine = StartCoroutine(PlayerInputsCoroutine());

        switch (GameManager.s_Instance.Gamemode)
        {
            case GameManager.GameMode.MAIN_MENU:
            case GameManager.GameMode.OPTIONS:
            case GameManager.GameMode.PAUSE:
            case GameManager.GameMode.GAME_OVER:
                {
                    if (!m_playerInput)
                    {
                        LogUtils.LogWarning(
                            "Cannot switch to menu inputs:" +
                            "m_playerInput is null");
                        break;
                    }

                    EnableMenuInputs();

                    m_playerInput.SwitchCurrentActionMap(m_menuActionMapName);
                }
                break;
            case GameManager.GameMode.SURVIVAL:
                {
                    if (!m_playerInput)
                    {
                        LogUtils.LogWarning(
                            "Cannot switch to gameplay inputs:" +
                            "m_playerInput is null");
                        break;
                    }

                    DisableMenuInputs();

                    m_playerInput.SwitchCurrentActionMap(m_playerActionMapName);
                }
                break;
            default:
                {
                }
                break;
        }
    }
}
