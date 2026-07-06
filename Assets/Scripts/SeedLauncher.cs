using UnityEngine;
using UnityEngine.XR;

public class SeedLauncher : MonoBehaviour
{
    [Header("Launcher Setup")]
    public ParticleSystem seedParticles;

    [Header("Input Setup")]
    public XRNode controllerNode = XRNode.RightHand;

    private bool isShooting = false;

    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
        bool triggerPulled = false;

        if (device.isValid)
        {
            device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPulled);
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
        if (seedParticles != null) seedParticles.Play();
    }

    private void StopShooting()
    {
        isShooting = false;
        if (seedParticles != null) seedParticles.Stop();
    }
}