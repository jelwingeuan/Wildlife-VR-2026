using UnityEngine;

public class ItemSwitcher : MonoBehaviour
{
    [Header("Equippable Items")]
    public GameObject seedLauncher;
    public GameObject waterCannon;

    [Header("Hand Setup")]
    [Tooltip("Drag your Right Hand object here so the items know where to snap to")]
    public Transform rightHandTransform;

    public void SwitchItem(int index)
    {
        // 1. Turn off all items first (Empty Hand state)
        if (seedLauncher != null) seedLauncher.SetActive(false);
        if (waterCannon != null) waterCannon.SetActive(false);

        GameObject itemToEquip = null;

        // 2. Figure out which item was selected
        if (index == 0) itemToEquip = seedLauncher;
        else if (index == 1) itemToEquip = waterCannon;

        // 3. Equip the item
        if (itemToEquip != null)
        {
            itemToEquip.SetActive(true);

            // 4. The FPS Gun Logic: Snap it to the hand!
            if (rightHandTransform != null)
            {
                // Attach it to the hand so it follows your movements
                itemToEquip.transform.SetParent(rightHandTransform);

                // Snap its position and rotation perfectly to the hand's center
                itemToEquip.transform.localPosition = Vector3.zero;
                itemToEquip.transform.localRotation = Quaternion.identity;

                // Turn off gravity so it doesn't fall to the floor
                Rigidbody rb = itemToEquip.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
            }
            else
            {
                Debug.LogWarning("[ItemSwitcher] Right Hand Transform is missing! Cannot attach item.");
            }
        }
    }
}