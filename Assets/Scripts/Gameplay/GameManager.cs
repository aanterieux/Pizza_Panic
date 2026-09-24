using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MySingleton<GameManager>
{
    [SerializeField] private GameObject m_gameOverUI = null;
    [SerializeField] [Min(0f)]
     private float m_timeScale = 1f;
    [SerializeField] private bool m_dontDestroyOnLoad = true;

    private Transform m_canvasTransform = null;
    private PlayerStatManager m_playerStatManager = null;
    private float m_timeScaleCpy = 1f;
    private bool m_isGameOver = false;
    private bool m_canTriggerGameOver = false;
    private bool m_hasSceneChanged = false;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
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

    protected override void OnDestroy()
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

    public void TriggerGameOver()
    {
        Instantiate(
            m_gameOverUI,
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
        SceneManager.LoadScene(0);
    }
}
