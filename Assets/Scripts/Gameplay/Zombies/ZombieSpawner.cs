using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_zombiePrefab = null;
    [SerializeField] private int m_maxSpawnedZombies = 10;
    [SerializeField] private int m_zombiesPerSpawn = 1;
    [SerializeField] private float m_spawnWaitingTime = 3.5f;
    [SerializeField] private float m_spawnRadius = 3f;
    [SerializeField] private bool m_isActive = true;

    private ZombieSpawner m_self = null;
    private ZombieManager m_manager = null;
    private float m_spawnTimer = 0f;
    private int m_zombieSpawnedCount = 0;

    private void Awake()
    {
        m_self = GetComponent<ZombieSpawner>();
        m_spawnTimer += Random.Range(-1.5f, 0.5f);
    }

    private void Start()
    {
        m_manager = FindAnyObjectByType<ZombieManager>();

        if (!m_manager)
        {
            m_manager = Instantiate(new ZombieManager());
        }
    }

    private void Update()
    {
        if (!m_manager.m_IsSpawnAllowed)
        {
            return;
        }

        m_isActive = (m_zombieSpawnedCount < m_maxSpawnedZombies);

        if (!m_isActive)
        {
            return;
        }

        m_spawnTimer += Time.deltaTime;

        if (m_spawnTimer >= m_spawnWaitingTime)
        {
            SpawnZombie();

            m_spawnTimer = 0f;
        }
    }


    private void SpawnZombie()
    {
        Vector2 horizontalOffset;
        Vector3 selfPos = transform.position;
        Vector3 spawnPos;
        Zombie newZombie;

        for (int i = 0; i < m_zombiesPerSpawn; ++i)
        {
            horizontalOffset = m_spawnRadius * Random.insideUnitCircle;
            spawnPos =
                selfPos
                + new Vector3(
                    horizontalOffset.x,
                    transform.position.y,
                    horizontalOffset.y
                );

            newZombie =
                Instantiate(
                    m_zombiePrefab,
                    spawnPos,
                    Quaternion.identity,
                    transform
                ).GetComponent<Zombie>();
            newZombie.LinkToSpawner(m_self);

            m_zombieSpawnedCount++;
            m_manager.IncrementZombieCount();
        }
    }

    public void NotifyZombieDeath()
    {
        m_zombieSpawnedCount--;
        m_manager.DecrementZombieCount();
    }
}
