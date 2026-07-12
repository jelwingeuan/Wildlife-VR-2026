using UnityEngine;

public class FireTarget : MonoBehaviour
{
    [Header("Fire Setup")]
    [Tooltip("Optional. The script also finds all child particle systems automatically.")]
    [SerializeField] private ParticleSystem fireParticles;

    [Header("Water Detection")]
    [Tooltip("The water particle system must use this tag.")]
    [SerializeField] private string waterTag = "WaterStream";

    [Header("Fire Sound")]
    [SerializeField] private FireSoundManager fireSoundManager;

    [Header("Extinguish Settings")]
    [SerializeField] private float destroyDelay = 2f;

    private bool hasBeenExtinguished;

    private ParticleSystem[] allFireParticles;
    private Collider[] allColliders;

    public bool IsExtinguished => hasBeenExtinguished;

    private void Awake()
    {
        // Find the sound manager from the Fires parent
        if (fireSoundManager == null)
        {
            fireSoundManager = GetComponentInParent<FireSoundManager>();
        }

        // Find every particle system belonging to this fire
        allFireParticles =
            GetComponentsInChildren<ParticleSystem>(true);

        // Find every collider belonging to this fire
        allColliders =
            GetComponentsInChildren<Collider>(true);

        if (fireParticles == null && allFireParticles.Length > 0)
        {
            fireParticles = allFireParticles[0];
        }
    }

    private void Start()
    {
        if (fireSoundManager == null)
        {
            Debug.LogError(
                "[FireTarget] No FireSoundManager found for: " +
                gameObject.name
            );
        }

        if (allColliders.Length == 0)
        {
            Debug.LogError(
                "[FireTarget] No collider found for: " +
                gameObject.name
            );
        }

        if (allFireParticles.Length == 0)
        {
            Debug.LogError(
                "[FireTarget] No particle systems found for: " +
                gameObject.name
            );
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (hasBeenExtinguished || other == null)
            return;

        bool isWater =
            other.CompareTag(waterTag) ||
            other.transform.root.CompareTag(waterTag);

        if (isWater)
        {
            Debug.Log(
                "[FireTarget] Water hit: " + gameObject.name
            );

            Extinguish();
        }
    }

    public void Extinguish()
    {
        if (hasBeenExtinguished)
            return;

        hasBeenExtinguished = true;

        Debug.Log(
            "[FireTarget] Extinguishing: " + gameObject.name
        );

        // Stop all flames, smoke and related effects
        foreach (ParticleSystem particles in allFireParticles)
        {
            if (particles != null)
            {
                particles.Stop(
                    true,
                    ParticleSystemStopBehavior.StopEmitting
                );
            }
        }

        // Disable every collider belonging to this fire
        foreach (Collider col in allColliders)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }

        if (fireSoundManager != null)
        {
            fireSoundManager.NotifyFireExtinguished(this);
        }

        Destroy(gameObject, destroyDelay);
    }
}