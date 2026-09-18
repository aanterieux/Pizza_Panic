using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using TMPro;

public abstract class UIButton<TButtonType> : MonoBehaviour where TButtonType : System.Enum
{
    [SerializeField] protected TButtonType m_type;

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

    protected void OnValidate()
    {
        if (!EqualityComparer<TButtonType>.Default.Equals(m_typeCpy, m_type)
            && buttonText)
        {
            m_buttonText.text =
                m_type
                .ToString()
                .ToLowerInvariant()
                .FirstCharacterToUpper();
            m_typeCpy = m_type;
        }
    }

    public abstract void OnClick();
}