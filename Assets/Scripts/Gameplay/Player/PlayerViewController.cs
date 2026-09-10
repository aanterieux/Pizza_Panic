using UnityEngine;

public class PlayerViewController : PlayerComponent
{
    [SerializeField] private float m_rotationSpeed = 1.5f;
    [SerializeField] private float m_maxRotationAngle = 45f;
    [SerializeField] private bool m_invertVAxis = true;

    private float m_verticalRotation = 0f;

    private void Awake()
    {
        m_verticalRotation = transform.localEulerAngles.x;

        if (m_verticalRotation > 180f)
        {
            m_verticalRotation -= 360f;
        }
    }

    private void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        if (!InputManager.s_MouseConnected)
        {
            return;
        }

        float verticalAngle =
            InputManager.s_MouseDelta.y *
            (m_invertVAxis
                ? 1f
                : -1f
            );

        m_verticalRotation += Time.deltaTime * verticalAngle * m_rotationSpeed;
        m_verticalRotation = Mathf.Clamp(
            m_verticalRotation,
            -m_maxRotationAngle,
            m_maxRotationAngle
        );

        transform.localRotation = Quaternion.Euler(
            m_verticalRotation,
            transform.localEulerAngles.y,
            transform.localEulerAngles.z
        );
    }
}
