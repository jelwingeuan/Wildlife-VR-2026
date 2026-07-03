using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TeleportPadSceneLoader : MonoBehaviour
{
    [Header("Scene Loading")]
    public string targetSceneName = "01_Tapir_Rescue_Zone";

    [Header("Teleport Settings")]
    public bool requireConfirm = true;
    public bool selectsTapirMission = false;

    [Header("Prompt Text")]
    public string promptMessage = "Teleport?";
    public string confirmText = "Press X or Enter to Confirm";
    public string cancelText = "Press Y or Backspace to Cancel";

    [Header("Debug")]
    public bool showDebugLogs = true;

    private bool playerInside = false;

    private void Update()
    {
        if (!playerInside)
            return;

        // Keyboard testing in Unity Editor
        if (Keyboard.current != null)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.xKey.wasPressedThisFrame)
            {
                ConfirmTeleport();
            }

            if (Keyboard.current.backspaceKey.wasPressedThisFrame || Keyboard.current.yKey.wasPressedThisFrame)
            {
                CancelTeleport();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (showDebugLogs)
        {
            Debug.Log("Teleport pad touched by: " + other.name + " | Tag: " + other.tag);
        }

        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log(promptMessage + " " + confirmText);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("Teleport prompt closed. Player left teleport pad.");
        }
    }

    public void ConfirmTeleport()
    {
        if (requireConfirm && !playerInside)
        {
            Debug.Log("Cannot teleport because player is not inside teleport pad.");
            return;
        }

        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("Target Scene Name is empty. Please type the scene name in the Inspector.");
            return;
        }

        if (selectsTapirMission && GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.SelectTapirMission();
        }

        Debug.Log("Teleport confirmed. Loading scene: " + targetSceneName);
        SceneManager.LoadScene(targetSceneName);
    }

    public void CancelTeleport()
    {
        playerInside = false;
        Debug.Log("Teleport cancelled.");
    }

    private void OnGUI()
    {
        if (!playerInside)
            return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 26;
        style.normal.textColor = Color.white;

        GUI.Box(new Rect(20, 20, 800, 150), "");
        GUI.Label(new Rect(40, 45, 760, 35), promptMessage, style);
        GUI.Label(new Rect(40, 85, 760, 35), confirmText, style);
        GUI.Label(new Rect(40, 120, 760, 35), cancelText, style);
    }
}