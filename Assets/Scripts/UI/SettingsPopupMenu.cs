using UnityEngine;
using UnityEngine.XR;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SettingsPopupMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject settingsPopup;

    private InputDevice leftController;

    private bool wasXPressed;
    private bool wasYPressed;

    private void Start()
    {
        if (settingsPopup != null)
            settingsPopup.SetActive(false);

        FindLeftController();
    }

    private void Update()
    {
        // Only check X and Y while the settings popup is open.
        if (settingsPopup == null || !settingsPopup.activeSelf)
        {
            wasXPressed = false;
            wasYPressed = false;
            return;
        }

        if (!leftController.isValid)
            FindLeftController();

        bool xPressed = false;
        bool yPressed = false;

        if (leftController.isValid)
        {
            // Left controller primary button = X
            leftController.TryGetFeatureValue(
                CommonUsages.primaryButton,
                out xPressed
            );

            // Left controller secondary button = Y
            leftController.TryGetFeatureValue(
                CommonUsages.secondaryButton,
                out yPressed
            );
        }

        // X — Exit
        if (xPressed && !wasXPressed)
            ExitGame();

        // Y — Continue
        if (yPressed && !wasYPressed)
            ContinueGame();

        wasXPressed = xPressed;
        wasYPressed = yPressed;
    }

    private void FindLeftController()
    {
        leftController =
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
    }

    public void OpenSettings()
    {
        if (settingsPopup != null)
            settingsPopup.SetActive(true);

        wasXPressed = false;
        wasYPressed = false;
    }

    public void ContinueGame()
    {
        if (settingsPopup != null)
            settingsPopup.SetActive(false);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}