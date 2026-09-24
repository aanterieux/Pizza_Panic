using UnityEngine;

public class AudioManager : MySingleton<AudioManager>
{
    public struct AudioParams
    {
        public static readonly AudioParams s_Default = new();
        public static readonly AudioParams s_Music = new(1f, 1f, true);
        public static readonly AudioParams s_Sound = new(1f, 1f, false);

        public float Volume;
        public float Pitch;
        public bool IsLooping;

        public AudioParams(float _volume = 1f, float _pitch = 1f, bool _isLooping = false)
        {
            Volume = _volume;
            Pitch = _pitch;
            IsLooping = _isLooping;
        }
    }

    [SerializeField] private bool m_dontDestroyOnLoad = true;
    [SerializeField] [Range(1, 64)]
     private int m_poolSize = 32;

    private AudioSource[] m_sources = null;
    private int m_nextSourceIndex = 0;

    private void Awake()
    {
        if (!InitialiseSingleton())
        {
            return;
        }

        if (m_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        m_sources = new AudioSource[m_poolSize];

        GameObject poolObj;
        AudioSource source;
        for (int i = 0; i < m_poolSize; ++i)
        {
            poolObj = new GameObject($"AudioSource{i}");
            poolObj.transform.SetParent(transform);

            source = poolObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.pitch = 1f;
            source.volume = 1f;
            source.spatialBlend = 1f;
            source.dopplerLevel = 0f;
            source.maxDistance = 5f;
            source.loop = false;

            m_sources[i] = source;
        }
    }

    private AudioSource GetFreeSource()
    {
        int index;

        for (int i = 0; i < m_poolSize; ++i)
        {
            index = (m_nextSourceIndex + i) % m_poolSize;

            if (!m_sources[index].isPlaying)
            {
                m_nextSourceIndex = (index + 1) % m_poolSize;

                return m_sources[index];
            }
        }

        return null;
    }

    private AudioSource Play(AudioClip _clip, Vector3 _position, AudioParams _params, bool _playSpatialised)
    {
        string dimension =
            (_playSpatialised)
                ? "3D"
                : "2D";

        if (!_clip)
        {
            LogUtils.LogWarning($"Cannot play {dimension} sound: _clip is null");
            return null;
        }

        AudioSource source = GetFreeSource();

        if (!source)
        {
            return null;
        }

        source.spatialBlend = (_playSpatialised) ? 1f : 0f;
        source.pitch = Mathf.Clamp(_params.Pitch, 0.01f, 10f);
        source.loop = _params.IsLooping;

        if (_playSpatialised)
        {
            source.transform.position = _position;
        }

        float volume = Mathf.Clamp01(_params.Volume);

        if (source.loop)
        {
            source.clip = _clip;
            source.volume = volume;
            source.Play();

            return source;
        }

        source.PlayOneShot(_clip, volume);

        return source;
    }


    public AudioSource Play2D(AudioClip _clip)
    {
        return Play(_clip, Vector3.zero, AudioParams.s_Default, false);
    }
    public AudioSource Play2D(AudioClip _clip, AudioParams _params)
    {
        return Play(_clip, Vector3.zero, _params, false);
    }

    public AudioSource Play3D(AudioClip _clip, Vector3 _position)
    {
        return Play(_clip, _position, AudioParams.s_Default, true);
    }
    public AudioSource Play3D(AudioClip _clip, Vector3 _position, AudioParams _params)
    {
        return Play(_clip, _position, _params, true);
    }
}
