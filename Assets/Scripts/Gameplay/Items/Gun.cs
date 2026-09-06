using UnityEngine;
using TMPro;

public class Gun : Item
{
    [Header("-- Gun --")]
    [SerializeField] private TextMeshProUGUI ammoText = null;
    [SerializeField] private int shotsPerSecond = 4;
    [SerializeField] private int shotDamage = 5;
    [SerializeField] private int ammosPerRound = 10;
    [SerializeField] private int roundNb = 3;
    [SerializeField] private float reloadDuration = 1.5f;

    private new Collider collider = null;
    private Transform shooterTransform = null;
    private Ray ray = new Ray();
    private float shootCooldown = 0f;
    private float shootTimer = 0f;
    private float shotReach = 0f;
    private float reloadTimer = 0f;
    private int remainingAmmos = 0;
    private bool isShooting = false;
    private bool isReloading = false;

    private void Awake()
    {
        collider = GetComponent<Collider>();

        shootCooldown = 1f / shotsPerSecond;
        remainingAmmos = ammosPerRound;

        TrySetAmmoTextVisibility(false);

        pickupDelegate_ += OnEquip;
        releaseDelegate_ += OnUnequip;
    }

    private void Update()
    {
        if (!IsPickedUp)
        {
            return;
        }

        if (isReloading)
        {
            reloadTimer += Time.deltaTime;

            if (reloadTimer >= reloadDuration)
            {
                Reload();
            }

            return;
        }

        if (remainingAmmos == 0)
        {
            if (roundNb == 0)
            {
                return;
            }

            isReloading = true;
            reloadTimer = 0f;

            Debug.Log("<color=orange>Reloading...</color>");

            return;
        }

        if (!isShooting)
        {
            return;
        }

        shootTimer += Time.deltaTime;

        if (shootTimer >= shootCooldown)
        {
            Shoot();
            shootTimer = 0f;
        }
    }


    private void Shoot()
    {
        ray.origin = shooterTransform.position;
        ray.direction = shooterTransform.forward;

        Debug.Log(
            $"<color=yellow>PEW @ {Time.time:F3}</color>"
        );

        remainingAmmos--;

        TryAdaptAmmoText();

        if (!Physics.Raycast(
            ray,
            out RaycastHit info,
            shotReach
        ))
        {
            Debug.Log("<color=red>MISS</color>");
            return;
        }

        Debug.Log(
            $"<color=green>HIT: {info.transform.name}</color>"
        );

        Zombie zombie = info.transform.GetComponent<Zombie>();

        if (!zombie)
        {
            Debug.Log(
                $"Hit {info.transform.name}, but it isn't a Zombie."
            );

            return;
        }

        Debug.Log(
            $"<color=magenta>TAKING DAMAGE @ {Time.time:F3}</color>"
        );

        zombie.TakeDamage(shotDamage);

        Debug.Log(
            $"<color=cyan>DAMAGE CALLED @ {Time.time:F3}</color>"
        );
    }

    private void Reload()
    {
        remainingAmmos = ammosPerRound;
        roundNb--;

        reloadTimer = 0f;
        isReloading = false;

        TryAdaptAmmoText();

        Debug.Log("<color=cyan>Reloaded!</color>");
    }

    private void TrySetAmmoTextVisibility(bool _isVisible)
    {
        if (!ammoText)
        {
            return;
        }

        ammoText.alpha =
            (_isVisible)
                ? 1f
                : 0f;
    }

    private void TryAdaptAmmoText()
    {
        if (!ammoText)
        {
            return;
        }

        ammoText.text = $"{remainingAmmos}\n-----\n{roundNb}";
    }


    public void GiveAmmos(int _ammoNb)
    {
        remainingAmmos += _ammoNb;
    }

    public void OnEquip(Transform _holderTransform)
    {
        if (!_holderTransform)
        {
            return;
        }

        OnPickup();

        shooterTransform = _holderTransform;
        transform.SetParent(shooterTransform);

        transform.localPosition =
            new Vector3(0.33f, -0.4f, 0.6f);

        transform.localRotation =
            Quaternion.Euler(0f, 75f, 70f);

        collider.enabled = false;

        TrySetAmmoTextVisibility(true);
        TryAdaptAmmoText();
    }
    public void OnUnequip()
    {
        OnRelease();

        isShooting = false;

        TrySetAmmoTextVisibility(false);

        collider.enabled = true;

        transform.SetParent(null);
    }


    public void SetIsAiming(bool _isAiming)
    {
        Color colour =
            (_isAiming)
                ? Color.green
                : Color.red;
        string textColour = "<color=" + colour.ToString() + ">";

        Debug.Log(textColour + "isAiming" + "</color>");
    }

    public void StartShooting(float _shotReach, Transform _shooterTransform)
    {
        if (isReloading)
        {
            return;
        }

        shooterTransform = _shooterTransform;

        shotReach = _shotReach;
        shootTimer = shootCooldown;
        isShooting = true;
    }
    public void StopShooting()
    {
        isShooting = false;
    }
}
