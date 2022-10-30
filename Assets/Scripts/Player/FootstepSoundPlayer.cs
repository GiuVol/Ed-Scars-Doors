using UnityEngine;

public class FootstepSoundPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip _footstepClip;

    [SerializeField]
    private float _volume;

    public void PlayStepSound()
    {
        AudioClipHandler.PlayAudio(_footstepClip, 1, transform.position, false, _volume);
    }
}
