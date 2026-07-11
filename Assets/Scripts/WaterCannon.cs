using UnityEngine;
using UnityEngine.XR;

public class WaterCannon : MonoBehaviour
{
    [Header("Cannon Setup")]
    public ParticleSystem waterParticles;

    [Header("Audio Setup")]
    public AudioSource sprayAudio;

    [Header("Input Setup")]
    [Tooltip("Which hand is holding the cannon?")]
    public XRNode controllerNode = XRNode.RightHand;

    private bool isShooting;

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

        if (triggerPulled && !isShooting)
        {
            StartShooting();
        }
        else if (!triggerPulled && isShooting)
        {
            StopShooting();
        }
    }

    private void StartShooting()
    {
        isShooting = true;

        if (waterParticles != null)
        {
            waterParticles.Play();
        }

        if (sprayAudio != null && !sprayAudio.isPlaying)
        {
            sprayAudio.Play();
        }
    }

    private void StopShooting()
    {
        isShooting = false;

        if (waterParticles != null)
        {
            waterParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }

        if (sprayAudio != null)
        {
            sprayAudio.Stop();
        }
    }

    private void OnDisable()
    {
        isShooting = false;

        if (waterParticles != null)
        {
            waterParticles.Stop();
        }

        if (sprayAudio != null)
        {
            sprayAudio.Stop();
        }
    }
}