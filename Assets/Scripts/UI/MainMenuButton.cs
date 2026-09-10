using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    private enum ButtonType
    {
        PLAY,
        QUIT
    }

    [SerializeField] private ButtonType m_buttonType = ButtonType.PLAY;

    private void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    private void QuitGame()
    {
        Application.Quit();
    }


    public void OnClick()
    {
        switch (m_buttonType)
        {
            case ButtonType.PLAY: StartGame(); break;
            case ButtonType.QUIT: QuitGame(); break;
            default: break;
        }
    }
}
