using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FireSoundManager : MonoBehaviour
{
    [Header("Fire Audio")]
    [SerializeField] private AudioSource fireAudioSource;

    private int activeFireCount = 0;

    private void Awake()
    {
        if (fireAudioSource == null)
        {
            fireAudioSource = GetComponent<AudioSource>();
        }

        if (fireAudioSource != null)
        {
            fireAudioSource.loop = true;
            fireAudioSource.playOnAwake = false;
        }
    }

    public void RegisterFire()
    {
        activeFireCount++;

        // Start the fire sound when at least one fire is burning
        if (fireAudioSource != null && !fireAudioSource.isPlaying)
        {
            fireAudioSource.Play();
        }
    }

    public void NotifyFireExtinguished()
    {
        activeFireCount = Mathf.Max(0, activeFireCount - 1);

        Debug.Log("[Fire Sound] Remaining fires: " + activeFireCount);

        // Stop only after every fire has been extinguished
        if (activeFireCount == 0 && fireAudioSource != null)
        {
            fireAudioSource.Stop();
            Debug.Log("[Fire Sound] All fires extinguished. Audio stopped.");
        }
    }
}