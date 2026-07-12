using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

[RequireComponent(typeof(AudioSource))]
public class VRSceneTeleporter : MonoBehaviour
{
    [Header("Scene Destination")]
    public string sceneToLoad;

    [Header("UI Prompt")]
    public GameObject promptCanvas;

    [Header("Teleport Audio")]
    [Tooltip("Audio Source attached to the TeleportPad")]
    public AudioSource teleportAudioSource;

    [Tooltip("Sound played when teleportation begins")]
    public AudioClip teleportSFX;

    [Tooltip("Time between playing the sound and loading the scene")]
    [Min(0f)]
    public float teleportDelay = 1.5f;

    [Range(0f, 1f)]
    public float teleportVolume = 1f;

    private bool playerIsOnPad = false;
    private bool isTeleporting = false;

    // Prevents buttons from triggering continuously while held.
    private bool wasXPressed = false;
    private bool wasYPressed = false;

    private void Start()
    {
        if (promptCanvas != null)
        {
            promptCanvas.SetActive(false);
        }

        // Automatically find the Audio Source on this TeleportPad.
        if (teleportAudioSource == null)
        {
            teleportAudioSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (!playerIsOnPad || isTeleporting)
        {
            return;
        }

        InputDevice leftHand =
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        // X BUTTON — Confirm teleport
        if (leftHand.TryGetFeatureValue(
            CommonUsages.primaryButton,
            out bool isXPressed))
        {
            if (isXPressed && !wasXPressed)
            {
                Debug.Log(
                    ">>> X Pushed! Starting teleport transition... <<<");

                StartCoroutine(TeleportRoutine());
            }

            wasXPressed = isXPressed;
        }

        // Y BUTTON — Cancel teleport
        if (leftHand.TryGetFeatureValue(
            CommonUsages.secondaryButton,
            out bool isYPressed))
        {
            if (isYPressed && !wasYPressed)
            {
                Debug.Log(
                    ">>> Y Pushed! Canceling teleport... <<<");

                HidePrompt();
            }

            wasYPressed = isYPressed;
        }
    }

    private IEnumerator TeleportRoutine()
    {
        isTeleporting = true;
        playerIsOnPad = false;

        if (promptCanvas != null)
        {
            promptCanvas.SetActive(false);
        }

        // Play the teleport sound.
        if (teleportAudioSource != null && teleportSFX != null)
        {
            teleportAudioSource.PlayOneShot(
                teleportSFX,
                teleportVolume);
        }
        else
        {
            Debug.LogWarning(
                "[TeleportPad] Teleport Audio Source or SFX is missing.");
        }

        // Wait so the sound can play before the current scene disappears.
        yield return new WaitForSeconds(teleportDelay);

        if (string.IsNullOrWhiteSpace(sceneToLoad))
        {
            Debug.LogError(
                "[TeleportPad] No destination scene has been entered.");

            isTeleporting = false;
            yield break;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTeleporting)
        {
            playerIsOnPad = true;

            if (promptCanvas != null)
            {
                promptCanvas.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isTeleporting)
        {
            HidePrompt();
        }
    }

    private void HidePrompt()
    {
        playerIsOnPad = false;
        wasXPressed = false;
        wasYPressed = false;

        if (promptCanvas != null)
        {
            promptCanvas.SetActive(false);
        }
    }
}