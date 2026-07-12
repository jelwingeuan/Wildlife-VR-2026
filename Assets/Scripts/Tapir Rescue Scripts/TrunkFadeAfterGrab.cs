using System.Collections;
using UnityEngine;

public class TrunkFadeAfterGrab : MonoBehaviour
{
    [Header("Mission")]
    public TapirMissionV2 mission;

    [Header("Fade Settings")]
    public float fadeDuration = 5f;
    public bool disappearWhenCleared = true;

    [Header("Lift Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip liftSFX;

    [Range(0f, 1f)]
    [SerializeField] private float liftVolume = 1f;

    private bool isClearing = false;
    private Renderer[] renderers;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        // Automatically find the Audio Source if it was not assigned.
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void StartFadeAndClear()
    {
        if (isClearing)
            return;

        isClearing = true;

        // Play the trunk-lifting sound once.
        if (audioSource != null && liftSFX != null)
        {
            audioSource.PlayOneShot(liftSFX, liftVolume);
        }
        else
        {
            Debug.LogWarning("[Trunk] Audio Source or Lift SFX is not assigned.");
        }

        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        Debug.Log("[Trunk] Grabbed. Fading for " 
                  + fadeDuration + " seconds.");

        float timer = 0f;

        Material[][] allMaterials =
            new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            allMaterials[i] = renderers[i].materials;

            foreach (Material mat in allMaterials[i])
            {
                MakeMaterialTransparent(mat);
            }
        }

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float alpha =
                Mathf.Lerp(1f, 0f, timer / fadeDuration);

            foreach (Material[] mats in allMaterials)
            {
                foreach (Material mat in mats)
                {
                    if (mat.HasProperty("_BaseColor"))
                    {
                        Color c = mat.GetColor("_BaseColor");
                        c.a = alpha;
                        mat.SetColor("_BaseColor", c);
                    }
                    else if (mat.HasProperty("_Color"))
                    {
                        Color c = mat.color;
                        c.a = alpha;
                        mat.color = c;
                    }
                }
            }

            yield return null;
        }

        if (mission != null)
        {
            mission.OnTrunkCleared(gameObject);
        }
        else
        {
            Debug.LogWarning(
                "[Trunk] Mission is not assigned.");
        }

        if (disappearWhenCleared)
            gameObject.SetActive(false);
    }

    private void MakeMaterialTransparent(Material mat)
    {
        if (mat == null)
            return;

        if (mat.HasProperty("_Surface"))
            mat.SetFloat("_Surface", 1f);

        if (mat.HasProperty("_Blend"))
            mat.SetFloat("_Blend", 0f);

        if (mat.HasProperty("_AlphaClip"))
            mat.SetFloat("_AlphaClip", 0f);

        mat.renderQueue = 3000;
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.EnableKeyword("_ALPHABLEND_ON");
    }
}