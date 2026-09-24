using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PlayerComponent
{
    [Header("- Movement -")]
    [SerializeField] private float m_baseMoveSpeed = 4f;
    [SerializeField] private float m_runSpeedMultiplier = 1.75f;
    [SerializeField] private float m_jumpForce = 5f;
    [SerializeField] private float m_deceleration = 5f;
    [SerializeField] private bool m_toggleToRun = true;

    [Header("- Rotation -")]
    [SerializeField] private float m_rotationSpeed = 4f;
    [SerializeField] private bool m_invertHAxis = false;

    [Header("- Ground detection -")]
    [SerializeField] private float m_maxSlopeAngle = 30f;
    [SerializeField] private float m_groundCheckRadius = 0.25f;
    [SerializeField] private float m_groundCheckDistance = 0.15f;
    [SerializeField] private LayerMask m_notJumpableLayers = 1 << 3;

    [Header("- Jump forgiveness -")]
    [SerializeField] private float m_jumpBufferTime = 0.15f;
    [SerializeField] private float m_coyoteTime = 0.15f;

    private Rigidbody m_rb = null;
    private CapsuleCollider m_capsule = null;
    private Vector3 m_movement = Vector3.zero;
    private float m_finalMoveSpeed = 0f;
    private float m_coyoteTimer = 0f;
    private float m_jumpBufferTimer = 0f;
    private float m_footstepSoundTimer = 0f;
    private float m_yaw = 0f;
    private bool m_isGrounded = false;
    private bool m_isRunning = false;

    public float MoveSpeed
    {
        get => m_finalMoveSpeed;
    }
    public bool IsMoving
    {
        get =>
            (!Mathf.Approximately(m_movement.x, 0f) ||
             !Mathf.Approximately(m_movement.z, 0f));
    }
    public bool IsRunning
    {
        get => m_isRunning;
    }

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        m_capsule = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        CheckGrounding();
        CheckCoyoteTime();

        if (m_jumpBufferTimer > 0f && m_coyoteTimer > 0f)
        {
            Jump();
            AudioController_.PlayJumpSound();
            m_isGrounded = false;

            m_coyoteTimer = 0f;
            m_jumpBufferTimer = 0f;
        }

        Quaternion rbRotation = Quaternion.Euler(0f, m_yaw, 0f);

        m_rb.MoveRotation(rbRotation);

        HandleMovement();
    }

    private void Update()
    {
        Rotate();

        if (m_jumpBufferTimer > 0f)
        {
            m_jumpBufferTimer -= Time.deltaTime;
        }
    }


    private void Rotate()
    {
        if (!InputManager.s_Instance.MouseConnected)
        {
            return;
        }

        Vector2 rotationAxis = InputManager.s_Instance.MouseDelta;
        float horizontal =
            rotationAxis.x *
            (m_invertHAxis ? -1f : 1f);

        m_yaw += horizontal * m_rotationSpeed * Time.deltaTime;
    }

    private void HandleMovement()
    {
        if (!IsMoving)
        {
            m_isRunning = false;
            Decelerate();
            return;
        }

        Move();
        HandleMoveSound();
    }
    private void Move()
    {
        Vector3 movementInput =
            Vector3.ClampMagnitude(m_movement, 1f);

        m_finalMoveSpeed =
            m_baseMoveSpeed *
            (m_isRunning ? m_runSpeedMultiplier : 1f);

        Vector3 movementDirection =
            transform.TransformDirection(movementInput) * m_finalMoveSpeed;

        Vector3 velocity = m_rb.linearVelocity;

        velocity.x = movementDirection.x;
        velocity.z = movementDirection.z;

        m_rb.linearVelocity = velocity;
    }
    private void HandleMoveSound()
    {
        float playInterval = 1f / (0.5f * m_finalMoveSpeed);

        if (!m_isGrounded)
        {
            m_footstepSoundTimer = 0.5f * playInterval;
            return;
        }

        m_footstepSoundTimer += Time.deltaTime;

        if (m_footstepSoundTimer >= playInterval)
        {
            AudioController_.PlayFootstepSound();
            m_footstepSoundTimer = 0f;
        }
    }

    private void Decelerate()
    {
        Vector3 velocity = m_rb.linearVelocity;

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );

        Vector3 newHorizontalVelocity =
            Vector3.MoveTowards(
                horizontalVelocity,
                Vector3.zero,
                m_deceleration *
                Time.fixedDeltaTime
            );

        velocity.x = newHorizontalVelocity.x;
        velocity.z = newHorizontalVelocity.z;

        m_rb.linearVelocity = velocity;
    }

    private void CheckGrounding()
    {
        Vector3 center =
            transform.TransformPoint(
                m_capsule.center
            );

        float halfHeight =
            m_capsule.height * 0.5f;

        float bottomOffset =
            Mathf.Max(
                halfHeight -
                m_capsule.radius,
                0f
            );

        Vector3 bottom =
            center +
            Vector3.down *
            bottomOffset;

        int groundMask =
            ~m_notJumpableLayers;

        bool hitGround =
            Physics.SphereCast(
                bottom + Vector3.up * 0.05f,
                m_groundCheckRadius,
                Vector3.down,
                out RaycastHit hit,
                m_groundCheckDistance,
                groundMask,
                QueryTriggerInteraction.Ignore
            );

        bool wasGrounded = m_isGrounded;

        m_isGrounded =
            hitGround &&
            !IsTooSteep(
                hit.normal,
                m_maxSlopeAngle
            );

        if (!m_isGrounded)
        {
            return;
        }

        //if (m_hasBeenAirborne &&
        //    m_airborneTimer >= m_minAirborneTimeForLanding)
        //{
        //    AudioController_.PlayLandingSound();
        //}
    }

    private void CheckCoyoteTime()
    {
        if (m_isGrounded)
        {
            m_coyoteTimer = m_coyoteTime;
        }
        else
        {
            m_coyoteTimer -= Time.fixedDeltaTime;
        }
    }

    private void Jump()
    {
        Vector3 velocity =
            m_rb.linearVelocity;

        velocity.y = m_jumpForce;

        m_rb.linearVelocity = velocity;
    }

    private bool IsTooSteep(in Vector3 _groundNormal, in float _referenceAngle)
    {
        float groundDot =
            Vector3.Dot(
                _groundNormal,
                Vector3.up
            );

        float minGroundDot =
            Mathf.Cos(
                _referenceAngle *
                Mathf.Deg2Rad
            );

        return groundDot < minGroundDot;
    }

    private void OnMoveTemplate(in InputAction.CallbackContext _context,ref float axis)
    {
        if (_context.canceled)
        {
            axis = 0f;
            return;
        }

        axis = _context.ReadValue<float>();
    }

    public void OnMoveX(InputAction.CallbackContext _context)
    {
        OnMoveTemplate(
            _context,
            ref m_movement.x
        );
    }
    public void OnMoveZ(InputAction.CallbackContext _context)
    {
        OnMoveTemplate(
            _context,
            ref m_movement.z
        );
    }

    public void OnJump(InputAction.CallbackContext _context)
    {
        if (!m_rb)
        {
            LogUtils.LogWarning(
                "Cannot jump: Rigidbody is null."
            );
            return;
        }

        if (_context.performed)
        {
            m_jumpBufferTimer = m_jumpBufferTime;
        }
    }

    public void OnRun(InputAction.CallbackContext _context)
    {
        if (m_toggleToRun)
        {
            if (_context.started)
            {
                m_isRunning = !m_isRunning;
            }

            return;
        }

        m_isRunning =
        _context.performed;
    }
}