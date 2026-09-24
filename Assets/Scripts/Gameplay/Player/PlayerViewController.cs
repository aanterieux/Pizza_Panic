using UnityEngine;

public class PlayerViewController : PlayerComponent
{
    private const float BOB_TIMER_LIMIT = 1000000f;

    [Header("- Rotation -")]
    [SerializeField] private float m_rotationSpeed = 1.5f;
    [SerializeField] private float m_maxRotationAngle = 45f;
    [SerializeField] private bool m_invertVAxis = true;

    [Header("- View Bobbing -")]
    [Header("- Up/Down")]
    [SerializeField] [Range(0f, 20f)] private float m_idleUpDownBobSpeed = 3f;
    [SerializeField] [Range(0f, 20f)] private float m_walkUpDownBobSpeed = 9.5f;
    [SerializeField] [Range(0f, 20f)] private float m_runUpDownBobSpeed = 15f;
    [SerializeField] [Range(0f, 0.1f)] private float m_upDownBobStrength = 0.0075f;

    [Header("- Left/Right")]
    [SerializeField] [Range(0f, 20f)] private float m_walkLeftRightBobSpeed = 7.5f;
    [SerializeField] [Range(0f, 20f)] private float m_runLeftRightBobSpeed = 10f;
    [SerializeField] [Range(0f, 1f)] private float m_leftRightBobStrength = 0.05f;

    private PlayerController m_controller = null;
    private float m_verticalRotation = 0f;
    private float m_upDownBobTimer = 0f;
    private float m_leftRightBobTimer = 0f;

    private void Awake()
    {
        m_controller = GetComponentInParent<PlayerController>();
        m_verticalRotation = transform.localEulerAngles.x;

        if (m_verticalRotation > 180f)
        {
            m_verticalRotation -= 360f;
        }
    }

    private void Update()
    {
        Bob();

        if (!InputManager.s_Instance.MouseConnected)
        {
            return;
        }

        Rotate();
    }

    private void Rotate()
    {
        float verticalAngle =
            InputManager.s_Instance.MouseDelta.y *
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

    private void Bob()
    {
        BobUpDown();
        BobLeftRight();
    }
    private void BobUpDown()
    {
        float upDownBobSpeed;

        if (!m_controller.IsMoving)
        {
            upDownBobSpeed = m_idleUpDownBobSpeed;
        }
        else
        {
            upDownBobSpeed =
                (m_controller.IsRunning)
                    ? m_runUpDownBobSpeed
                    : m_walkUpDownBobSpeed;
        }

        m_upDownBobTimer += upDownBobSpeed * Time.deltaTime;

        if (m_upDownBobTimer > BOB_TIMER_LIMIT)
        {
            m_upDownBobTimer = 0f;
        }

        transform.localPosition +=
            m_upDownBobStrength
            * Mathf.Sin(m_upDownBobTimer)
            * Vector3.up;
    }
    private void BobLeftRight()
    {
        if (!m_controller.IsMoving)
        {
            float zRotation = transform.localRotation.eulerAngles.z;

            if (!Mathf.Approximately(zRotation, 0f))
            {
                Vector3 localRotation = transform.localRotation.eulerAngles;
                localRotation.z = 0f;

                transform.localRotation = Quaternion.Euler(localRotation);
            }

            return;
        }

        float leftRightBobSpeed =
            (m_controller.IsRunning)
                ? m_runLeftRightBobSpeed
                : m_walkLeftRightBobSpeed;

        m_leftRightBobTimer += leftRightBobSpeed * Time.deltaTime;

        if (m_leftRightBobTimer > BOB_TIMER_LIMIT)
        {
            m_leftRightBobTimer = 0f;
        }

        transform.Rotate(
                m_leftRightBobStrength
                * Mathf.Cos(m_leftRightBobTimer)
                * Vector3.forward
            );
    }
}
