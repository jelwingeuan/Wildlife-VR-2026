using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FireSoundManager : MonoBehaviour
{
    [Header("Fire Audio")]
    [SerializeField] private AudioSource fireAudioSource;

    private int remainingFires;

    // Prevents the same fire from being counted twice
    private readonly HashSet<int> extinguishedFireIDs = new();

    private void Awake()
    {
        if (fireAudioSource == null)
        {
            fireAudioSource = GetComponent<AudioSource>();
        }

        fireAudioSource.loop = true;
        fireAudioSource.playOnAwake = false;
    }

    private void Start()
    {
        // Automatically count all active FireTargets under the Fires parent
        FireTarget[] fires = GetComponentsInChildren<FireTarget>(false);
        remainingFires = fires.Length;

        Debug.Log("[FireSoundManager] Active fires found: " + remainingFires);

        if (remainingFires > 0 &&
            fireAudioSource != null &&
            fireAudioSource.clip != null)
        {
            fireAudioSource.Play();
        }
    }

    public void NotifyFireExtinguished(FireTarget fire)
    {
        if (fire == null)
            return;

        int fireID = fire.GetInstanceID();

        // Stop one fire from reporting several times
        if (!extinguishedFireIDs.Add(fireID))
            return;

        remainingFires = Mathf.Max(0, remainingFires - 1);

        Debug.Log(
            "[FireSoundManager] Fire extinguished. Remaining: " +
            remainingFires
        );

        if (remainingFires == 0)
        {
            if (fireAudioSource != null)
            {
                fireAudioSource.Stop();
            }

            Debug.Log(
                "[FireSoundManager] All fires extinguished. Sound stopped."
            );
        }
    }
}