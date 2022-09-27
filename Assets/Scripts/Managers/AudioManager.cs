using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _source;

    public AudioSource Source
    {
        get
        {
            if (_source == null)
            {
                _source = gameObject.AddComponent<AudioSource>();
            }

            return _source;
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

        if (!Source.isPlaying)
        {
            Source.Stop();
        }

        Source.clip = clip;
        Source.loop = true;
        Source.Play();
    }
}
