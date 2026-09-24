public class GameOverButton : UIButton<GameOverButton.ButtonType>
{
    public enum ButtonType
    {
        RESTART,
        MENU
    }

    private void RestartGame()
    {
        GameManager.s_Instance.RestartGame();
    }

    private void ExitToMenu()
    {
        GameManager.s_Instance.ExitToMainMenu();
    }


    public override void OnClick()
    {
        base.OnClick();

        switch (base.ButtonType_)
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
