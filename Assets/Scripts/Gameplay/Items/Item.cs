using UnityEngine;

public class Item : MonoBehaviour
{
    protected delegate void OnPickupDelegate(object _optional = null);
    protected delegate void OnReleaseDelegate(object _optional = null);

    [Header("-- Item --")]
    [SerializeField] [Min(0.01f)]
    private float hitboxFactor = 1.25f;

    private Vector3 baseSize = Vector3.one;
    private float baseRadius = 0.5f;
    private float baseHeight = 2f;
    private float hitboxFactorCpy = 0f;
    private bool hitboxAdjustmentTrigger = false;
    private bool isPickedUp = false;

    protected OnPickupDelegate pickupDelegate_ = null;
    protected OnReleaseDelegate releaseDelegate_ = null;

    protected bool HitboxAdjustmentTrigger_
    {
        get => hitboxAdjustmentTrigger;
    }

    public bool IsPickedUp
    {
        get => isPickedUp;
    }

    protected void OnValidate()
    {
        if (hitboxFactorCpy != hitboxFactor)
        {
            hitboxAdjustmentTrigger = true;
            hitboxFactorCpy = hitboxFactor;
        }
    }


    protected void SetCurrentHitboxValuesAsDefault(Collider _collider)
    {
        switch (_collider)
        {
            case BoxCollider:
                {
                    baseSize = (_collider as BoxCollider).size;
                }
                break;
            case SphereCollider:
                {
                    baseRadius = (_collider as SphereCollider).radius;
                }
                break;
            case CapsuleCollider:
                {
                    CapsuleCollider cc = (_collider as CapsuleCollider);

                    baseRadius = cc.radius;
                    baseHeight = cc.height;
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
            Debug.LogWarning("Cannot adjust item hitbox: collider is null.");
            return;
        }

        switch (_collider)
        {
            case BoxCollider:
                {
                    (_collider as BoxCollider).size = hitboxFactor * baseSize;
                }
                break;
            case SphereCollider:
                {
                    (_collider as SphereCollider).radius = hitboxFactor * baseRadius;
                }
                break;
            case CapsuleCollider:
                {
                    CapsuleCollider cc = (_collider as CapsuleCollider);
                    cc.radius = hitboxFactor * baseRadius;
                    cc.height = hitboxFactor * baseHeight;
                }
                break;
            default:
                {
                }
                break;
        }

        hitboxAdjustmentTrigger = false;
    }


    public void OnPickup()
    {
        pickupDelegate_?.Invoke();
        isPickedUp = true;
    }
    public void OnRelease()
    {
        releaseDelegate_?.Invoke();
        isPickedUp = false;
    }
}
