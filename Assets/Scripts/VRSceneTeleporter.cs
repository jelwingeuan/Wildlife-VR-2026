using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR; // This lets us read the headset hardware directly

public class VRSceneTeleporter : MonoBehaviour
{
    [Header("Scene Destination")]
    public string sceneToLoad;

    [Header("UI Prompt")]
    public GameObject promptCanvas;

    private bool playerIsOnPad = false;

    // These stop the button from firing 90 times a second while you hold it down
    private bool wasXPressed = false;
    private bool wasYPressed = false;

    void Start()
    {
        if (promptCanvas != null)
            promptCanvas.SetActive(false);
    }

    void Update()
    {
        // If the player isn't on the pad, don't bother checking the buttons
        if (!playerIsOnPad) return;

        // Find the Left Hand Controller Hardware
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        // --- CHECK X BUTTON (Primary) ---
        if (leftHand.TryGetFeatureValue(CommonUsages.primaryButton, out bool isXPressed))
        {
            if (isXPressed && !wasXPressed) // Only trigger the exact moment it is pushed
            {
                Debug.Log(">>> DIRECT HARDWARE: X Pushed! Teleporting... <<<");
                SceneManager.LoadScene(sceneToLoad);
            }
            wasXPressed = isXPressed;
        }

        // --- CHECK Y BUTTON (Secondary) ---
        if (leftHand.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isYPressed))
        {
            if (isYPressed && !wasYPressed)
            {
                Debug.Log(">>> DIRECT HARDWARE: Y Pushed! Canceling... <<<");
                HidePrompt();
            }
            wasYPressed = isYPressed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsOnPad = true;
            if (promptCanvas != null)
                promptCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HidePrompt();
        }
    }

    private void HidePrompt()
    {
        playerIsOnPad = false;
        if (promptCanvas != null)
            promptCanvas.SetActive(false);
    }
}
