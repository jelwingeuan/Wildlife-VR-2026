using UnityEngine;

public class TrunkClearOnRelease : MonoBehaviour
{
    [Header("Mission")]
    public TapirMissionV2 mission;

    [Header("Clear Settings")]
    public float movedDistanceToClear = 2f;
    public bool disappearWhenCleared = true;
    public GameObject clearedEffect;

    private Vector3 startPosition;
    private bool cleared = false;

    private void Start()
    {
        startPosition = transform.position;
    }

    public void CheckClearOnRelease()
    {
        if (cleared)
            return;

        float movedDistance = Vector3.Distance(transform.position, startPosition);

        Debug.Log("[Trunk] Released. Moved distance = " + movedDistance);

        if (movedDistance >= movedDistanceToClear)
        {
            ClearTrunk();
        }
        else
        {
            Debug.Log("[Trunk] Not far enough. Move it farther away.");
        }
    }

    private void ClearTrunk()
    {
        cleared = true;

        Debug.Log("[Trunk] Cleared.");

        if (clearedEffect != null)
            Instantiate(clearedEffect, transform.position, Quaternion.identity);

        if (mission != null)
            mission.OnTrunkCleared(gameObject);
        else
            Debug.LogWarning("[Trunk] Mission is not assigned.");

        if (disappearWhenCleared)
            gameObject.SetActive(false);
    }
}