using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class VRSceneTeleporter : MonoBehaviour
{
    [Header("Scene Destination")]
    public string sceneToLoad;

    [Header("UI Prompt")]
    public GameObject promptCanvas;

    [Header("Teleport Transition Audio")]
    [SerializeField] private AudioSource teleportAudioSource;
    [SerializeField] private AudioClip teleportSfx;

    [Tooltip("Minimum delay before loading the next scene.")]
    [SerializeField, Min(0f)] private float teleportDelay = 1.2f;

    [SerializeField, Range(0f, 1f)] private float teleportVolume = 1f;

    private bool playerIsOnPad;
    private bool isTeleporting;

    // Prevents the buttons from triggering repeatedly while held.
    private bool wasXPressed;
    private bool wasYPressed;

    private void Awake()
    {
        // Automatically finds an Audio Source on the TeleportPad
        // if one was not manually assigned.
        if (teleportAudioSource == null)
        {
            teleportAudioSource = GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        if (promptCanvas != null)
        {
            promptCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerIsOnPad || isTeleporting)
            return;

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
                Debug.Log(">>> Y Pushed! Canceling... <<<");
                HidePrompt();
            }

            wasYPressed = isYPressed;
        }
    }

    private IEnumerator TeleportRoutine()
    {
        if (isTeleporting)
            yield break;

        if (string.IsNullOrWhiteSpace(sceneToLoad))
        {
            Debug.LogError(
                "[VRSceneTeleporter] Scene To Load is empty.");

            yield break;
        }

        isTeleporting = true;
        playerIsOnPad = false;

        // Hide the confirmation UI.
        if (promptCanvas != null)
        {
            promptCanvas.SetActive(false);
        }

        // Play teleport transition sound.
        if (teleportAudioSource != null && teleportSfx != null)
        {
            teleportAudioSource.PlayOneShot(
                teleportSfx,
                teleportVolume);
        }
        else
        {
            Debug.LogWarning(
                "[VRSceneTeleporter] Teleport Audio Source " +
                "or Teleport SFX has not been assigned.");
        }

        // Wait before changing scene so the sound can play.
        float waitTime = teleportDelay;

        if (teleportSfx != null)
        {
            waitTime = Mathf.Max(
                teleportDelay,
                teleportSfx.length);
        }

        yield return new WaitForSecondsRealtime(waitTime);

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

        if (promptCanvas != null)
        {
            promptCanvas.SetActive(false);
        }
    }
}