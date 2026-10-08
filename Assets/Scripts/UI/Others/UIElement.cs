using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIElement : MonoBehaviour
{
    [Header("-- UIElement --")]
    [SerializeField] private AudioClip m_selectSound = null;
    [SerializeField] private AudioClip m_interactSound = null;

    protected bool m_isSelected_ = false;

    public virtual void Select()
    {
        m_isSelected_ = true;

        if (!InputManager.s_Instance.GamepadReady)
        {
            EventSystem evtSys = EventSystem.current;

            if (evtSys)
            {
                evtSys.SetSelectedGameObject(gameObject);
            }
        }

        if (AudioManager.s_Instance && m_selectSound)
        {
            AudioManager.s_Instance.Play2D(
                m_selectSound,
                AudioManager.AudioParams.s_Sound
            );
        }

        LogUtils.LogInfo(name);
    }
    public virtual void Deselect()
    {
        m_isSelected_ = false;

        if (!InputManager.s_Instance.GamepadReady)
        {
            EventSystem evtSys = EventSystem.current;

            if (evtSys &&
                evtSys.currentSelectedGameObject == gameObject)
            {
                evtSys.SetSelectedGameObject(null);
            }
        }
    }

    public virtual void Interact()
    {
        AudioManager.s_Instance.Play2D(
            m_interactSound,
            AudioManager.AudioParams.s_Sound
        );
    }
}
