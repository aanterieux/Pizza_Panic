using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActionManager : MonoBehaviour
{
    [SerializeField] private Transform camTransform = null;

    private PlayerStatManager statManager = null;
    private PlayerInventory inventory = null;
    private Zombie target = null;
    private Ray ray = new Ray();
    private float meleeAttackTimer = 0f;
    private float meleeAttackCooldown = 0f;
    private bool meleeAttackTrigger = false;

    private void Awake()
    {
        statManager = GetComponent<PlayerStatManager>();
        inventory = GetComponent<PlayerInventory>();

        meleeAttackCooldown = statManager.MeleeAttackCooldown;
    }

    private void FixedUpdate()
    {

    }

    private void Update()
    {
        if (meleeAttackTimer < meleeAttackCooldown)
        {
            meleeAttackTimer += Time.deltaTime;
        }

        if (meleeAttackTrigger &&
            meleeAttackTimer >= meleeAttackCooldown)
        {
            if (target)
            {
                target.TakeDamage(statManager.MeleeAttackDamage);
            }

            meleeAttackTimer = 0f;
            meleeAttackTrigger = false;
        }
    }


    private void TriggerMeleeAttack()
    {
        meleeAttackTrigger = true;
    }
    private void StopMeleeAttack()
    {
        meleeAttackTrigger = false;
    }


    private void PickupItem(Holdable _pickableItem)
    {
        inventory.SetCurrentItem(_pickableItem);
        _pickableItem.OnHold(camTransform);
    }

    private void DropItem(Holdable _pickableItem)
    {
        inventory.SetCurrentItem(null);
        _pickableItem.OnDrop();
    }

    private void ConsumeItem(Consumable _consumableItem)
    {

    }

    private void ThrowItem(Holdable _throwableItem, float _throwForce)
    {
        _throwableItem.OnThrow(_throwForce);
    }

    private void EquipGun(Gun _gunToEquip)
    {
        inventory.SetCurrentItem(_gunToEquip);
        _gunToEquip.OnEquip(camTransform);
    }


    private void UpdateRayOriginAndDirection()
    {
        ray.origin = camTransform.position;
        ray.direction = camTransform.forward;
    }


    // Primary action:
    //   - Throw fists
    //   - Shoot
    //   - Throw picked up object
    public void OnPrimaryAction(InputAction.CallbackContext _context)
    {
        UpdateRayOriginAndDirection();

        float maxRayDistance = 0f;
        Item currentItem = inventory.CurrentItem;

        switch (currentItem)
        {
            case null:
                {
                    maxRayDistance = statManager.MeleeAttackReach;
                }
                break;
            default:
                {
                    maxRayDistance = statManager.RangedAttackReach;
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
                                ray,
                                out RaycastHit info,
                                maxRayDistance
                        );

                        if (foundSomething)
                        {
                            targetTransform = info.transform;

                            if (targetTransform)
                            {
                                target = targetTransform.GetComponent<Zombie>();
                            }
                        }

                        TriggerMeleeAttack();
                    }
                    else if (_context.canceled)
                    {
                        target = null;
                        StopMeleeAttack();
                    }
                }
                break;
            case Gun:
                {
                    Gun gun = (currentItem as Gun);

                    if (_context.started)
                    {
                        gun.StartShooting(
                            statManager.RangedAttackReach,
                            camTransform
                        );
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

                    if (_context.started && holdable.IsPickedUp)
                    {
                        ThrowItem(holdable, statManager.ThrowForce);
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
    //   - Aim
    //   - Pick object up
    //   - Let go of picked up object
    public void OnSecondaryAction(InputAction.CallbackContext _context)
    {
        if (!_context.started)
        {
            return;
        }

        UpdateRayOriginAndDirection();

        bool nothingFound =
            !Physics.Raycast(
                ray,
                out RaycastHit info,
                statManager.PickupReach
            );
        Transform itemTransform = info.transform;

        if (nothingFound || itemTransform == null)
        {
            return;
        }

        Item item = itemTransform.GetComponent<Item>();

        // When looking at a gun
        if (item is Gun)
        {
            EquipGun(item as Gun);
            return;
        }

        // When looking at any other item
        if (item is Holdable || item is Consumable)
        {
            if (_context.started)
            {
                Holdable holdable = item as Holdable;

                if (!holdable.IsPickedUp)
                {
                    PickupItem(holdable);
                }
                else
                {
                    DropItem(holdable);
                }
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext _context)
    {

    }

    public void OnSwitchWeapon(InputAction.CallbackContext _context)
    {

    }
}
