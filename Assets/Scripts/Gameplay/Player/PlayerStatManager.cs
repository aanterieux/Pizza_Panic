using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatManager : PlayerComponent
{
    [Header("- Health -")]
    [SerializeField] [Min(0)]
     private int m_health = 100;
    [SerializeField] [Min(1)]
     private int m_maxHealth = 100;
    [SerializeField] [Min(0f)]
    private float m_regenerationDelay = 2f;
    [SerializeField] [Min(0f)]
     private float m_regenerationsPerSec = 10f;
    [SerializeField] [Range(0, 100)]
    private int m_healthPerStep = 20;

    [Header("- Attack -")]
    [SerializeField] [Min(1.5f)]
     private float m_rangedAttackReach = 15f;
    [SerializeField] [Min(0.5f)]
     private float m_meleeAttackReach = 0.75f;
    [SerializeField] [Min(0f)]
     private float m_meleeAttackCooldown = 0.5f;
    [SerializeField] [Min(0)]
     private int m_meleeAttackDamage = 2;

    [Header("- UI- ")]
    [SerializeField] private GameObject m_playerUI = null;

    [Header("- Misc -")]
    [SerializeField] [Min(0f)]
     private float m_pickupReach = 5f;
    [SerializeField] [Min(0f)]
     private float m_throwForce = 10f;

    private Transform m_canvasTransform = null;
    private Image m_damageOverlay = null;
    private TextMeshProUGUI m_healthTextValue = null;
    private float m_regenerationTimer = 0f;
    private int m_healthCpy = 0;
    private int m_healthBuffer = 0;
    private bool m_regenerationDelayTrigger = true;

    public float m_RangedAttackReach
    {
        get => m_rangedAttackReach;
    }
    public float m_MeleeAttackReach
    {
        get => m_meleeAttackReach;
    }
    public float m_MeleeAttackCooldown
    {
        get => m_meleeAttackCooldown;
    }
    public int m_MeleeAttackDamage
    {
        get => m_meleeAttackDamage;
    }
    public float m_PickupReach
    {
        get => m_pickupReach;
    }
    public float m_ThrowForce
    {
        get => m_throwForce;
    }
    public int m_Health
    {
        get => m_health;
    }


    private void Awake()
    {
        m_healthBuffer = m_health;
    }

    private void Start()
    {
        m_canvasTransform = FindAnyObjectByType<Canvas>().transform;

        if (!m_canvasTransform)
        {
            return;
        }

        GameObject uiObj = Instantiate(
            m_playerUI,
            m_canvasTransform
        );

        if (!uiObj)
        {
            return;
        }

        Transform uiTransform = uiObj.transform;

        if (!uiTransform)
        {
            return;
        }

        m_damageOverlay =
            uiTransform
                .Find("DamageOverlay")
                .GetComponent<Image>();
        m_healthTextValue =
            uiTransform
                .Find("HealthText_Text")
                .GetChild(0)
                .GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (m_health < m_maxHealth)
        {
            if (m_regenerationDelayTrigger)
            {
                m_regenerationTimer = -m_regenerationDelay;
                m_regenerationDelayTrigger = false;
            }

            RegenerateHealth();
        }

        if (m_healthCpy != m_health)
        {
            TryAdaptDamageOverlayAlpha();
            m_healthCpy = m_health;
        }
    }

    private void OnTriggerEnter(Collider _collider)
    {
        Zombie zombie = _collider.GetComponent<Zombie>();

        if (zombie && !zombie.m_IsAttacking)
        {
            zombie.TriggerAttack();
        }
    }

    private void OnValidate()
    {
        if (m_healthBuffer != m_health)
        {
            TryAdaptHealthText();
            m_healthBuffer = m_health;
        }

        if (m_maxHealth < m_health)
        {
            // When game is running
            // => Clamp health normally
            if (Application.isPlaying)
            {
                m_health = m_maxHealth;
            }
            // When game is not running
            // => Adapt maxHealth to health
            else
            {
                m_maxHealth = m_health;
            }
        }
    }


    private void RegenerateHealth()
    {
        m_regenerationTimer += Time.deltaTime;

        if (m_regenerationTimer >= 1f / m_regenerationsPerSec)
        {
            m_health += m_healthPerStep;
            m_regenerationTimer = 0f;
        }

        if (m_health > m_maxHealth)
        {
            m_health = m_maxHealth;
        }

        m_healthBuffer = m_health;

        TryAdaptHealthText();
    }

    private void TryAdaptDamageOverlayAlpha()
    {
        if (!m_damageOverlay)
        {
            return;
        }

        Color overlayColour = m_damageOverlay.color;
        overlayColour.a = (1f - (float)(m_health) / m_maxHealth);
        m_damageOverlay.color = overlayColour;
    }

    private void TryAdaptHealthText()
    {
        if (!m_healthTextValue)
        {
            return;
        }

        float percentage = m_health / (float)(m_maxHealth);
        Color textColour = Color.black;
        textColour.r = 1f - percentage;
        textColour.g = percentage;

        m_healthTextValue.text = Mathf.Max(m_health, 0).ToString();
        m_healthTextValue.color = textColour;
    }


    public void TakeDamage(int _damage)
    {
        m_health -= _damage;
        m_regenerationDelayTrigger = true;
        m_healthBuffer = m_health;

        TryAdaptHealthText();
    }
}
