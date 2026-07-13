using UnityEngine;

public class FireTarget : MonoBehaviour
{
    [Header("Fire Setup")]
    [Tooltip("Drag the Fire's particle system here")]
    public ParticleSystem fireParticles;
    
    [Tooltip("Drag the AudioSource containing the fire crackle here")]
    public AudioSource fireAudio;

    // Unity automatically runs this function when a particle with 
    // "Send Collision Messages" enabled hits this object's collider.
    void OnParticleCollision(GameObject other)
    {
        // Check if the object hitting us is the water stream
        if (other.name.Contains("Water Stream"))
        {
            Extinguish();
        }
    }

    public void Extinguish()
    {
        // 1. Stop the fire particles from emitting
        if (fireParticles != null)
        {
            fireParticles.Stop();
        }
        
        // 2. Stop the fire sound effect immediately
        if (fireAudio != null)
        {
            fireAudio.Stop();
        }
        
        // 3. Turn off the collider so we don't keep registering hits
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        // 4. Optional: Destroy the fire game object entirely after 2 seconds 
        // to give the remaining smoke/particles time to naturally fade away.
        Destroy(gameObject, 2f);
    }
}