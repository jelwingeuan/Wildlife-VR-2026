using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomAnimalVoice : MonoBehaviour
{
    [Header("Animal Voice Clips")]
    [Tooltip("Add several different animal sounds here.")]
    public AudioClip[] animalVoiceClips;

    [Header("Random Timing")]
    [Tooltip("Minimum number of seconds before the animal makes a sound.")]
    public float minimumDelay = 8f;

    [Tooltip("Maximum number of seconds before the animal makes a sound.")]
    public float maximumDelay = 20f;

    [Header("Voice Settings")]
    [Range(0f, 1f)]
    public float volume = 0.8f;

    [Tooltip("Adds a small pitch variation so repeated sounds feel more natural.")]
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    private AudioSource audioSource;
    private Coroutine voiceRoutine;
    private int previousClipIndex = -1;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void OnEnable()
    {
        voiceRoutine = StartCoroutine(PlayRandomVoices());
    }

    private void OnDisable()
    {
        if (voiceRoutine != null)
        {
            StopCoroutine(voiceRoutine);
            voiceRoutine = null;
        }
    }

    private IEnumerator PlayRandomVoices()
    {
        while (true)
        {
            // Wait for a random amount of time.
            float delay = Random.Range(minimumDelay, maximumDelay);
            yield return new WaitForSeconds(delay);

            if (animalVoiceClips == null || animalVoiceClips.Length == 0)
                continue;

            int randomIndex = GetRandomClipIndex();
            previousClipIndex = randomIndex;

            audioSource.pitch = Random.Range(
                pitchRange.x,
                pitchRange.y
            );

            audioSource.PlayOneShot(
                animalVoiceClips[randomIndex],
                volume
            );

            // Wait until the current animal call finishes.
            yield return new WaitWhile(() => audioSource.isPlaying);
        }
    }

    private int GetRandomClipIndex()
    {
        if (animalVoiceClips.Length == 1)
            return 0;

        int selectedIndex;

        do
        {
            selectedIndex = Random.Range(0, animalVoiceClips.Length);
        }
        while (selectedIndex == previousClipIndex);

        return selectedIndex;
    }
}