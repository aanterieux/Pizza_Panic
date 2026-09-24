using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : UIButton<MainMenuButton.ButtonType>
{
    public enum ButtonType
    {
        PLAY,
        QUIT
    }

    private void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    private void QuitGame()
    {
        Application.Quit();
    }


    public override void OnClick()
    {
        base.OnClick();

        switch (base.ButtonType_)
        {
            case ButtonType.PLAY: StartGame(); break;
            case ButtonType.QUIT: QuitGame(); break;
            default: break;
        }
    }
}
