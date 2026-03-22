using UnityEngine;

namespace DressUp.Core
{
public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource sfxSource;

    [SerializeField]
    private AudioClip creamApply;
    [SerializeField]
    private AudioClip brushPickup;
    [SerializeField]
    private AudioClip brushApply;
    [SerializeField]
    private AudioClip lipstickApply;
    [SerializeField]
    private AudioClip blushApply;
    [SerializeField]
    private AudioClip spongeReset;

    private void Play(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    private void OnEnable()
    {
        AudioEvents.OnCreamApplied += PlayCream;
        AudioEvents.OnBrushPickup += PlayBrushPickup;
        AudioEvents.OnBrushApplied += PlayBrushApply;
        AudioEvents.OnLipstickApplied += PlayLipstick;
        AudioEvents.OnBlushApplied += PlayBlush;
        AudioEvents.OnSpongeReset += PlaySponge;
    }

    private void OnDisable()
    {
        AudioEvents.OnCreamApplied -= PlayCream;
        AudioEvents.OnBrushPickup -= PlayBrushPickup;
        AudioEvents.OnBrushApplied -= PlayBrushApply;
        AudioEvents.OnLipstickApplied -= PlayLipstick;
        AudioEvents.OnBlushApplied -= PlayBlush;
        AudioEvents.OnSpongeReset -= PlaySponge;
    }

    private void PlayCream() => Play(creamApply);
    private void PlayBrushPickup() => Play(brushPickup);
    private void PlayBrushApply() => Play(brushApply);
    private void PlayLipstick() => Play(lipstickApply);
    private void PlayBlush() => Play(blushApply);
    private void PlaySponge() => Play(spongeReset);
}
}
