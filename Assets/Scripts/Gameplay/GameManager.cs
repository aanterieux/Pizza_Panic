using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject m_gameOverUI = null;
    [SerializeField] [Min(0f)]
     private float m_timeScale = 1f;

    private Transform m_canvasTransform = null;
    private PlayerStatManager m_playerStatManager = null;
    private float m_timeScaleCpy = 1f;
    private bool m_isGameOver = false;
    private bool m_canTriggerGameOver = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            HideAndLockCursor();
        }

        UpdateTimeScale();
    }

    private void Start()
    {
        m_canvasTransform = FindAnyObjectByType<Canvas>().transform;
        m_playerStatManager = FindAnyObjectByType<PlayerStatManager>();
    }

    private void Update()
    {
        if (m_isGameOver)
        {
            if (m_canTriggerGameOver)
            {
                TriggerGameOver();
                m_canTriggerGameOver = false;
            }

            return;
        }

        if (m_playerStatManager.m_Health <= 0)
        {
            m_isGameOver = true;
            m_canTriggerGameOver = true;
        }

        if (m_timeScaleCpy != m_timeScale)
        {
            UpdateTimeScale();
            m_timeScaleCpy = m_timeScale;
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


    private bool CheckMouseValidity(in string _action)
    {
        if (!InputManager.s_MouseConnected)
        {
            LogUtility.LogWarning($"Could not {_action} cursor: no mouse connected");
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
        if (!InputManager.s_MouseConnected)
        {
            return;
        }

        Vector2 screenCenter = 0.5f * new Vector2(
            Screen.width,
            Screen.height
        );

        InputManager.s_CurrentMouse.WarpCursorPosition(screenCenter);
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
