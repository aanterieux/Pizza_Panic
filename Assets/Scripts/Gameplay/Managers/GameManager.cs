using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MySingleton<GameManager>
{
    [SerializeField] private bool m_dontDestroyOnLoad = true;
    [Space]
    [SerializeField] private GameObject m_pauseUIPrefab = null;
    [SerializeField] private GameObject m_gameOverUIPrefab = null;
    [SerializeField] [Min(0f)] private float m_timeScale = 1f;

    private Transform m_canvasTransform = null;
    private PlayerStatManager m_playerStatManager = null;
    private GameObject m_pauseUI = null;
    private float m_timeScaleCpy = 1f;
    private bool m_isGameOver = false;
    private bool m_canTriggerGameOver = false;
    private bool m_hasSceneChanged = false;

    private void Start()
    {
        if (!m_dontDestroyOnLoad)
        {
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
    }

    private void Update()
    {
        if (m_hasSceneChanged)
        {
            RefreshSceneReferences();
            m_hasSceneChanged = false;
        }

        if (m_isGameOver)
        {
            if (m_canTriggerGameOver)
            {
                TriggerGameOver();
                m_canTriggerGameOver = false;
            }

            return;
        }

        if (m_timeScaleCpy != m_timeScale)
        {
            UpdateTimeScale();
            m_timeScaleCpy = m_timeScale;
        }

        if (m_playerStatManager &&
            m_playerStatManager.m_Health <= 0)
        {
            m_isGameOver = true;
            m_canTriggerGameOver = true;
        }
    }

    private void OnValidate()
    {
        if (m_timeScaleCpy != m_timeScale)
        {
            UpdateTimeScale();
            m_timeScaleCpy = m_timeScale;
        }
    }


    protected override void OnSingletonAwake()
    {
        if (m_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnSingletonDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene _scene, LoadSceneMode _loadSceneMode)
    {
        if (_scene.name != "MainMenu")
        {
            WarpCursorToScreenCenter();
            HideAndLockCursor();
        }

        UpdateTimeScale();

        m_hasSceneChanged = true;
        m_isGameOver = false;
    }

    private void RefreshSceneReferences()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();

        m_canvasTransform =
            (canvas)
                ? canvas.transform
                : null;

        m_playerStatManager = FindAnyObjectByType<PlayerStatManager>();

    }

    private bool CheckMouseValidity(in string _action)
    {
        if (!InputManager.s_Instance.MouseConnected)
        {
            LogUtils.LogWarning($"Could not {_action} cursor: no mouse connected");
            return false;
        }

        return true;
    }

    private void UpdateTimeScale()
    {
        Time.timeScale = m_timeScale;
    }


    public void HideAndLockCursor()
    {
        if (!CheckMouseValidity("hide and lock"))
        {
            return;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void ShowAndUnlockCursor()
    {
        if (!CheckMouseValidity("show and unlock"))
        {
            return;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void WarpCursorToScreenCenter()
    {
        if (!InputManager.s_Instance.MouseConnected)
        {
            return;
        }

        Vector2 screenCenter = 0.5f * new Vector2(
            Screen.width,
            Screen.height
        );

        InputManager.s_Instance.CurrentMouse.WarpCursorPosition(screenCenter);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;

        if (!m_pauseUI)
        {
            Instantiate(m_pauseUIPrefab, m_canvasTransform);
        }

        m_pauseUI.SetActive(true);
        MenuController.s_Instance.SetMenuMode(MenuController.MenuMode.PAUSE);
    }
    public void ResumeGame()
    {
        Time.timeScale = m_timeScale;

        if (m_pauseUI)
        {
            m_pauseUI.SetActive(false);
        }

        MenuController.s_Instance.SetMenuMode(MenuController.MenuMode.NONE);
    }

    public void TriggerGameOver()
    {
        Instantiate(
            m_gameOverUIPrefab,
            m_canvasTransform
        );
        ShowAndUnlockCursor();

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
