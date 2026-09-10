using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActionManager : PlayerComponent
{
    private PlayerStatManager m_statManager = null;
    private PlayerInventory m_inventory = null;
    private Zombie m_target = null;
    private Ray m_ray = new Ray();
    private float m_meleeAttackTimer = 0f;
    private float m_meleeAttackCooldown = 0f;
    private bool m_meleeAttackTrigger = false;

    private void Awake()
    {
        m_statManager = GetComponent<PlayerStatManager>();
        m_inventory = GetComponent<PlayerInventory>();

        m_meleeAttackCooldown = m_statManager.m_MeleeAttackCooldown;
    }

    private void FixedUpdate()
    {

    }

    private void Update()
    {
        if (m_meleeAttackTimer < m_meleeAttackCooldown)
        {
            m_meleeAttackTimer += Time.deltaTime;
        }

        if (m_meleeAttackTrigger &&
            m_meleeAttackTimer >= m_meleeAttackCooldown)
        {
            if (m_target)
            {
                m_target.TakeDamage(m_statManager.m_MeleeAttackDamage);
            }

            m_meleeAttackTimer = 0f;
            m_meleeAttackTrigger = false;
        }
    }


    private void TriggerMeleeAttack()
    {
        m_meleeAttackTrigger = true;
    }
    private void StopMeleeAttack()
    {
        m_meleeAttackTrigger = false;
    }


    private void TryPickupItem(Item _item)
    {
        if (_item == null ||
            _item.m_IsPickedUp ||
            m_inventory.m_CurrentItem == _item)
        {
            return;
        }

        m_inventory.SetCurrentItem(_item);
        _item.OnPickup(m_CamTransform_);
    }
    private void DropItem(Item _item)
    {
        if (!_item)
        {
            return;
        }

        if (_item is Holdable || _item is Consumable)
        {
            (_item as Holdable).OnDrop();
            m_inventory.ClearCurrentItem();
            return;
        }

        m_inventory.SetCurrentItem(null);
    }

    private void ThrowItem(Holdable _throwableItem, float _throwForce)
    {
        if (!_throwableItem)
        {
            return;
        }

        _throwableItem.OnThrow(_throwForce);
        m_inventory.ClearCurrentItem();
    }

    private void UpdateRayOriginAndDirection()
    {
        m_ray.origin = m_CamTransform_.position;
        m_ray.direction = m_CamTransform_.forward;
    }


    // Primary action:
    //   - Throw fists
    //   - Shoot
    //   - Throw picked up object
    public void OnPrimaryAction(InputAction.CallbackContext _context)
    {
        UpdateRayOriginAndDirection();

        float maxRayDistance = 0f;
        Item currentItem = m_inventory.m_CurrentItem;

        switch (currentItem)
        {
            case null:
                {
                    maxRayDistance = m_statManager.m_MeleeAttackReach;
                }
                break;
            default:
                {
                    maxRayDistance = m_statManager.m_RangedAttackReach;
                }
                break;
        }

        Transform targetTransform = null;

        switch (currentItem)
        {
            case null:
                {
                    if (_context.performed)
                    {
                        bool foundSomething =
                            Physics.Raycast(
                                m_ray,
                                out RaycastHit info,
                                maxRayDistance
                        );

                        if (foundSomething)
                        {
                            targetTransform = info.transform;

                            if (targetTransform)
                            {
                                m_target = targetTransform.GetComponent<Zombie>();
                            }
                        }

                        TriggerMeleeAttack();
                    }
                    else if (_context.canceled)
                    {
                        m_target = null;
                        StopMeleeAttack();
                    }
                }
                break;
            case Gun:
                {
                    Gun gun = (currentItem as Gun);

                    if (_context.started)
                    {
                        gun.StartShooting(m_statManager.m_RangedAttackReach);
                    }
                    else if (_context.canceled)
                    {
                        gun.StopShooting();
                    }
                }
                break;
            case Holdable:
                {
                    Holdable holdable = (currentItem as Holdable);

                    if (_context.started && holdable.m_IsPickedUp)
                    {
                        ThrowItem(holdable, m_statManager.m_ThrowForce);
                    }
                }
                break;
            default:
                {
                }
                break;
        }
    }

    // Secondary action:
    //   - Pick object up
    //   - Drop picked up object
    public void OnSecondaryAction(InputAction.CallbackContext _context)
    {
        if (!_context.started)
        {
            return;
        }

        UpdateRayOriginAndDirection();

        Item item = null;

        bool hasFoundNothing =
            !Physics.Raycast(
                m_ray,
                out RaycastHit info,
                m_statManager.m_PickupReach
            );
        Transform targetTransform = info.transform;

        if (hasFoundNothing ||
            targetTransform == null)
        {
            return;
        }

        item = targetTransform.GetComponent<Item>();

        if (!item)
        {
            return;
        }

        if (!item.m_IsPickedUp)
        {
            TryPickupItem(item);
            return;
        }

        if (item is Holdable || item is Consumable)
        {
            DropItem(item as Holdable);
        }
    }

    public void OnReload(InputAction.CallbackContext _context)
    {
        if (!_context.started)
        {
            return;
        }

        Item item = m_inventory.m_CurrentItem;

        if (!item)
        {
            return;
        }

        Gun gun = item as Gun;

        if (!gun)
        {
            return;
        }

        gun.TryReload();
    }

    public void OnInteract(InputAction.CallbackContext _context)
    {

    }

    public void OnSwitchWeapon(InputAction.CallbackContext _context)
    {

    }
}
