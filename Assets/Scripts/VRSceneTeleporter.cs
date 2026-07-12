using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class VRSceneTeleporter : MonoBehaviour
{
    [Header("Scene Destination")]
    [Tooltip("Exact scene name as listed in Build Profiles.")]
    public string sceneToLoad;

    [Header("UI Prompt")]
    public GameObject promptCanvas;

    [Header("Teleport Audio")]
    [Tooltip("Audio Source attached to the Teleport Pad.")]
    public AudioSource teleportAudioSource;

    [Tooltip("Sound played before changing scene.")]
    public AudioClip teleportSFX;

    [Tooltip("How long to wait before changing scene. Set to 0 to use the full audio clip length.")]
    [Min(0f)]
    public float teleportDelay = 1f;

    private bool playerIsOnPad;
    private bool isTeleporting;

    // Prevents buttons from firing repeatedly while held.
    private bool wasXPressed;
    private bool wasYPressed;

    private void Start()
    {
        if (promptCanvas != null)
            promptCanvas.SetActive(false);

        // Automatically find the Audio Source on this object.
        if (teleportAudioSource == null)
            teleportAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!playerIsOnPad || isTeleporting)
            return;

        InputDevice leftHand =
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        // X button — confirm teleport.
        if (leftHand.TryGetFeatureValue(
                CommonUsages.primaryButton,
                out bool isXPressed))
        {
            if (isXPressed && !wasXPressed)
            {
                Debug.Log("X pushed. Starting teleport transition.");

                StartCoroutine(TeleportRoutine());
            }

            wasXPressed = isXPressed;
        }

        // Y button — cancel.
        if (leftHand.TryGetFeatureValue(
                CommonUsages.secondaryButton,
                out bool isYPressed))
        {
            if (isYPressed && !wasYPressed)
            {
                Debug.Log("Y pushed. Canceling teleport.");

                HidePrompt();
            }

            wasYPressed = isYPressed;
        }
    }

    private IEnumerator TeleportRoutine()
    {
        // Prevent multiple scene-load requests.
        isTeleporting = true;
        playerIsOnPad = false;

        if (promptCanvas != null)
            promptCanvas.SetActive(false);

        // Play teleport sound.
        if (teleportAudioSource != null && teleportSFX != null)
        {
            teleportAudioSource.PlayOneShot(teleportSFX);
        }
        else
        {
            Debug.LogWarning(
                "Teleport Audio Source or Teleport SFX is missing.");
        }

        // If Teleport Delay is 0, use the audio clip's full duration.
        float delay = teleportDelay;

        if (delay <= 0f && teleportSFX != null)
            delay = teleportSFX.length;

        // Small fallback delay when no audio is assigned.
        if (delay <= 0f)
            delay = 0.1f;

        // Realtime prevents Time.timeScale from interrupting it.
        yield return new WaitForSecondsRealtime(delay);

        if (string.IsNullOrWhiteSpace(sceneToLoad))
        {
            Debug.LogError(
                "No destination scene has been entered.");

            isTeleporting = false;
            yield break;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || isTeleporting)
            return;

        playerIsOnPad = true;

        if (promptCanvas != null)
            promptCanvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isTeleporting)
            HidePrompt();
    }

    private void HidePrompt()
    {
        playerIsOnPad = false;

        if (promptCanvas != null)
            promptCanvas.SetActive(false);
    }
}