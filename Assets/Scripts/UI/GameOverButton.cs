public class GameOverButton : UIButton<GameOverButton.ButtonType>
{
    public enum ButtonType
    {
        RESTART,
        MENU
    }

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


    public override void OnClick()
    {
        switch (base.m_type)
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
