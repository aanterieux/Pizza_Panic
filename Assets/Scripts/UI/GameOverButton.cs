using UnityEngine;

public class GameOverButton : MonoBehaviour
{
    private enum ButtonType
    {
        RESTART,
        MENU
    }

    [SerializeField] private ButtonType type = ButtonType.RESTART;

    private GameManager m_gameManager = null;

    private void Start()
    {
        m_gameManager = FindAnyObjectByType<GameManager>();
    }


    private void RestartGame()
    {
        m_gameManager.RestartGame();
    }

    private void ExitToMenu()
    {
        m_gameManager.ExitToMainMenu();
    }


    public void OnClick()
    {
        switch (type)
        {
            case ButtonType.RESTART:
                {
                    RestartGame();
                }
                break;
            case ButtonType.MENU:
                {
                    ExitToMenu();
                }
                break;
            default:
                {
                }
                break;
        }
    }
}
