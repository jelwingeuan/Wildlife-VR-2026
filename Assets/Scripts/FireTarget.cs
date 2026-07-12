using UnityEngine;

public class FireTarget : MonoBehaviour
{
    [Header("Fire Setup")]
    [Tooltip("Drag the fire's particle system here")]
    public ParticleSystem fireParticles;

    [Header("Fire Sound Manager")]
    [SerializeField] private FireSoundManager fireSoundManager;

    private bool hasBeenExtinguished = false;

    private void Start()
    {
        // Automatically find the manager on the Fires parent
        if (fireSoundManager == null)
        {
            fireSoundManager = GetComponentInParent<FireSoundManager>();
        }

        // Tell the manager that this fire is currently active
        if (fireSoundManager != null)
        {
            fireSoundManager.RegisterFire();
        }
        else
        {
            Debug.LogWarning(
                "[FireTarget] No FireSoundManager found for " + gameObject.name
            );
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.name.Contains("Water Stream"))
        {
            Extinguish();
        }
    }

    public void Extinguish()
    {
        // Prevent water particles from triggering this repeatedly
        if (hasBeenExtinguished)
            return;

        hasBeenExtinguished = true;

        // Stop producing new fire particles
        if (fireParticles != null)
        {
            fireParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }

        // Stop receiving water collision messages
        Collider col = GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = false;
        }

        // Reduce the active fire count
        if (fireSoundManager != null)
        {
            fireSoundManager.NotifyFireExtinguished();
        }

        // Allow the remaining particles or smoke to fade
        Destroy(gameObject, 2f);
    }
}