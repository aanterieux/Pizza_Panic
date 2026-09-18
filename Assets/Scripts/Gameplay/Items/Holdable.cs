using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Holdable : Item
{
    [Header("-- Holdable --")]
    [SerializeField] [Min(0)]
    private int m_baseDamageOnHit = 1;

    private Collider m_holdableCollider = null;

    protected Transform m_hitTransform_ = null;
    protected bool m_isThrown_ = false;


    private void Awake()
    {
        if (!m_holdableCollider)
        {
            m_holdableCollider = GetComponent<Collider>();
        }

        SetCurrentHitboxValuesAsDefault(m_holdableCollider);
        AdjustColliderHitbox(m_holdableCollider);
    }


    protected new void OnValidate()
    {
        base.OnValidate();

        if (HitboxAdjustmentTrigger_)
        {
            if (!m_holdableCollider)
            {
                m_holdableCollider = GetComponent<Collider>();
            }

            AdjustColliderHitbox(m_holdableCollider);
        }
    }

    private void OnCollisionEnter(Collision _collision)
    {
        Transform collisionTransform = _collision.transform;

        if (IsPickedUp || collisionTransform.GetComponent<Item>())
        {
            return;
        }

        m_hitTransform_ = collisionTransform;

        if (!m_hitTransform_)
        {
            return;
        }

        Zombie zombie = m_hitTransform_.GetComponent<Zombie>();

        if (!zombie)
        {
            return;
        }

        zombie.TakeDamage(m_baseDamageOnHit);
    }

    private void OnCollisionExit(Collision _collision)
    {
        m_hitTransform_ = null;
    }


    private void ResetRigidbodyVelocity()
    {
        Rb_.linearVelocity = Vector3.zero;
        Rb_.angularVelocity = Vector3.zero;
    }


    public void OnDrop()
    {
        OnRelease();
        ResetRigidbodyVelocity();

        m_isThrown_ = false;
    }

    public void OnThrow(float _throwForce)
    {
        if (!m_holderTransform_)
        {
            return;
        }

        Vector3 throwDirection = m_holderTransform_.forward;

        OnRelease();

        Rb_.AddForce(
            _throwForce * throwDirection
            + 0.33f * _throwForce * Vector3.up,
            ForceMode.VelocityChange
        );

        m_isThrown_ = true;
    }
}
