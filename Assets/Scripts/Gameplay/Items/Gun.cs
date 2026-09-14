using UnityEngine;
using TMPro;

public class Gun : Item
{
    [Header("-- Gun --")]
    [Header("- Gun Properties -")]
    [SerializeField] [Min(1)]
     private int m_shotsPerSecond = 4;
    [SerializeField] private int m_damagePerShot = 5;
    [SerializeField] [Min(0)]
     private int m_magazineCapacity = 10;
    [SerializeField] [Min(0)]
     private int m_startMagazineCount = 3;
    [SerializeField] [Min(0f)]
     private float m_reloadDuration = 1.5f;

    [Header("- UI -")]
    [SerializeField] private GameObject m_gunUI = null;

    [Header("- Misc -")]
    [SerializeField] private GameObject m_particlePrefab = null;

    private Collider m_collider = null;
    private Ray m_ray = new Ray();
    private TextMeshProUGUI m_ammoText = null;
    private ParticleSystem m_particleSys = null;
    private float m_shotCooldown = 0f;
    private float m_shotTimer = 0f;
    private float m_shotReach = 0f;
    private float m_reloadTimer = 0f;
    private int m_currentMagazineAmmo = 0;
    private int m_reserveAmmo = 0;
    private bool m_isShooting = false;
    private bool m_isReloading = false;
    private bool m_isPickedUpCpy = false;
    private bool m_reloadTextTrigger = true;

    private void Awake()
    {
        //m_Rb_.linearVelocity = Vector3.zero;
        m_collider = GetComponent<Collider>();

        m_currentMagazineAmmo = m_magazineCapacity;
        m_reserveAmmo =
            Mathf.Max(0, (m_startMagazineCount - 1) * m_magazineCapacity);
        m_shotCooldown = 1f / m_shotsPerSecond;

        if (m_particlePrefab)
        {
            m_particleSys =
                Instantiate(m_particlePrefab, transform)
                .GetComponent<ParticleSystem>();
        }
    }

    private void Start()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();

        if (!canvas)
        {
            return;
        }

        GameObject uiObj = Instantiate(m_gunUI, canvas.transform);
        m_ammoText = uiObj.GetComponentInChildren<TextMeshProUGUI>();

        TrySetAmmoTextVisibility(false);
    }

    private void Update()
    {
        if (m_isPickedUpCpy != m_IsPickedUp)
        {
            if (m_IsPickedUp)
            {
                OnEquip();
            }
            else
            {
                OnUnequip();
            }

            m_isPickedUpCpy = m_IsPickedUp;
        }

        if (!m_IsPickedUp)
        {
            return;
        }

        if (m_isReloading)
        {
            m_reloadTimer += Time.deltaTime;

            if (m_reloadTextTrigger && m_ammoText)
            {
                m_ammoText.text = $"Reloading.../{m_reserveAmmo}";
                m_reloadTextTrigger = false;
            }

            if (m_reloadTimer >= m_reloadDuration)
            {
                Reload();
                m_reloadTextTrigger = true;
            }

            return;
        }

        if (m_reserveAmmo > 0 && m_currentMagazineAmmo == 0)
        {
            m_isReloading = true;
            m_reloadTimer = 0f;

            return;
        }

        if (!m_isShooting || m_currentMagazineAmmo == 0)
        {
            return;
        }

        m_shotTimer += Time.deltaTime;

        if (m_shotTimer >= m_shotCooldown)
        {
            Shoot();
            m_shotTimer = 0f;
        }
    }


    private void Shoot()
    {
        m_ray.origin = m_HolderTransform_.position;
        m_ray.direction = m_HolderTransform_.forward;

        m_currentMagazineAmmo--;

        TryAdaptAmmoText();

        if (m_particleSys)
        {
            m_particleSys.Play();
        }

        if (!Physics.Raycast(
            m_ray,
            out RaycastHit info,
            m_shotReach
        ))
        {
            return;
        }

        Zombie zombie = info.transform.GetComponent<Zombie>();

        if (!zombie)
        {
            return;
        }

        zombie.TakeDamage(m_damagePerShot);
    }

    private void Reload()
    {
        int missingAmmo =
            m_magazineCapacity - m_currentMagazineAmmo;

        int ammoToReload =
            Mathf.Min(missingAmmo, m_reserveAmmo);

        m_currentMagazineAmmo += ammoToReload;
        m_reserveAmmo -= ammoToReload;

        m_reloadTimer = 0f;
        m_isReloading = false;

        TryAdaptAmmoText();
    }

    private void TrySetAmmoTextVisibility(bool _isVisible)
    {
        if (!m_ammoText)
        {
            return;
        }

        m_ammoText.alpha =
            (_isVisible)
                ? 1f
                : 0f;
    }

    private void TryAdaptAmmoText()
    {
        if (!m_ammoText)
        {
            return;
        }

        m_ammoText.text = $"{m_currentMagazineAmmo}/{m_reserveAmmo}";
    }


    public void GiveAmmos(int _ammoNb)
    {
        if (_ammoNb <= 0)
        {
            return;
        }

        m_reserveAmmo += _ammoNb;

        TryAdaptAmmoText();
    }

    public void TryReload()
    {
        if (m_isReloading ||
            m_reserveAmmo <= 0 ||
            m_currentMagazineAmmo == m_magazineCapacity)
        {
            return;
        }

        m_reloadTimer = 0f;
        m_isReloading = true;
    }

    public void OnEquip()
    {
        if (!m_HolderTransform_)
        {
            return;
        }

        m_Rb_.isKinematic = true;

        transform.SetParent(m_HolderTransform_);
        transform.localPosition =
            new Vector3(0.33f, -0.4f, 0.6f);
        transform.localRotation =
            Quaternion.Euler(0f, 75f, 70f);

        m_collider.enabled = false;

        TrySetAmmoTextVisibility(true);
        TryAdaptAmmoText();
    }
    public void OnUnequip()
    {
        m_Rb_.isKinematic = false;
        m_isShooting = false;

        TrySetAmmoTextVisibility(false);

        m_collider.enabled = true;

        transform.SetParent(null);
    }


    public void SetIsAiming(bool _isAiming)
    {
        Color colour =
            (_isAiming)
                ? Color.green
                : Color.red;
        string textColour = "<color=" + colour.ToString() + ">";
    }

    public void StartShooting(float _shotReach)
    {
        if (m_isReloading)
        {
            return;
        }

        m_shotReach = _shotReach;
        m_shotTimer = m_shotCooldown;
        m_isShooting = true;
    }
    public void StopShooting()
    {
        m_isShooting = false;
    }
}
