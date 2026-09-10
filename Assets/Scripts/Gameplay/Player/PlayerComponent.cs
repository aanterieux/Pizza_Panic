using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    private Transform m_camTransform = null;

    protected Transform m_CamTransform_
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
}
