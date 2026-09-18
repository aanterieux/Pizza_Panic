using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{


    [SerializeField] private bool m_dontDestroyOnLoad = true;

    private void Awake()
    {
        if (m_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }


    public static void PlayResource(AudioSource _source, AudioResource _resource = null)
    {
        if (_resource != null)
        {
            _source.resource = _resource;
        }

        _source.Play();
    }
}
