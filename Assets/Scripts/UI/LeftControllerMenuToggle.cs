using UnityEngine;
using UnityEngine.XR;

public class LeftControllerMenuToggle : MonoBehaviour
{
    [Header("Hand Menu")]
    public GameObject menuRoot;

    private InputDevice leftController;
    private bool wasMenuButtonPressed;

    private void Start()
    {
        FindLeftController();

        if (menuRoot != null)
            menuRoot.SetActive(false);
    }

    private void Update()
    {
        if (!leftController.isValid)
            FindLeftController();

        bool menuButtonPressed = false;

        if (leftController.isValid)
        {
            leftController.TryGetFeatureValue(
                CommonUsages.menuButton,
                out menuButtonPressed
            );
        }

        // Opens or closes once per button press
        if (menuButtonPressed && !wasMenuButtonPressed)
        {
            ToggleMenu();
        }

        wasMenuButtonPressed = menuButtonPressed;
    }

    private void FindLeftController()
    {
        leftController =
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
    }

    public void ToggleMenu()
    {
        if (menuRoot != null)
            menuRoot.SetActive(!menuRoot.activeSelf);
    }

    // Continue button
    public void ContinueGame()
    {
        if (menuRoot != null)
            menuRoot.SetActive(false);
    }

    // Exit Now button
    public void ExitGame()
    {
#if UNITY_EDITOR
        // Stops Play Mode while testing in Unity
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Closes the Quest APK or Windows build
        Application.Quit();
#endif
    }
}