using UnityEngine;

public class Consumable : Holdable
{
    private enum ConsumableType
    {
        CHEESE_SAUCE,
        BBQ_SAUCE,
        MEXICAN_SAUCE
    }

    [SerializeField] private ConsumableType type = ConsumableType.CHEESE_SAUCE;
    [SerializeField] private AudioClip mexicanSauceMusic = null;

    private Collider consumableCollider = null;
    private AudioSource audioSource = null;
    private bool hasStartedMusicOnce = false;
    private bool canDestroySelf = true;

    private void Awake()
    {
        if (!consumableCollider)
        {
            consumableCollider = GetComponent<Collider>();
        }

        if (type == ConsumableType.MEXICAN_SAUCE)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = mexicanSauceMusic;

            canDestroySelf = false;
        }

        base.SetCurrentHitboxValuesAsDefault(consumableCollider);
        base.AdjustColliderHitbox(consumableCollider);
    }

    private new void Update()
    {
        base.Update();

        if (isThrown_ && hitTransform_)
        {
            isThrown_ = false;

            if (audioSource && audioSource.clip &&
                !hasStartedMusicOnce)
            {
                audioSource.Play();
                hasStartedMusicOnce = true;
            }

            Zombie zombie = hitTransform_.GetComponent<Zombie>();

            if (zombie)
            {
                switch (type)
                {
                    case ConsumableType.CHEESE_SAUCE:
                        {
                            zombie.TakeDamage(4);
                            zombie.GiveEffect(Zombie.ZombieEffect.SLOWNESS, 30f, 5f);
                        }
                        break;
                    case ConsumableType.BBQ_SAUCE:
                        {

                        }
                        break;
                    case ConsumableType.MEXICAN_SAUCE:
                        {
                            //zombie.StartDancing();
                        }
                        break;
                    default:
                        {
                        }
                        break;
                }
            }
        }

        if (hasStartedMusicOnce && type == ConsumableType.MEXICAN_SAUCE)
        {
            if (!audioSource.isPlaying)
            {
                canDestroySelf = true;
            }
        }

        if (canDestroySelf)
        {
            Destroy(gameObject);
        }
    }

    private new void OnValidate()
    {
        base.OnValidate();
    }
}
