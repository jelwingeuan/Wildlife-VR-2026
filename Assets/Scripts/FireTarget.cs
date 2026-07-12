using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FireSFXManager : MonoBehaviour
{
    private AudioSource fireAudio;
    private FireTarget[] fireTargets;

    private void Start()
    {
        fireAudio = GetComponent<AudioSource>();

        // Find every FireTarget under the parent "Fires" object
        fireTargets = GetComponentsInChildren<FireTarget>(true);

        // Start the looping fire sound
        fireAudio.loop = true;
        fireAudio.playOnAwake = false;

        if (fireAudio.clip != null && fireTargets.Length > 0)
        {
            fireAudio.Play();
        }
    }

    private void Update()
    {
        bool anyFireStillBurning = false;

        foreach (FireTarget fire in fireTargets)
        {
            // Destroyed fires become null automatically
            if (fire == null)
                continue;

            if (fire.fireParticles != null &&
                fire.fireParticles.isEmitting)
            {
                anyFireStillBurning = true;
                break;
            }
        }

        // Stop the sound when every fire is extinguished
        if (!anyFireStillBurning)
        {
            fireAudio.Stop();

            // No need to keep checking after all fires are gone
            enabled = false;
        }
    }
}