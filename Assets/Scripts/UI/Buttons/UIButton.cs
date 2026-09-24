using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using TMPro;

public abstract class UIButton<TButtonType> :
    MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
    where TButtonType : System.Enum
{
    [SerializeField] private TButtonType m_buttonType;
    [Space]
    [SerializeField] private AudioClip m_clickSound = null;
    [SerializeField] private AudioClip m_focusSound = null;
    [SerializeField] private AudioClip m_unfocusSound = null;

    private TextMeshProUGUI m_buttonText = null;
    private TButtonType m_typeCpy;

    private TextMeshProUGUI buttonText
    {
        get
        {
            if (!m_buttonText)
            {
                m_buttonText = GetComponentInChildren<TextMeshProUGUI>();
            }

            return m_buttonText;
        }
    }

    protected TButtonType ButtonType_
    {
        get => m_buttonType;
    }
    protected AudioClip FocusSound_
    {
        get => m_focusSound;
    }
    protected AudioClip UnfocusSound_
    {
        get => m_unfocusSound;
    }
    protected AudioClip ClickSound_
    {
        get => m_clickSound;
    }

    protected void OnValidate()
    {
        if (!EqualityComparer<TButtonType>.Default.Equals(m_typeCpy, ButtonType_)
            && buttonText)
        {
            m_buttonText.text =
                ButtonType_
                .ToString()
                .ToLowerInvariant()
                .FirstCharacterToUpper();
            m_typeCpy = ButtonType_;
        }
    }


    public virtual void OnClick()
    {
        AudioManager.s_Instance.Play2D(
            m_clickSound,
            AudioManager.AudioParams.s_Sound
        );
    }

    public virtual void OnPointerEnter(PointerEventData _eventData)
    {
        AudioManager.s_Instance.Play2D(
            m_focusSound,
            AudioManager.AudioParams.s_Sound
        );
    }
    public virtual void OnPointerExit(PointerEventData _eventData)
    {
        AudioManager.s_Instance.Play2D(
            m_unfocusSound,
            AudioManager.AudioParams.s_Sound
        );
    }
}
