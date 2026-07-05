using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.XR; // Lets us talk directly to the headset hardware

public class RadialSelection : MonoBehaviour
{
    [Header("Radial Settings")]
    [Range(2, 10)]
    public int numberOfRadialPart = 3;
    public GameObject radialPartPrefab;
    public Transform radialPartCanvas;
    public float angleBetweenPart = 10;
    public Transform handTransform;

    [Header("Events")]
    public UnityEvent<int> OnPartSelected;

    private List<GameObject> spawnedParts = new List<GameObject>();
    private int currentSelectedRadialPart = -1;

    // We use this to track if the button was just pressed or just released
    private bool wasButtonPressedLastFrame = false;

    void Update()
    {
        // 1. Get the Right Hand controller directly from the Quest hardware
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        bool isBPressed = false;

        // 2. Check if the "Secondary Button" (B button) is currently being held down
        if (rightHand.isValid)
        {
            rightHand.TryGetFeatureValue(CommonUsages.secondaryButton, out isBPressed);
        }

        // 3. Trigger when the button is PUSHED DOWN
        if (isBPressed && !wasButtonPressedLastFrame)
        {
            SpawnRadialPart();
        }

        // 4. Trigger when the button is RELEASED
        else if (!isBPressed && wasButtonPressedLastFrame)
        {
            HideAndTriggerSelected();
        }

        // 5. Save the state for the next frame
        wasButtonPressedLastFrame = isBPressed;

        // 6. Only run calculations if the UI is currently open/visible
        if (radialPartCanvas != null && radialPartCanvas.gameObject.activeSelf)
        {
            GetSelectedRadialPart();
        }
    }

    public void SpawnRadialPart()
    {
        if (radialPartCanvas == null || handTransform == null || radialPartPrefab == null)
        {
            Debug.LogError("[RadialSelection] Cannot spawn! Canvas, Hand Transform, or Prefab is missing.");
            return;
        }

        radialPartCanvas.gameObject.SetActive(true);
        radialPartCanvas.position = handTransform.position;

        // --- THE ANGLE FIX ---
        // Forces the menu to always face the player's headset (Main Camera)
        if (Camera.main != null)
        {
            Vector3 directionAwayFromPlayer = radialPartCanvas.position - Camera.main.transform.position;
            radialPartCanvas.rotation = Quaternion.LookRotation(directionAwayFromPlayer);
        }
        else
        {
            // Fallback just in case the camera isn't tagged as MainCamera
            radialPartCanvas.rotation = handTransform.rotation;
        }
        // ----------------------

        // Clear previous parts
        foreach (var item in spawnedParts)
        {
            Destroy(item);
        }
        spawnedParts.Clear();

        // Build the radial pieces
        for (int i = 0; i < numberOfRadialPart; i++)
        {
            float angle = -i * 360f / numberOfRadialPart - angleBetweenPart / 2f;
            Vector3 radialPartEulerAngle = new Vector3(0, 0, angle);

            GameObject spawnedRadialPart = Instantiate(radialPartPrefab, radialPartCanvas);
            spawnedRadialPart.transform.position = radialPartCanvas.position;
            spawnedRadialPart.transform.localEulerAngles = radialPartEulerAngle;

            Image img = spawnedRadialPart.GetComponent<Image>();
            if (img != null)
            {
                // Note: The UI Image component on the prefab needs Image Type set to "Filled"
                img.fillAmount = (1f / (float)numberOfRadialPart) - (angleBetweenPart / 360f);
            }

            spawnedParts.Add(spawnedRadialPart);
        }
    }

    public void GetSelectedRadialPart()
    {
        Vector3 centerToHand = handTransform.position - radialPartCanvas.position;
        Vector3 centerToHandProjected = Vector3.ProjectOnPlane(centerToHand, radialPartCanvas.forward);

        float angle = Vector3.SignedAngle(radialPartCanvas.up, centerToHandProjected, -radialPartCanvas.forward);

        if (angle < 0)
            angle += 360;

        currentSelectedRadialPart = (int)(angle * numberOfRadialPart / 360);

        for (int i = 0; i < spawnedParts.Count; i++)
        {
            Image img = spawnedParts[i].GetComponent<Image>();
            if (img == null) continue;

            if (i == currentSelectedRadialPart)
            {
                img.color = Color.yellow;
                spawnedParts[i].transform.localScale = 1.1f * Vector3.one;
            }
            else
            {
                img.color = Color.white;
                spawnedParts[i].transform.localScale = 1f * Vector3.one;
            }
        }
    }

    public void HideAndTriggerSelected()
    {
        if (currentSelectedRadialPart != -1)
        {
            OnPartSelected.Invoke(currentSelectedRadialPart);
        }

        radialPartCanvas.gameObject.SetActive(false);
    }
}