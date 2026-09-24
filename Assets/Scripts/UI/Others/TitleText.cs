using UnityEngine;

public class TitleText : MonoBehaviour
{
    private const float SIZE_CHANGE_TIMER_LIMIT = 1000000f;

    [SerializeField] [Range(0f, 10f)] private float m_sizeChangeSpeed = 1.5f;
    [SerializeField] [Range(0.01f, 1.5f)] private float m_minScale = 1f;
    [SerializeField] [Range(1.5f, 3f)] private float m_maxScale = 1.5f;

    private RectTransform m_rectTransform = null;
    private float m_sizeChangeTimer = 0f;

    private void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        m_sizeChangeTimer += m_sizeChangeSpeed * Time.deltaTime;

        if (m_sizeChangeTimer > SIZE_CHANGE_TIMER_LIMIT)
        {
            m_sizeChangeTimer = 0f;
        }

        float newScale =
            m_minScale
            + Mathf.Abs(
                (m_maxScale - m_minScale)
                * Mathf.Sin(m_sizeChangeTimer)
            );
        Vector3 fullScale = m_rectTransform.localScale;
        fullScale.x = newScale;
        fullScale.y = newScale;

        m_rectTransform.localScale = fullScale;

        float newRotation =
            0.5f * m_minScale +
            (1.5f * m_maxScale - 0.5f * m_minScale)
            * Mathf.Cos(2f * m_sizeChangeTimer);
        Vector3 fullRotation = m_rectTransform.localRotation.eulerAngles;
        fullRotation.z = newRotation;

        m_rectTransform.localRotation = Quaternion.Euler(fullRotation);
    }
}
