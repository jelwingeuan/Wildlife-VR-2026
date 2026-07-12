using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class VRSceneTeleporter : MonoBehaviour
{
    [Header("Scene Destination")]
    public string sceneToLoad;

    [Header("UI Prompt")]
    public GameObject promptCanvas;

    [Header("Teleport Pad Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip alertSFX;

    private bool playerIsOnPad = false;

    // Prevents buttons from activating repeatedly while held
    private bool wasXPressed = false;
    private bool wasYPressed = false;

    private void Start()
    {
        if (promptCanvas != null)
            promptCanvas.SetActive(false);

        // Automatically find the Audio Source on this TeleportPad
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!playerIsOnPad)
            return;

        InputDevice leftHand =
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        // X button — confirm teleport
        if (leftHand.TryGetFeatureValue(
            CommonUsages.primaryButton,
            out bool isXPressed))
        {
            if (isXPressed && !wasXPressed)
            {
                Debug.Log(
                    ">>> DIRECT HARDWARE: X Pushed! Teleporting... <<<"
                );

                SceneManager.LoadScene(sceneToLoad);
            }

            wasXPressed = isXPressed;
        }

        // Y button — cancel teleport
        if (leftHand.TryGetFeatureValue(
            CommonUsages.secondaryButton,
            out bool isYPressed))
        {
            if (isYPressed && !wasYPressed)
            {
                Debug.Log(
                    ">>> DIRECT HARDWARE: Y Pushed! Canceling... <<<"
                );

                HidePrompt();
            }

            wasYPressed = isYPressed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Prevent repeated trigger events
        if (playerIsOnPad)
            return;

        playerIsOnPad = true;

        if (promptCanvas != null)
            promptCanvas.SetActive(true);

        // Play alert sound once
        if (audioSource != null && alertSFX != null)
            audioSource.PlayOneShot(alertSFX);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            HidePrompt();
    }

    private void HidePrompt()
    {
        playerIsOnPad = false;

        // Reset controller button states
        wasXPressed = false;
        wasYPressed = false;

        if (promptCanvas != null)
            promptCanvas.SetActive(false);
    }
}