using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(AudioSource))]
public class SeedLauncher : MonoBehaviour
{
    [Header("Launcher Setup")]
    public ParticleSystem seedParticles;

    [Header("Audio Setup")]
    [Tooltip("Sound that loops while the trigger is held")]
    public AudioSource launcherAudio;

    [Header("Input Setup")]
    public XRNode controllerNode = XRNode.RightHand;

    private bool isShooting = false;

    private void Awake()
    {
        // Automatically find the Audio Source on this object.
        if (launcherAudio == null)
        {
            launcherAudio = GetComponent<AudioSource>();
        }

        if (launcherAudio != null)
        {
            launcherAudio.playOnAwake = false;
            launcherAudio.loop = true;
        }
    }

    private void Update()
    {
        InputDevice device =
            InputDevices.GetDeviceAtXRNode(controllerNode);

        bool triggerPulled = false;

        if (device.isValid)
        {
            device.TryGetFeatureValue(
                CommonUsages.triggerButton,
                out triggerPulled
            );
        }

        // Trigger pressed.
        if (triggerPulled && !isShooting)
        {
            StartShooting();
        }
        // Trigger released.
        else if (!triggerPulled && isShooting)
        {
            StopShooting();
        }
    }

    private void StartShooting()
    {
        isShooting = true;

        if (seedParticles != null &&
            !seedParticles.isPlaying)
        {
            seedParticles.Play();
        }

        if (launcherAudio != null &&
            !launcherAudio.isPlaying)
        {
            launcherAudio.Play();
        }
    }

    private void StopShooting()
    {
        isShooting = false;

        if (seedParticles != null)
        {
            seedParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }

        if (launcherAudio != null &&
            launcherAudio.isPlaying)
        {
            launcherAudio.Stop();
        }
    }

    private void OnDisable()
    {
        isShooting = false;

        if (seedParticles != null)
        {
            seedParticles.Stop();
        }

        if (launcherAudio != null)
        {
            launcherAudio.Stop();
        }
    }
}