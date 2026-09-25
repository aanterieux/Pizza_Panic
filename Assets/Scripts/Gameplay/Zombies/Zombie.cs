using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private enum ZombieState
    {
        IDLE,
        CHASE,
        ATTACK,
        DEAD
    }

    public enum ZombieEffect
    {
        NONE,

        SLOWNESS,
        BURN,
        FREEZE,

        COUNT
    }

    [Header("- Health -")]
    [SerializeField] private int m_health = 25;
    [SerializeField] private int m_maxHealth = 25;

    [Header("- Attack -")]
    [SerializeField] private float m_attackCooldown = 0.8f;
    [SerializeField] private int m_damage = 15;

    [Header("- Feedback -")]
    [SerializeField] private AudioClip m_attackSound = null;
    [SerializeField] private AudioClip m_hurtSound = null;
    [SerializeField] private AudioClip m_deathSound = null;
    [SerializeField] private Color m_hurtColour = Color.white;
    [SerializeField] [Min(0f)]
     private float m_visualFeedbackSpeed = 1f;

    [Header("- Misc -")]
    [SerializeField] private ZombieState m_state = ZombieState.CHASE;
    [SerializeField] private uint m_destinationUpdatesPerSecond = 16U;
    [SerializeField] private float m_burySpeed = 5f;
    [SerializeField] private float m_distanceToAttack = 0.5f;
    [SerializeField] private float m_distanceToChase = 0.75f;

    private const int EFFECT_COUNT = ((int)(ZombieEffect.COUNT)) - 2;

    private NavMeshAgent m_agent = null;
    private Transform m_playerTransform = null;
    private CapsuleCollider m_capsule = null;
    private MeshRenderer m_meshRenderer = null;
    private ZombieSpawner m_origin = null;
    private float[] m_effectsStrength = new float[EFFECT_COUNT];
    private float[] m_effectsDuration = new float[EFFECT_COUNT];
    private float[] m_effectsTimer = new float[EFFECT_COUNT];
    private bool[] m_hasEffect = new bool[EFFECT_COUNT];
    private float m_attackTimer = 0f;
    private float m_baseMoveSpeed = 0f;
    private float m_destinationUpdateTimer = 0f;
    private float m_feedbackTimer = 0f;
    private Color m_baseColour = Color.black;

    public bool m_IsAttacking
    {
        get => (m_state == ZombieState.ATTACK && m_attackTimer > 0f);
    }


    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_capsule = GetComponent<CapsuleCollider>();
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_baseColour = m_meshRenderer.sharedMaterial.color;

        VaryStatsAndSize();

        m_baseMoveSpeed = m_agent.speed;

        if (m_health <= 0)
        {
            m_state = ZombieState.DEAD;
        }

        if (m_state == ZombieState.DEAD)
        {
            m_health = 0;
        }
    }

    private void Start()
    {
        m_playerTransform = FindAnyObjectByType<PlayerController>().transform;
    }

    private void Update()
    {
        ManageStates();
        ManageEffects();

        if (m_feedbackTimer < 1f &&
            m_state != ZombieState.DEAD)
        {
            m_feedbackTimer += m_visualFeedbackSpeed * Time.deltaTime;
            m_meshRenderer.material.color =
                Color.Lerp(
                    m_hurtColour,
                    m_baseColour,
                    m_feedbackTimer
                );
        }
    }

    private void OnValidate()
    {
        if (m_state != ZombieState.DEAD)
        {
            if (m_maxHealth < m_health)
            {
                m_maxHealth = m_health;
            }
        }
    }


    private void VaryStatsAndSize()
    {
        int healthChange = Random.Range(-7, 16);
        float speedChange = Random.Range(-1.5f, 1.5f);
        float cooldownChange = Random.Range(-0.05f, 0.05f);
        int damageChange = Random.Range(-5, 6);

        m_health += healthChange;
        m_agent.speed += speedChange;
        m_attackCooldown += cooldownChange;
        m_damage += damageChange;

        float healthStrength =
            Mathf.InverseLerp(-7f, 15f, healthChange) * 2f - 1f;
        float speedStrength =
            Mathf.InverseLerp(-1.5f, 1.5f, speedChange) * 2f - 1f;
        float cooldownStrength =
            Mathf.InverseLerp(0.1f, -0.1f, cooldownChange) * 2f - 1f;
        float damageStrength =
            Mathf.InverseLerp(-5f, 5f, damageChange) * 2f - 1f;
        float combatStrength = (
                healthStrength +
                speedStrength +
                cooldownStrength +
                damageStrength
            ) / 5f;
        float statMean =
            Mathf.Lerp(0.66f, 1.25f, (combatStrength + 1f) / 2f);

        m_capsule.radius *= statMean;
        m_capsule.height *= statMean;
        m_capsule.transform.localScale *= statMean;
    }

    private void ManageStates()
    {
        switch (m_state)
        {
            case ZombieState.IDLE:
                {

                }
                break;
            case ZombieState.CHASE:
                {
                    if (!m_agent || !m_agent.enabled || !m_agent.isOnNavMesh)
                    {
                        return;
                    }

                    UpdateDestination();

                    if (GetDistanceToPlayer() <= m_distanceToAttack)
                    {
                        m_state = ZombieState.ATTACK;
                    }
                }
                break;
            case ZombieState.ATTACK:
                {
                    m_attackTimer += Time.deltaTime;

                    if (m_attackTimer > m_attackCooldown)
                    {
                        m_playerTransform
                            .GetComponent<PlayerStatManager>()
                            .TakeDamage(m_damage);

                        m_attackTimer = 0f;
                    }

                    if (GetDistanceToPlayer() > m_distanceToChase)
                    {
                        m_state = ZombieState.CHASE;
                    }
                }
                break;
            case ZombieState.DEAD:
                {
                    m_agent.enabled = false;
                    m_capsule.enabled = false;

                    transform.Translate(Time.deltaTime * m_burySpeed * Vector3.down);

                    if (transform.position.y + 1f < -0.5f)
                    {
                        if (m_origin)
                        {
                            m_origin.NotifyZombieDeath();
                        }

                        if (m_playerTransform)
                        {
                            Item playerItem =
                                m_playerTransform
                                .GetComponent<PlayerInventory>()
                                .CurrentItem;

                            if (playerItem)
                            {
                                (playerItem as Gun)
                                    .GiveAmmos(Random.Range(2, 12 + 1));
                            }
                        }

                        Destroy(gameObject);
                    }
                }
                break;
            default:
                {
                }
                break;
        }
    }

    private void ManageEffects()
    {
        if (m_state == ZombieState.DEAD)
        {
            return;
        }

        for (int i = 0; i < m_hasEffect.Length; ++i)
        {
            if (!m_hasEffect[i] || ((ZombieEffect)(i)) == ZombieEffect.NONE)
            {
                continue;
            }

            m_effectsTimer[i] += Time.deltaTime;

            if (m_effectsTimer[i] > m_effectsDuration[i])
            {
                m_hasEffect[i] = false;
                m_effectsTimer[i] = 0f;

                if (m_hasEffect[(int)(ZombieEffect.SLOWNESS)])
                {
                    m_agent.speed = m_baseMoveSpeed;
                }

                return;
            }
        }

        if (m_hasEffect[(int)(ZombieEffect.SLOWNESS)])
        {
            float normalisedSlowness =
                1f -
                0.01f * m_effectsStrength[(int)(ZombieEffect.SLOWNESS)];

            m_agent.speed = m_baseMoveSpeed * normalisedSlowness;
        }
    }

    private void UpdateDestination()
    {
        m_destinationUpdateTimer += Time.deltaTime;

        if (m_destinationUpdateTimer >= 1f / m_destinationUpdatesPerSecond)
        {
            m_agent.SetDestination(m_playerTransform.position);
            m_destinationUpdateTimer = 0f;
        }
    }

    public void TriggerAttack()
    {
        m_state = ZombieState.ATTACK;

        AudioManager.s_Instance.Play3D(m_attackSound, transform.position, AudioManager.AudioParams.s_Sound);
    }

    private void Die()
    {
        m_state = ZombieState.DEAD;
     
        AudioManager.s_Instance.Play3D(m_deathSound, transform.position, AudioManager.AudioParams.s_Sound);
    }

    private float GetDistanceToPlayer()
    {
        return
            Vector3.Distance(
                transform.position,
                m_playerTransform.position
            );
    }

    private void TriggerDamageFeedback()
    {
        m_meshRenderer.material.color = Color.white;
        m_feedbackTimer = 0f;

        AudioManager.s_Instance.Play3D(m_hurtSound, transform.position, AudioManager.AudioParams.s_Sound);
    }


    public void LinkToSpawner(ZombieSpawner _spawner)
    {
        m_origin = _spawner;
    }

    public void TakeDamage(int _damage)
    {
        if (m_state == ZombieState.DEAD)
        {
            return;
        }

        m_health -= _damage;

        TriggerDamageFeedback();

        if (m_health <= 0)
        {
            Die();
        }
    }

    public void GiveEffect(ZombieEffect _effect, float _strengthInPercentage, float _duration)
    {
        if (_effect == ZombieEffect.NONE ||
            _effect == ZombieEffect.COUNT)
        {
            return;
        }

        int effectIndex = (int)(_effect);

        if (m_hasEffect[effectIndex])
        {
            return;
        }

        m_hasEffect[effectIndex] = true;
        m_effectsStrength[effectIndex] = _strengthInPercentage;
        m_effectsDuration[effectIndex] = _duration;
        m_effectsTimer[effectIndex] = 0f;
    }
}
