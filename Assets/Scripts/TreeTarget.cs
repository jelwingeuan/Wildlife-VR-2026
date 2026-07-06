using System.Collections;
using UnityEngine;

public class TreeTarget : MonoBehaviour
{
    [Header("Tree Setup")]
    [Tooltip("Drag the child Tree model here")]
    public GameObject treeModel;

    [Header("Animation Settings")]
    [Tooltip("How small the tree is when first planted (e.g., 0.2 is 20% size)")]
    public float saplingScale = 0.2f;
    public float fadeDuration = 2f;
    public float growDuration = 3f;

    private Vector3 finalSize;
    private bool isPlanted = false;
    private Renderer[] treeRenderers;

    void Start()
    {
        if (treeModel != null)
        {
            // Hunt down and destroy any Animators that are forcing the scale to 100%
            Animator[] sneakyAnimators = treeModel.GetComponentsInChildren<Animator>();
            foreach (Animator anim in sneakyAnimators)
            {
                Destroy(anim);
            }

            // Save the full size you set in the Inspector
            finalSize = treeModel.transform.localScale;

            // Get all materials to control the fade effect
            treeRenderers = treeModel.GetComponentsInChildren<Renderer>();

            // Set to completely invisible and shrink to sapling size
            SetTreeAlpha(0f);
            treeModel.transform.localScale = finalSize * saplingScale;
            treeModel.SetActive(false);
        }
    }

    void OnParticleCollision(GameObject other)
    {
        // 🚨 DEBUG TOOL: This will print EXACTLY what is touching the green box!
        Debug.Log("💥 SOMETHING HIT THE PLANTING ZONE: " + other.name);

        // Trigger the entire sequence ONLY when hit by the Seed Launcher
        if (other.name.Contains("Seed Stream") && !isPlanted)
        {
            isPlanted = true; // Lock it so it only happens once
            StartCoroutine(PlantAndGrowSequence());
        }
    }

    IEnumerator PlantAndGrowSequence()
    {
        // --- PHASE 1: TURN ON & FADE IN ---
        treeModel.SetActive(true);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / fadeDuration;

            // Force the scale to stay small EVERY SINGLE FRAME of the fade
            treeModel.transform.localScale = finalSize * saplingScale;

            // Smoothly increase opacity from 0 to 1
            SetTreeAlpha(percent);
            yield return null;
        }

        SetTreeAlpha(1f); // Make sure it is 100% solid

        // --- PHASE 2: GROW TO FULL SIZE ---
        timer = 0f;
        Vector3 startScale = treeModel.transform.localScale;

        while (timer < growDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / growDuration;

            // Smoothly scale from the tiny sapling size to the huge final size
            treeModel.transform.localScale = Vector3.Lerp(startScale, finalSize, percent);
            yield return null;
        }

        // Snap exactly to final size at the very end
        treeModel.transform.localScale = finalSize;
    }

    // Helper function to change the opacity of the materials safely
    private void SetTreeAlpha(float alpha)
    {
        if (treeRenderers == null) return;

        foreach (Renderer r in treeRenderers)
        {
            foreach (Material mat in r.materials)
            {
                // 1. If it is a standard URP material, safely fade _BaseColor
                if (mat.shader.name.Contains("Lit") && mat.HasProperty("_BaseColor"))
                {
                    Color c = mat.GetColor("_BaseColor");
                    c.a = alpha;
                    mat.SetColor("_BaseColor", c);
                }
                // 2. If it is a custom material, try the standard _Color instead
                else if (mat.HasProperty("_Color"))
                {
                    Color c = mat.GetColor("_Color");
                    c.a = alpha;
                    mat.SetColor("_Color", c);
                }
            }
        }
    }
}