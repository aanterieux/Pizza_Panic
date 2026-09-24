using UnityEngine;

public class Consumable : Holdable
{
    private enum ConsumableType
    {
        CHEESE_SAUCE,
        BBQ_SAUCE,
        MEXICAN_SAUCE
    }

    [SerializeField] private ConsumableType m_type = ConsumableType.CHEESE_SAUCE;
    [SerializeField] private AudioClip m_mexicanSauceMusic = null;

    private Collider m_consumableCollider = null;
    private AudioSource m_musicPlayer = null;
    private bool m_hasStartedMusicOnce = false;
    private bool m_canDestroySelf = false;

    private void Awake()
    {
        if (!m_consumableCollider)
        {
            m_consumableCollider = GetComponent<Collider>();
        }

        if (m_type == ConsumableType.MEXICAN_SAUCE)
        {
            m_canDestroySelf = false;
        }

        SetCurrentHitboxValuesAsDefault(m_consumableCollider);
        AdjustColliderHitbox(m_consumableCollider);
    }

    private void Update()
    {
        if (m_isThrown_ && m_hitTransform_)
        {
            m_isThrown_ = false;

            if (!m_hasStartedMusicOnce && m_type == ConsumableType.MEXICAN_SAUCE)
            {
                m_musicPlayer =
                    AudioManager.s_Instance.Play2D(m_mexicanSauceMusic);
                m_hasStartedMusicOnce = true;
            }

            Zombie zombie = m_hitTransform_.GetComponent<Zombie>();

            if (zombie)
            {
                switch (m_type)
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

                m_canDestroySelf = true;
            }
        }

        if (m_hasStartedMusicOnce && m_type == ConsumableType.MEXICAN_SAUCE)
        {
            if (m_musicPlayer &&
                !m_musicPlayer.isPlaying)
            {
                m_canDestroySelf = true;
            }
        }

        if (m_canDestroySelf)
        {
            Destroy(gameObject);
        }
    }

    private new void OnValidate()
    {
        base.OnValidate();
    }
}
