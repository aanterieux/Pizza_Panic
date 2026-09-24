using UnityEngine;
using TMPro;

public class FPSViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_tmpText = null;

    private int m_frameCount;
    private float m_totalFrameTime;

    private void Update()
    {
        m_frameCount++;
        m_totalFrameTime += Time.unscaledDeltaTime;

        if (m_frameCount >= 1000)
        {
            float averageFPS = m_frameCount / m_totalFrameTime;
            string logText = $"Average FPS: {averageFPS:F2}";

            if (m_tmpText)
            {
                m_tmpText.text = logText;
            }
            else
            {
                Debug.Log(logText);
            }

            m_frameCount = 0;
            m_totalFrameTime = 0f;
        }
    }
}
