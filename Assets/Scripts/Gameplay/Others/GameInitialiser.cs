using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitialiser : MonoBehaviour
{
    private static GameInitialiser s_instance = null;

    [SerializeField] private bool m_dontDestroyOnLoad = true;
    
    [Space]

    [Header("- Managers prefabs -")]
    [SerializeField] private GameObject m_gameManagerPrefab = null;
    [SerializeField] private GameObject m_inputManagerPrefab = null;
    [SerializeField] private GameObject m_audioManagerPrefab = null;
    [SerializeField] private GameObject m_menuControllerPrefab = null;
    [SerializeField] private GameObject m_zombieManagerPrefab = null;

    [Header("- Misc -")]
    [SerializeField] private AudioClip m_menuMusic = null;

    private AudioSource m_musicPlayer = null;
    private Coroutine m_playMusicMenuCoroutine = null;
    private bool m_isMusicPlaying = false;

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

        TryInstantiatePrefab(m_gameManagerPrefab);
        TryInstantiatePrefab(m_inputManagerPrefab);
        TryInstantiatePrefab(m_audioManagerPrefab);
        TryInstantiatePrefab(m_menuControllerPrefab);
        TryInstantiatePrefab(m_zombieManagerPrefab);

        SceneManager.sceneLoaded += PlayMusicInMenu;
        SceneManager.sceneUnloaded += StopMusicOnGameStart;
    }

    private void Start()
    {
        Scene activeScene = SceneManager.GetActiveScene();

        if (activeScene.name == "MainMenu")
        {
            PlayMusicInMenu(activeScene, LoadSceneMode.Single);
        }
    }


    private void OnDestroy()
    {
        StopAllCoroutines();

        SceneManager.sceneUnloaded -= StopMusicOnGameStart;
        SceneManager.sceneLoaded -= PlayMusicInMenu;

        if (s_instance == this)
        {
            s_instance = null;
        }
    }


    private void TryInstantiatePrefab(GameObject _prefab)
    {
        if (!_prefab)
        {
            LogUtils.LogError($"Cannot instantiate _prefab: {_prefab} is null");
            return;
        }

        Instantiate(_prefab, transform);
    }

    private void PlayMusicInMenu(Scene _scene, LoadSceneMode _loadMode)
    {
        if (m_isMusicPlaying ||
            m_playMusicMenuCoroutine != null ||
            _scene.name != "MainMenu")
        {
            return;
        }

        m_playMusicMenuCoroutine = StartCoroutine(PlayMusicInMenuCoroutine());
    }
    private void StopMusicOnGameStart(Scene _scene)
    {
        if (_scene.name != "MainMenu")
        {
            return;
        }

        if (m_musicPlayer != null)
        {
            m_musicPlayer.Stop();
            m_musicPlayer = null;
        }

        if (m_playMusicMenuCoroutine != null)
        {
            StopCoroutine(m_playMusicMenuCoroutine);
            m_playMusicMenuCoroutine = null;
        }

        m_isMusicPlaying = false;
    }


    private IEnumerator PlayMusicInMenuCoroutine()
    {
        int remainingAttempts = 100;
        WaitForSecondsRealtime delay = new(0.033f);

        while (!AudioManager.s_Instance && remainingAttempts > 0)
        {
            remainingAttempts--;
            yield return delay;
        }

        m_musicPlayer = AudioManager.s_Instance.Play2D(m_menuMusic, AudioManager.AudioParams.s_Music);
        m_isMusicPlaying = (m_musicPlayer != null);
        m_playMusicMenuCoroutine = null;
    }
}
