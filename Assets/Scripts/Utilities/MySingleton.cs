using UnityEngine;

public abstract class MySingleton<T> : MonoBehaviour where T : MySingleton<T>
{
    public static T s_Instance
    {
        get;
        private set;
    }

    protected bool InitialiseSingleton()
    {
        if (s_Instance != null &&
            s_Instance != this)
        {
            Destroy(gameObject);
            return false;
        }

        s_Instance = (T)(this);
        return true;
    }

    protected virtual void OnDestroy()
    {
        if (s_Instance == this)
        {
            s_Instance = null;
        }
    }
}
