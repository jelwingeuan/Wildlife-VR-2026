using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TeleportPadSceneLoader : MonoBehaviour
{
    [Header("Scene Loading")]
    public string targetSceneName = "Tapir_Rescue_Zone";

    [Header("Teleport Settings")]
    public bool requireConfirm = true;
    public bool selectsTapirMission = true;

    [Header("Prompt Text")]
    public string promptMessage = "Teleport to Tapir Rescue Zone?";
    public string confirmText = "Press X or Enter to Confirm";
    public string cancelText = "Press Y or Backspace to Cancel";

    private bool playerInside = false;

    private void Update()
    {
        if (!playerInside)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.xKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
        {
            ConfirmTeleport();
        }

        if (Keyboard.current.yKey.wasPressedThisFrame || Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            CancelTeleport();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
            Debug.Log("Teleport cancelled: player left pad.");
        }
    }

    public void ConfirmTeleport()
    {
        if (!playerInside && requireConfirm)
            return;

        if (selectsTapirMission && GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.SelectTapirMission();
        }

        Debug.Log("Loading scene: " + targetSceneName);
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

        GUI.Box(new Rect(20, 20, 720, 140), "");
        GUI.Label(new Rect(40, 45, 680, 35), promptMessage, style);
        GUI.Label(new Rect(40, 85, 680, 35), confirmText, style);
        GUI.Label(new Rect(40, 120, 680, 35), cancelText, style);
    }
}