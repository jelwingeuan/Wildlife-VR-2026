using UnityEngine;
using UnityEngine.XR;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SimpleHandMenu : MonoBehaviour
{
    public static bool IsOpen { get; private set; }

    [Header("Hand Menu")]
    [SerializeField] private GameObject handMenuCanvas;

    private InputDevice leftController;

    private bool wasMenuPressed;
    private bool wasXPressed;
    private bool wasYPressed;

    private void Start()
    {
        IsOpen = false;

        if (handMenuCanvas != null)
            handMenuCanvas.SetActive(false);

        FindLeftController();
    }

    private void Update()
    {
        if (!leftController.isValid)
            FindLeftController();

        bool menuPressed = false;
        bool xPressed = false;
        bool yPressed = false;

        if (leftController.isValid)
        {
            // Three-line Menu button on the left controller.
            leftController.TryGetFeatureValue(
                CommonUsages.menuButton,
                out menuPressed
            );

            // X button.
            leftController.TryGetFeatureValue(
                CommonUsages.primaryButton,
                out xPressed
            );

            // Y button.
            leftController.TryGetFeatureValue(
                CommonUsages.secondaryButton,
                out yPressed
            );
        }

        // Press the Menu button once to open or close the menu.
        if (menuPressed && !wasMenuPressed)
            ToggleMenu();

        // X and Y only work as menu controls while the menu is open.
        if (IsOpen)
        {
            if (xPressed && !wasXPressed)
            {
                ExitGame();
            }
            else if (yPressed && !wasYPressed)
            {
                ContinueGame();
            }
        }

        wasMenuPressed = menuPressed;
        wasXPressed = xPressed;
        wasYPressed = yPressed;
    }

    private void FindLeftController()
    {
        leftController =
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
    }

    private void ToggleMenu()
    {
        if (handMenuCanvas == null)
            return;

        IsOpen = !IsOpen;
        handMenuCanvas.SetActive(IsOpen);
    }

    private void ContinueGame()
    {
        IsOpen = false;

        if (handMenuCanvas != null)
            handMenuCanvas.SetActive(false);
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        // Stops Play Mode while testing in Unity.
        EditorApplication.isPlaying = false;
#else
        // Closes the Quest application.
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        IsOpen = false;
    }
}