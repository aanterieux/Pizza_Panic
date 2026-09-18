using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Item : MonoBehaviour
{
    [Header("-- Item --")]
    [SerializeField] [Min(0.01f)]
     private float m_hitboxFactor = 1.25f;
    [SerializeField] [Min(0.75f)]
     private float m_distanceWithHolder = 1.5f;

    private Rigidbody m_rb = null;
    private AudioSource m_audioSource = null;
    private Vector3 m_baseSize = Vector3.one;
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
    protected AudioSource AudioPlayer_
    {
        get
        {
            if (!m_audioSource)
            {
                m_audioSource = GetComponent<AudioSource>();
            }

            return m_audioSource;
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


    private void Awake()
    {
        AudioPlayer_.playOnAwake = false;
    }

    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }


    protected void OnValidate()
    {
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
                    m_baseSize = (_collider as BoxCollider).size;
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
                    (_collider as BoxCollider).size = m_hitboxFactor * m_baseSize;
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
            LogUtils.LogWarning("Cannot pickup item: _holderTransform hasn't changed");
            return;
        }

        m_holderTransform_ = _holderTransform;

        Rb_.isKinematic = true;
        Rb_.useGravity = false;

        transform.SetParent(
            m_holderTransform_
        );
        transform.localPosition = new Vector3(0f, 0f, m_distanceWithHolder);

        m_isPickedUp = true;
    }
    public void OnRelease()
    {
        Rb_.isKinematic = false;
        Rb_.useGravity = true;

        transform.SetParent(null);

        m_holderTransform_ = null;
        m_isPickedUp = false;
    }
}
