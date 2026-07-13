using UnityEngine;
using UnityEngine.XR;

public class StandaloneLeftControllerTracker : MonoBehaviour
{
    [Header("Optional Tracking Origin")]
    [Tooltip("Leave empty if your player starts at world position 0,0,0.")]
    [SerializeField] private Transform trackingOrigin;

    private InputDevice leftController;

    private void Start()
    {
        FindLeftController();
    }

    private void Update()
    {
        if (!leftController.isValid)
            FindLeftController();

        if (!leftController.isValid)
            return;

        bool hasPosition = leftController.TryGetFeatureValue(
            CommonUsages.devicePosition,
            out Vector3 controllerPosition
        );

        bool hasRotation = leftController.TryGetFeatureValue(
            CommonUsages.deviceRotation,
            out Quaternion controllerRotation
        );

        if (hasPosition)
        {
            if (trackingOrigin != null)
                transform.position =
                    trackingOrigin.TransformPoint(controllerPosition);
            else
                transform.position = controllerPosition;
        }

        if (hasRotation)
        {
            if (trackingOrigin != null)
                transform.rotation =
                    trackingOrigin.rotation * controllerRotation;
            else
                transform.rotation = controllerRotation;
        }
    }

    private void FindLeftController()
    {
        leftController =
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
    }
}