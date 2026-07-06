using UnityEngine;
using UnityEngine.XR;

public class WaterCannon : MonoBehaviour
{
    [Header("Cannon Setup")]
    public ParticleSystem waterParticles;

    [Header("Input Setup")]
    [Tooltip("Which hand is holding the cannon?")]
    public XRNode controllerNode = XRNode.RightHand;

    private bool isShooting = false;

    void Update()
    {
        // 1. Get the controller directly from the hardware
        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);

        bool triggerPulled = false;

        // 2. Read the trigger button state
        if (device.isValid)
        {
            device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPulled);
        }

        // 3. Start shooting when the trigger is pulled
        if (triggerPulled && !isShooting)
        {
            StartShooting();
        }
        // 4. Stop shooting when the trigger is released
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
    }

    private void StopShooting()
    {
        isShooting = false;
        if (waterParticles != null)
        {
            waterParticles.Stop();
        }
    }
}