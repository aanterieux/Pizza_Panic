using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    private struct TransformData
    {
        private Vector3 m_position;
        private Quaternion m_rotation;
        private Vector3 m_scale;
        private Transform m_parent;

        public Vector3 Position
        {
            get => m_position;
        }
        public Quaternion Rotation
        {
            get => m_rotation;
        }
        public Vector3 Scale
        {
            get => m_scale;
        }
        public Transform Parent
        {
            get => m_parent;
        }

        public TransformData(Vector3? _position = null, Quaternion? _rotation = null, Vector3? _scale = null, Transform _parent = null)
        {
            m_position = _position ?? Vector3.zero;
            m_rotation = _rotation ?? Quaternion.identity;
            m_scale = _scale ?? Vector3.one;
            m_parent = _parent;
        }


        public static TransformData FromTransform(Transform _transform)
        {
            return
                new TransformData(
                    _transform.position,
                    _transform.rotation,
                    _transform.localScale,
                    _transform.parent
                );
        }


        public static bool operator ==(TransformData _left, TransformData _right)
        {
            return (
                _left.m_rotation == _right.m_rotation &&
                _left.m_scale == _right.m_scale &&
                _left.m_position == _right.m_position
            );
        }
        public static bool operator !=(TransformData _left, TransformData _right)
        {
            return !(_left == _right);
        }
    }

    [Header("-- Item --")]
    [SerializeField] [Min(0.01f)] private float m_hitboxFactor = 1.25f;
    [SerializeField] [Min(0.75f)] private float m_distanceWithHolder = 1.5f;

    private Rigidbody m_rb = null;
    private Vector3 m_initialColliderSize = Vector3.one;
    private TransformData m_initialTranformData = new();
    private TransformData m_initialTansformDataCpy = default;
    private float m_baseRadius = 0.5f;
    private float m_baseHeight = 2f;
    private float m_hitboxFactorCpy = 0f;
    private bool m_hitboxAdjustmentTrigger = false;
    private bool m_isPickedUp = false;

    protected Transform m_holderTransform_ = null;

    protected Rigidbody Rb_
    {
        get
        {
            if (!m_rb)
            {
                m_rb = GetComponent<Rigidbody>();
            }

            return m_rb;
        }
    }
    protected Transform HolderTransform_
    {
        get => m_holderTransform_;
    }
    protected bool HitboxAdjustmentTrigger_
    {
        get => m_hitboxAdjustmentTrigger;
    }

    public bool IsPickedUp
    {
        get => m_isPickedUp;
    }


    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    protected void OnValidate()
    {
        if (m_initialTansformDataCpy != m_initialTranformData)
        {
            UpdateInitialTransform();
            m_initialTansformDataCpy = m_initialTranformData;
        }

        if (m_hitboxFactorCpy != m_hitboxFactor)
        {
            m_hitboxAdjustmentTrigger = true;
            m_hitboxFactorCpy = m_hitboxFactor;
        }
    }


    protected void SetCurrentHitboxValuesAsDefault(Collider _collider)
    {
        switch (_collider)
        {
            case BoxCollider:
                {
                    m_initialColliderSize = (_collider as BoxCollider).size;
                }
                break;
            case SphereCollider:
                {
                    m_baseRadius = (_collider as SphereCollider).radius;
                }
                break;
            case CapsuleCollider:
                {
                    CapsuleCollider cc = (_collider as CapsuleCollider);

                    m_baseRadius = cc.radius;
                    m_baseHeight = cc.height;
                }
                break;
            default:
                {
                }
                break;
        }
    }

    protected void AdjustColliderHitbox(Collider _collider)
    {
        if (!_collider)
        {
            LogUtils.LogWarning("Cannot adjust item hitbox: collider is null");
            return;
        }

        switch (_collider)
        {
            case BoxCollider:
                {
                    (_collider as BoxCollider).size = m_hitboxFactor * m_initialColliderSize;
                }
                break;
            case SphereCollider:
                {
                    (_collider as SphereCollider).radius = m_hitboxFactor * m_baseRadius;
                }
                break;
            case CapsuleCollider:
                {
                    CapsuleCollider cc = (_collider as CapsuleCollider);
                    cc.radius = m_hitboxFactor * m_baseRadius;
                    cc.height = m_hitboxFactor * m_baseHeight;
                }
                break;
            default:
                {
                }
                break;
        }

        m_hitboxAdjustmentTrigger = false;
    }


    public void UpdateInitialTransform(bool _fromEditorButton = false)
    {
        m_initialTranformData = TransformData.FromTransform(transform);

        if (_fromEditorButton)
        {
            m_initialTansformDataCpy = m_initialTranformData;
        }
    }

    public void OnPickup(Transform _holderTransform)
    {
        bool isHolderNull = (_holderTransform == null);

        if (isHolderNull)
        {
            LogUtils.LogWarning("Cannot pickup item: _holderTransform is null");
            return;
        }

        bool isSameHolder = (_holderTransform == m_holderTransform_);

        if (isSameHolder)
        {
            LogUtils.LogWarning("Cannot pickup item: it is already picked up");
            return;
        }

        m_holderTransform_ = _holderTransform;

        Rb_.isKinematic = true;
        Rb_.useGravity = false;

        transform.SetParent(m_holderTransform_);
        transform.localPosition = new Vector3(0f, 0f, m_distanceWithHolder);

        m_isPickedUp = true;
    }
    public void OnRelease()
    {
        Rb_.isKinematic = false;
        Rb_.useGravity = true;

        m_holderTransform_ = null;
        m_isPickedUp = false;

        if (!Application.isPlaying)
        {
            transform.SetParent(m_initialTranformData.Parent);
            transform.position = m_initialTranformData.Position;
            transform.rotation = m_initialTranformData.Rotation;
            transform.localScale = m_initialTranformData.Scale;

            return;
        }

        transform.SetParent(null);
    }
}
