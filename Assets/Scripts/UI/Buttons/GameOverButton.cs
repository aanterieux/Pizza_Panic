public class GameOverButton : UIButton<GameOverButton.GameOverButtonType>
{
    public enum GameOverButtonType
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


    public override void Interact()
    {
        base.Interact();

        switch (base.ButtonType)
        {
            case GameOverButtonType.RESTART: RestartGame(); break;
            case GameOverButtonType.MENU:    ExitToMenu();  break;
            default: break;
        }
    }
}
