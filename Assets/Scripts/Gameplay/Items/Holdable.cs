using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Holdable : Item
{
    [Header("-- Holdable --")]
    [SerializeField] [Min(0)]
    private int baseDamageOnHit = 1;

    private Rigidbody rb = null;
    private Transform holder = null;
    private Collider holdableCollider = null;
    private float distanceWithHolder = 1f;

    protected Transform hitTransform_ = null;
    protected bool isThrown_ = false;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (!holdableCollider)
        {
            holdableCollider = GetComponent<Collider>();
        }

        base.SetCurrentHitboxValuesAsDefault(holdableCollider);
        base.AdjustColliderHitbox(holdableCollider);
    }

    protected void Update()
    {
        if (!IsPickedUp)
        {
            return;
        }

        transform.position =
            holder.position
            + distanceWithHolder * holder.forward;
    }

    protected new void OnValidate()
    {
        base.OnValidate();

        if (base.HitboxAdjustmentTrigger_)
        {
            if (!holdableCollider)
            {
                holdableCollider = GetComponent<Collider>();
            }

            base.AdjustColliderHitbox(holdableCollider);
        }
    }

    private void OnCollisionEnter(Collision _collision)
    {
        Transform collisionTransform = _collision.transform;

        if (IsPickedUp || collisionTransform.GetComponent<Item>())
        {
            return;
        }

        hitTransform_ = collisionTransform;

        if (!hitTransform_)
        {
            return;
        }

        Zombie zombie = hitTransform_.GetComponent<Zombie>();

        if (!zombie)
        {
            return;
        }

        zombie.TakeDamage(baseDamageOnHit);
    }

    private void OnCollisionExit(Collision _collision)
    {
        hitTransform_ = null;
    }


    private void ResetRigidbodyVelocity()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }


    public void OnHold(Transform _holder)
    {
        if (!_holder)
        {
            return;
        }

        holder = _holder;
        distanceWithHolder = Vector3.Distance(transform.position, holder.position);

        if (!rb)
        {
            rb = GetComponent<Rigidbody>();
        }

        ResetRigidbodyVelocity();
        rb.useGravity = false;

        isThrown_ = false;
    }

    public void OnDrop()
    {
        OnRelease();

        holder = null;
        distanceWithHolder = 0f;
        
        ResetRigidbodyVelocity();
        rb.useGravity = true;

        isThrown_ = false;
    }

    public void OnThrow(float _throwForce)
    {
        OnRelease();

        isThrown_ = true;
        rb.useGravity = true;

        rb.AddForce(
            _throwForce * holder.forward
            + 0.33f * _throwForce * Vector3.up,
            ForceMode.VelocityChange
        );
    }
}
