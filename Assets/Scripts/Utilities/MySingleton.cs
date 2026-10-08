using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class MySingleton<T> : MonoBehaviour where T : MySingleton<T>
{
    public static T s_Instance
    {
        get;
        private set;
    }

    private string m_name = "";
    private bool m_isInitialised = false;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (!InitialiseSingleton())
        {
            if (s_Instance == (T)(this))
            {
                LogUtils.LogError($"Cannot initialise {m_name}: singleton already exists");
            }

            return;
        }

        m_isInitialised = true;
        OnSingletonAwake();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (!m_isInitialised)
        {
            return;
        }

        OnSingletonDestroy();

        if (s_Instance == this)
        {
            s_Instance = null;
        }
    }


    protected virtual void OnSingletonAwake()
    {
    }

    protected virtual void OnSingletonDestroy()
    {
    }


    private void OnSceneLoaded(Scene _scene, LoadSceneMode _loadSceneMode)
    {
        InitialiseSingleton();
    }


    protected bool InitialiseSingleton()
    {
        if (s_Instance != null &&
            s_Instance != this)
        {
            m_name = gameObject.name;
            Destroy(gameObject);
            return false;
        }

        s_Instance = (T)(this);
        return true;
    }

}
