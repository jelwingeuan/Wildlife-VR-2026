using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(AudioSource))]
public class WaterCannon : MonoBehaviour
{
    [Header("Cannon Setup")]
    [SerializeField] private ParticleSystem waterParticles;

    [Header("Audio Setup")]
    [SerializeField] private AudioSource sprayAudio;

    [Header("Input Setup")]
    [Tooltip("Which hand is holding the cannon?")]
    [SerializeField] private XRNode controllerNode = XRNode.RightHand;

    private InputDevice controller;
    private bool isShooting;

    private void Awake()
    {
        // Automatically find the Audio Source on this object.
        if (sprayAudio == null)
        {
            sprayAudio = GetComponent<AudioSource>();
        }

        // Force the correct audio behaviour.
        if (sprayAudio != null)
        {
            sprayAudio.playOnAwake = false;
            sprayAudio.loop = true;
        }
    }

    private void OnEnable()
    {
        FindController();
    }

    private void Update()
    {
        // Reconnect if the controller was not detected yet.
        if (!controller.isValid)
        {
            FindController();
        }

        bool triggerPulled = false;

        if (controller.isValid)
        {
            controller.TryGetFeatureValue(
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

    private void FindController()
    {
        controller = InputDevices.GetDeviceAtXRNode(controllerNode);
    }

    private void StartShooting()
    {
        isShooting = true;

        if (waterParticles != null &&
            !waterParticles.isPlaying)
        {
            waterParticles.Play();
        }

        if (sprayAudio != null &&
            sprayAudio.resource != null &&
            !sprayAudio.isPlaying)
        {
            sprayAudio.Play();
        }
        else if (sprayAudio == null)
        {
            Debug.LogWarning(
                "Water Cannon has no Audio Source assigned."
            );
        }
        else if (sprayAudio.resource == null)
        {
            Debug.LogWarning(
                "Audio Source has no water spray sound assigned."
            );
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

        if (sprayAudio != null &&
            sprayAudio.isPlaying)
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