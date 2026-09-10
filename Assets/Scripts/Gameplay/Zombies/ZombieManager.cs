using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    [SerializeField] [Range(0, 1000)]
     private int m_maxZombieCount = 100;
    [SerializeField] private bool m_dontDestroyOnLoad = true;

    private int m_zombieCount = 0;

    public int m_ZombieCount
    {
        get => m_zombieCount;
    }
    public bool m_IsSpawnAllowed
    {
        get => (m_zombieCount < m_maxZombieCount);
    }

    private void Awake()
    {
        if (m_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }


    public void IncrementZombieCount()
    {
        m_zombieCount++;
    }
    public void DecrementZombieCount()
    {
        if (m_zombieCount == 0)
        {
            return;
        }

        m_zombieCount--;
    }
}
