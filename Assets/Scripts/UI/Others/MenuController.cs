using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuController : MySingleton<MenuController>
{
    public enum MenuMode
    {
        NONE,

        MAIN_MENU,
        OPTIONS,
        CREDITS,
        PAUSE,
        GAME_OVER
    }

    [SerializeField] private List<UIElement> m_elementList = new();
    [SerializeField] private MenuMode m_menuMode = MenuMode.MAIN_MENU;
    [SerializeField] private bool m_wraparoundSelection = true;
    [SerializeField] private bool m_dontDestroyOnLoad = false;

    private Canvas m_canvas = null;
    private int m_selectionIndex = -1;

    private bool isListEmpty
    {
        get => (m_elementList.Count == 0);
    }

    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();

        if (m_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void UpdateElementList()
    {
        if (!m_canvas)
        {
            m_canvas = FindAnyObjectByType<Canvas>();
        }

        if (!m_canvas)
        {
            return;
        }

        m_elementList.Clear();

        if (m_menuMode == MenuMode.NONE)
        {
            return;
        }

        string expectedParentName =
            CommonUtils.NormaliseString(
                $"{m_menuMode}Buttons"
            );

        m_elementList.AddRange(
            m_canvas.transform
                .GetComponentsInChildren<UIElement>(true)
                .Where(element =>
                {
                    for (Transform current =
                         element.transform.parent;
                         current != null;
                         current = current.parent)
                    {
                        if (CommonUtils.NormaliseString(current.name)
                            == expectedParentName)
                        {
                            return true;
                        }
                    }

                    return false;
                })
        );
    }


    private void SelectElement(int _index)
    {
        if (isListEmpty &&
            !CommonUtils.IsInRangeInclusive(_index, 0, m_elementList.Count - 1))
        {
            return;
        }

        if (m_selectionIndex >= 0)
        {
            m_elementList[m_selectionIndex].Deselect();
        }

        m_selectionIndex = _index;
        m_elementList[m_selectionIndex].Select();
    }

    private void ResetSelection()
    {
        if (isListEmpty)
        {
            return;
        }

        DeselectCurrentElement();
        SelectFirstElement();
    }

    private void DeselectAllElements()
    {
        for (int i = 0; i < m_elementList.Count; ++i)
        {
            m_elementList[i].Deselect();
        }

        m_selectionIndex = -1;
    }

    private void SelectFirstElement()
    {
        m_selectionIndex = 0;
        m_elementList[m_selectionIndex].Select();
    }

    private void OnMenuModeChange()
    {
        UpdateElementList();
        ResetSelection();
    }


    private void OnSceneLoaded(Scene _scene, LoadSceneMode _loadSceneMode)
    {
        OnMenuModeChange();
    }


    public void SetMenuMode(MenuMode _newMode)
    {
        if (m_menuMode == _newMode)
        {
            return;
        }

        m_menuMode = _newMode;

        OnMenuModeChange();
    }


    public void SelectNextElement()
    {
        if (isListEmpty)
        {
            return;
        }

        int nextIndex = m_selectionIndex + 1;

        if (nextIndex >= m_elementList.Count)
        {
            nextIndex =
                (m_wraparoundSelection)
                    ? 0
                    : m_elementList.Count - 1;
        }

        SelectElement(nextIndex);
    }
    public void SelectPreviousElement()
    {
        if (isListEmpty)
        {
            return;
        }

        int previousIndex = m_selectionIndex - 1;

        if (previousIndex < 0)
        {
            previousIndex =
                (m_wraparoundSelection)
                    ? m_elementList.Count - 1
                    : 0;
        }

        SelectElement(previousIndex);
    }

    public void DeselectCurrentElement()
    {
        if (CommonUtils.IsInRangeInclusive(m_selectionIndex, 0, m_elementList.Count - 1))
        {
            UIElement currentElement = m_elementList[m_selectionIndex];

            if (!currentElement)
            {
                return;
            }

            currentElement.Deselect();
        }
    }

    public void InteractWithSelectedElement()
    {
        if (isListEmpty)
        {
            return;
        }

        UIElement selectedElement = m_elementList[m_selectionIndex];

        if (!selectedElement)
        {
            return;
        }

        selectedElement.Interact();
    }


    public void OnSelectNext(InputAction.CallbackContext _context)
    {
        if (isListEmpty)
        {
            LogUtils.LogWarning("Cannot navigate empty menu");
            return;
        }

        SelectNextElement();
    }
    public void OnSelectPrevious(InputAction.CallbackContext _context)
    {
        if (isListEmpty)
        {
            LogUtils.LogWarning("Cannot navigate empty menu");
            return;
        }

        SelectPreviousElement();
    }

    public void OnInteract(InputAction.CallbackContext _context)
    {
        InteractWithSelectedElement();
    }
}
