using System.Collections;
using UnityEngine;

public class AudioClipHandler : MonoBehaviour
{
    private AudioSource _source;

    private AudioSource Source
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
    
    private Coroutine _handlingLifetimeCoroutine;

    public static AudioClipHandler PlayAudio(string clipResourcePath, float spatialBlend = 0, 
                                             NullableVector3 position = null, bool loop = false, 
                                             float volume = 1, bool destroyOnLoad = true)
    {
        AudioClip clipResource = Resources.Load<AudioClip>(clipResourcePath);
        return PlayAudio(clipResource, spatialBlend, position, loop, volume, destroyOnLoad);
    }
    
    public static AudioClipHandler PlayAudio(AudioClip clip, float spatialBlend = 0, 
                                             NullableVector3 position = null, bool loop = false, 
                                             float volume = 1, bool destroyOnLoad = true)
    {
        if (clip == null)
        {
            return null;
        }

        AudioClipHandler audioClipHandler = new GameObject(clip.name + "_clip").AddComponent<AudioClipHandler>();

        if (position != null)
        {
            audioClipHandler.transform.position = (Vector3) position;
        }

        audioClipHandler.StartClip(clip, spatialBlend, loop, volume, destroyOnLoad);

        return audioClipHandler;
    }

    private void StartClip(string clipResourcePath, float spatialBlend = 0, 
                           bool loop = false, float volume = 1, bool destroyOnLoad = true)
    {
        AudioClip clipResource = Resources.Load<AudioClip>(clipResourcePath);
        StartClip(clipResource, spatialBlend, loop, volume, destroyOnLoad);
    }

    private void StartClip(AudioClip clip, float spatialBlend = 0, 
                           bool loop = false, float volume = 1, bool destroyOnLoad = true)
    {
        if (clip == null)
        {
            return;
        }

        _source = gameObject.AddComponent<AudioSource>();
        Source.clip = clip;
        Source.spatialBlend = Mathf.Clamp01(spatialBlend);
        Source.loop = loop;
        Source.volume = Mathf.Clamp01(volume);
        Source.Play();
        _handlingLifetimeCoroutine = StartCoroutine(HandleLifetime());
        
        if (!destroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void StopClip()
    {
        if (Source.isPlaying)
        {
            _source.Stop();
        }
    }

    private IEnumerator HandleLifetime()
    {
        yield return new WaitUntil(() => !Source.isPlaying);

        Destroy(gameObject);

        _handlingLifetimeCoroutine = null;
    }
}
