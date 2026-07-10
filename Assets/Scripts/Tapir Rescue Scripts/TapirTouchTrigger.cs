using UnityEngine;

public class TapirTouchTrigger : MonoBehaviour
{
    [Header("Mission")]
    public TapirMissionV2 mission;

    [Header("Player")]
    public Transform playerCamera;

    [Header("Distance Settings")]
    public float touchDistance = 1.5f;
    public float leaveDistance = 3.0f;

    [Header("Trigger Settings")]
    public bool requirePlayerLeaveFirst = true;
    public bool triggerOnlyOnce = true;

    private bool hasTriggered = false;
    private bool playerHasLeftArea = false;
    private bool missionReadyDetected = false;

    private void Update()
    {
        if (triggerOnlyOnce && hasTriggered)
            return;

        if (mission == null || playerCamera == null)
            return;

        // Only allow tapir walking after all trunks are cleared
        if (mission.state != TapirMissionV2.MissionState.WaitingForTapirTouch)
        {
            missionReadyDetected = false;
            playerHasLeftArea = false;
            return;
        }

        float distance = Vector3.Distance(transform.position, playerCamera.position);

        if (!missionReadyDetected)
        {
            missionReadyDetected = true;
            Debug.Log("[Tapir Touch] Tapir is idle now. Walk near the tapir to trigger movement.");
        }

        // This prevents instant auto-trigger if player is already standing near the tapir
        if (requirePlayerLeaveFirst && !playerHasLeftArea)
        {
            if (distance >= leaveDistance)
            {
                playerHasLeftArea = true;
                Debug.Log("[Tapir Touch] Player moved away. Now walk near the tapir to make it walk.");
            }

            return;
        }

        if (distance <= touchDistance)
        {
            hasTriggered = true;
            Debug.Log("[Tapir Touch] Player walked near tapir. Tapir walking triggered.");
            mission.TouchTapirToWalk();
        }
    }
}