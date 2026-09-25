using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    private Transform m_camTransform = null;
    private PlayerAudioController m_audioController = null;

    protected Transform CamTransform_
    {
        get
        {
            if (!m_camTransform)
            {
                m_camTransform =
                    GetComponentInChildren<PlayerViewController>()
                    .transform;
            }

            return m_camTransform;
        }
    }
    protected PlayerAudioController AudioController_
    {
        get
        {
            if (!m_audioController)
            {
                m_audioController = GetComponent<PlayerAudioController>();
            }

            return m_audioController;
        }
    }
}
