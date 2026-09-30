using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : UIButton<MainMenuButton.MainMenuButtonType>
{
    public enum MainMenuButtonType
    {
        PLAY,
        CREDITS,
        BACK,
        QUIT
    }

    private static bool s_isCreditsObjActive = true;

    private GameObject m_buttonsObj = null;
    private GameObject m_creditsObj = null;

    private bool IsOfType(MainMenuButtonType _type)
    {
        return (ButtonType == _type);
    }
    private bool IsOneOf(params MainMenuButtonType[] _types)
    {
        foreach (MainMenuButtonType type in _types)
        {
            if (IsOfType(type))
            {
                return true;
            }
        }

        return false;
    }


    private void Start()
    {
        m_buttonsObj = GameObject.Find("MainMenuButtons");

        if (IsOneOf(MainMenuButtonType.CREDITS, MainMenuButtonType.BACK))
        {
            m_creditsObj = GameObject.Find("Credits");
            s_isCreditsObjActive = true;
        }
    }

    private void Update()
    {
        if (s_isCreditsObjActive &&
            m_creditsObj && m_creditsObj.activeSelf)
        {
            m_creditsObj.SetActive(false);
            s_isCreditsObjActive = false;
        }
    }


    private void StartGame()
    {
        SceneManager.LoadScene("Pizzeria");
        MenuController.s_Instance.SetMenuMode(MenuController.MenuMode.NONE);
    }
    private void QuitGame()
    {
        Application.Quit();
    }

    private void ShowCredits()
    {
        if (m_buttonsObj)
        {
            m_buttonsObj.SetActive(false);
        }

        if (m_creditsObj)
        {
            m_creditsObj.SetActive(true);
        }

        MenuController.s_Instance.SetMenuMode(MenuController.MenuMode.CREDITS);
    }
    private void HideCredits()
    {
        if (m_buttonsObj)
        {
            m_buttonsObj.SetActive(true);
        }

        if (m_creditsObj)
        {
            m_creditsObj.SetActive(false);
        }
     
        MenuController.s_Instance.SetMenuMode(MenuController.MenuMode.MAIN_MENU);
    }


    public override void Interact()
    {
        base.Interact();

        switch (base.ButtonType)
        {
            case MainMenuButtonType.PLAY: StartGame();   break;
            case MainMenuButtonType.CREDITS: ShowCredits(); break;
            case MainMenuButtonType.BACK: HideCredits(); break;
            case MainMenuButtonType.QUIT: QuitGame();    break;
            default: break;
        }
    }
}
