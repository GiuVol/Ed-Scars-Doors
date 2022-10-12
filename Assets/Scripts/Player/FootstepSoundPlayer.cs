using UnityEngine;

public class FootstepSoundPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip _footstepClip;

    public void PlayStepSound()
    {
        AudioClipHandler.PlayAudio(_footstepClip, 1, transform.position);
    }
}
