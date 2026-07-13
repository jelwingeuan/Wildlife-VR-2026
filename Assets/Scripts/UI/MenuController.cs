using UnityEngine;
using UnityEngine.XR;

public class MenuController : MonoBehaviour
{
    [Header("Menu Setup")]
    [Tooltip("Drag your UI Canvas (Menu) in here")]
    public GameObject menuCanvas;

    [Header("Input Setup")]
    [Tooltip("Must be LeftHand! Meta locks the RightHand menu button.")]
    public XRNode controllerNode = XRNode.LeftHand;

    // This prevents the menu from flickering on/off if you hold the button down
    private bool wasPressedLastFrame = false;

    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
        bool isMenuButtonPressed = false;

        if (device.isValid)
        {
            // Listen specifically for the Menu Button (the three lines button)
            device.TryGetFeatureValue(CommonUsages.menuButton, out isMenuButtonPressed);
        }

        // Only trigger the menu if the button is pressed NOW, but wasn't pressed a split-second ago
        if (isMenuButtonPressed && !wasPressedLastFrame)
        {
            ToggleMenu();
        }

        // Save the current state for the next frame
        wasPressedLastFrame = isMenuButtonPressed;
    }

    private void ToggleMenu()
    {
        if (menuCanvas != null)
        {
            // If the menu is ON, this turns it OFF. If it is OFF, this turns it ON!
            menuCanvas.SetActive(!menuCanvas.activeSelf);
        }
    }
}
