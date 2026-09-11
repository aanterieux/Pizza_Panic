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
    [SerializeField] private float m_groundCheckRadius = 0.3f;
    [SerializeField] private float m_groundCheckDistance = 0.15f;
    [SerializeField] private LayerMask m_notJumpableLayers;

    private Rigidbody m_rb = null;
    private CapsuleCollider m_capsule = null;
    private Ray m_groundRay = new Ray();
    private Vector3 m_movement = Vector3.zero;
    private bool m_jumpTrigger = false;
    private bool m_isAirborne = false;
    private bool m_isRunning = false;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        m_capsule = GetComponent<CapsuleCollider>();

        m_groundRay.direction = Vector3.down;
    }

    private void FixedUpdate()
    {
        CheckGrounding();

        if (m_jumpTrigger && !m_isAirborne)
        {
            Jump();
            m_jumpTrigger = false;
        }

        Move();
    }

    private void Update()
    {
        Rotate();
    }


    private void Rotate()
    {
        if (!InputManager.s_MouseConnected)
        {
            Debug.LogWarning("Cannot rotate: 'mouse' is null.");
            return;
        }

        Vector2 rotationAxis = InputManager.s_MouseDelta;
        float horizontal = rotationAxis.x * ((m_invertHAxis) ? -1f : 1f);

        rotationAxis.x = 0f;
        rotationAxis.y = horizontal;

        transform.Rotate(Time.deltaTime * m_rotationSpeed * rotationAxis);
    }

    private void Move()
    {
        bool isMovingOnXAxis = !Mathf.Approximately(m_movement.x, 0f);
        bool isMovingOnZAxis = !Mathf.Approximately(m_movement.z, 0f);
        bool isMovingHorizontally = (isMovingOnXAxis || isMovingOnZAxis);

        // When not moving
        if (!isMovingHorizontally)
        {
            m_isRunning = false;
            Decelerate();
            return;
        }

        // When moving
        // Avoid moving faster diagonally
        if (isMovingOnXAxis && isMovingOnZAxis)
        {
            m_movement = Vector3.ClampMagnitude(m_movement, 1f);
        }

        float moveSpeed =
            m_baseMoveSpeed *
                ((m_isRunning)
                    ? m_runSpeedMultiplier
                    : 1f);
        // Convert move direction from local space to
        // global space for accurate Rigidbody movement
        Vector3 movementDirection =
            moveSpeed * transform.TransformDirection(m_movement);
        Vector3 velocity = m_rb.linearVelocity;
        velocity.x = movementDirection.x;
        velocity.z = movementDirection.z;

        m_rb.linearVelocity = velocity;
    }

    private void Decelerate()
    {
        // Apply horizontal drag
        Vector3 linearVel = m_rb.linearVelocity;
        m_rb.AddForce(
            new Vector3(
                -(linearVel.x * m_deceleration),
                0f,
                -(linearVel.z * m_deceleration)
            )
        );
    }

    private void CheckGrounding()
    {
        Vector3 center = transform.TransformPoint(m_capsule.center);
        float halfHeight = m_capsule.height * 0.5f;
        float bottomOffset = Mathf.Max(halfHeight - m_capsule.radius, 0f);
        Vector3 bottom = center + Vector3.down * bottomOffset;

        bool isGrounded = Physics.SphereCast(
            bottom + Vector3.up * 0.05f,
            m_groundCheckRadius,
            Vector3.down,
            out RaycastHit hit,
            m_groundCheckDistance,
            ~m_notJumpableLayers,
            QueryTriggerInteraction.Ignore
        );

        if (!isGrounded)
        {
            m_isAirborne = true;
            return;
        }

        m_isAirborne = IsTooSteep(hit.normal, m_maxSlopeAngle);
    }

    private void Jump()
    {
        Vector3 velocity = m_rb.linearVelocity;
        velocity.y = m_jumpForce;
        m_rb.linearVelocity = velocity;
    }

    private bool IsTooSteep(in Vector3 _groundNormal, in float _referenceAngle)
    {
        float groundDot = Vector3.Dot(_groundNormal, Vector3.up);
        float minGroundDot = Mathf.Cos(_referenceAngle * Mathf.Deg2Rad);

        return (groundDot < minGroundDot);
    }


    private void OnMove_Template(in InputAction.CallbackContext _context, ref float _axis)
    {
        if (_context.canceled)
        {
            _axis = 0f;
            return;
        }

        _axis = _context.ReadValue<float>();
    }


    public void OnMoveX(InputAction.CallbackContext _context)
    {
        OnMove_Template(_context, ref m_movement.x);
    }
    public void OnMoveZ(InputAction.CallbackContext _context)
    {
        OnMove_Template(_context, ref m_movement.z);
    }

    public void OnJump(InputAction.CallbackContext _context)
    {
        if (!m_rb)
        {
            Debug.LogWarning("Cannot jump: 'rb' is null.");
            return;
        }

        m_jumpTrigger = (_context.performed);
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

        m_isRunning = (_context.performed);
    }
}
