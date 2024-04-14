using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioClip[] audioClips;
    public float MasterVolume = 0.5f;

    public static AudioController instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public AudioSource PlaySoundWithoutEnd(int index, float volumeMultiplier = 1f, float pitchEffectMultiplier = 0.1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = audioClips[index];
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.volume = MasterVolume * volumeMultiplier;
        audioSource.pitch = UnityEngine.Random.Range(1.0f - pitchEffectMultiplier, 1.0f + pitchEffectMultiplier);

        audioSource.Play();
        return audioSource;
    }

    public void PlaySound(int index, float volumeMultiplier = 1f, float pitchEffectMultiplier = 0.1f)
    {
        AudioSource audioSource = PlaySoundWithoutEnd(index,volumeMultiplier,pitchEffectMultiplier);
        StartCoroutine(DestroySourceAfterPlayed(audioSource));
    }

    public void PlaySound(string name, float volumeMultiplier = 1f)
    {
        int index = System.Array.FindIndex(audioClips, clip => clip.name == name);
        PlaySound(index, volumeMultiplier);
    }

    IEnumerator DestroySourceAfterPlayed(AudioSource source)
    {
        yield return new WaitUntil(() => !source.isPlaying);
        Destroy(source);
    }
}
