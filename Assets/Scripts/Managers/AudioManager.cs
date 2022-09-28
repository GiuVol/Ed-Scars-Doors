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
    
    public void PlayOst(string clipResourcePath)
    {
        AudioClip clipResource = Resources.Load<AudioClip>(clipResourcePath);
        PlayOst(clipResource);
    }

    public void PlayOst(AudioClip clip)
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

    public void PlayAmbience(string clipResourcePath)
    {
        AudioClip clipResource = Resources.Load<AudioClip>(clipResourcePath);
        PlayAmbience(clipResource);
    }
    
    public void PlayAmbience(AudioClip clip)
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
