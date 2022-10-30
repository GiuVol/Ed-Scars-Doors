using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _ostSource;

    public AudioSource OstSource
    {
        get
        {
            if (_ostSource == null)
            {
                _ostSource = gameObject.AddComponent<AudioSource>();
            }

            return _ostSource;
        }
    }

    private AudioSource _ambienceSource;

    public AudioSource AmbienceSource
    {
        get
        {
            if (_ambienceSource == null)
            {
                _ambienceSource = gameObject.AddComponent<AudioSource>();
            }

            return _ambienceSource;
        }
    }
    
    public void PlayOst(string clipResourcePath, float volume = 1)
    {
        AudioClip clipResource = Resources.Load<AudioClip>(clipResourcePath);
        PlayOst(clipResource, volume);
    }

    public void PlayOst(AudioClip clip, float volume = 1)
    {
        if (clip == null)
        {
            return;
        }

        StopOst();

        OstSource.clip = clip;
        OstSource.loop = true;
        OstSource.Play();
    }

    public void StopOst()
    {
        if (!OstSource.isPlaying)
        {
            OstSource.Stop();
        }
    }

    public void PlayAmbience(string clipResourcePath, float volume = 1)
    {
        AudioClip clipResource = Resources.Load<AudioClip>(clipResourcePath);
        PlayAmbience(clipResource, volume);
    }
    
    public void PlayAmbience(AudioClip clip, float volume = 1)
    {
        if (clip == null)
        {
            return;
        }

        StopAmbience();

        AmbienceSource.clip = clip;
        AmbienceSource.loop = true;
        AmbienceSource.Play();
    }

    public void StopAmbience()
    {
        if (!AmbienceSource.isPlaying)
        {
            AmbienceSource.Stop();
        }
    }
}
