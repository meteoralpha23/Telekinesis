using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXsource;

    [Header("Audio Clips")]
    public AudioClip background;
    public AudioClip teleknesisStartSound;
    public AudioClip throwSound;
    public AudioClip holdingSound; // New AudioClip for holding sound

    private void Start()
    {
        if (musicSource != null && background != null)
        {
            musicSource.clip = background;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("MusicSource or Background clip is not assigned.");
        }
    }

    public void PlayTeleknesisStart()
    {
        SFXsource.clip = teleknesisStartSound;
        SFXsource.Play();
    }

    public void PlayThrowSound()
    {
        SFXsource.clip = throwSound;
        SFXsource.Play();
    }

    // Play holding sound
    public void PlayHoldingSound()
    {
        if (holdingSound != null)
        {
            SFXsource.clip = holdingSound;
            SFXsource.loop = true; // Loop the holding sound
            SFXsource.Play();
        }
    }

    // Stop holding sound
    public void StopHoldingSound()
    {
        SFXsource.loop = false; // Stop looping
        SFXsource.Stop();
    }
}