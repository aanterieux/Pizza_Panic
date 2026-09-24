using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitialiser : MonoBehaviour
{
    private static GameInitialiser s_instance = null;

    [SerializeField] private bool m_dontDestroyOnLoad = true;
    
    [Space]

    [Header("- Managers prefabs -")]
    [SerializeField] private GameObject m_GameManagerPrefab = null;
    [SerializeField] private GameObject m_InputManagerPrefab = null;
    [SerializeField] private GameObject m_AudioManagerPrefab = null;
    [SerializeField] private GameObject m_ZombieManagerPrefab = null;

    [Header("- Misc -")]
    [SerializeField] private AudioClip m_menuMusic = null;

    private AudioSource m_musicPlayer = null;

    private void Awake()
    {
        if (s_instance != null &&
            s_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;

        if (m_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        TryInstantiatePrefab(m_GameManagerPrefab);
        TryInstantiatePrefab(m_InputManagerPrefab);
        TryInstantiatePrefab(m_AudioManagerPrefab);
        TryInstantiatePrefab(m_ZombieManagerPrefab);

        SceneManager.sceneLoaded += StopMusicOnGameStart;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            return;
        }

        m_musicPlayer = AudioManager.s_Instance.Play2D(m_menuMusic, AudioManager.AudioParams.s_Music);
    }

    private void OnDestroy()
    {
        if (s_instance == this)
        {
            s_instance = null;
        }
    }


    private void TryInstantiatePrefab(GameObject _prefab)
    {
        if (!_prefab)
        {
            return;
        }

        Instantiate(_prefab, transform);
    }

    private void StopMusicOnGameStart(Scene _scene, LoadSceneMode _loadMode)
    {
        if (m_musicPlayer == null ||
            SceneManager.GetActiveScene().name == "MainMenu")
        {
            return;
        }

        m_musicPlayer.Stop();
        m_musicPlayer = null;
        SceneManager.sceneLoaded -= StopMusicOnGameStart;
    }
}
