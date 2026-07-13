using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomAnimalVoice : MonoBehaviour
{
    [Header("Animal Voice Clips")]
    public AudioClip[] animalVoiceClips;

    [Header("Random Delay")]
    public float minimumDelay = 8f;
    public float maximumDelay = 20f;

    [Header("Sound Settings")]
    [Range(0f, 1f)]
    public float volume = 0.7f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void OnEnable()
    {
        StartCoroutine(RandomVoiceRoutine());
    }

    private IEnumerator RandomVoiceRoutine()
    {
        while (true)
        {
            float delay = Random.Range(minimumDelay, maximumDelay);
            yield return new WaitForSeconds(delay);

            if (animalVoiceClips != null && animalVoiceClips.Length > 0)
            {
                AudioClip randomClip =
                    animalVoiceClips[Random.Range(0, animalVoiceClips.Length)];

                audioSource.PlayOneShot(randomClip, volume);
            }
        }
    }
}