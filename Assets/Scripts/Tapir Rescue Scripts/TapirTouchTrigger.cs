using UnityEngine;

public class TapirTouchTrigger : MonoBehaviour
{
    [Header("Mission")]
    public TapirMissionV2 mission;

    [Header("Player")]
    public Transform playerCamera;

    [Header("Touch Distance")]
    public float touchDistance = 2.0f;
    public float leaveDistance = 2.8f;

    [Header("Timing")]
    public float readyDelay = 1.5f;
    public bool triggerOnlyOnce = true;

    private bool hasTriggered = false;
    private bool missionReadyDetected = false;
    private bool playerHasLeftTapirArea = false;
    private float readyTimer = 0f;

    private void Update()
    {
        if (triggerOnlyOnce && hasTriggered)
            return;

        if (mission == null || playerCamera == null)
            return;

        if (mission.state != TapirMissionV2.MissionState.WaitingForTapirTouch)
        {
            missionReadyDetected = false;
            playerHasLeftTapirArea = false;
            readyTimer = 0f;
            return;
        }

        float distance = Vector3.Distance(transform.position, playerCamera.position);

        if (!missionReadyDetected)
        {
            missionReadyDetected = true;
            readyTimer = 0f;

            Debug.Log("[Tapir Touch] Tapir is now idle. Player must move away and come back to trigger walking.");
        }

        readyTimer += Time.deltaTime;

        if (readyTimer < readyDelay)
            return;

        if (!playerHasLeftTapirArea)
        {
            if (distance >= leaveDistance)
            {
                playerHasLeftTapirArea = true;
                Debug.Log("[Tapir Touch] Player left tapir area. Come close again to make tapir walk.");
            }

            return;
        }

        if (distance <= touchDistance)
        {
            hasTriggered = true;
            Debug.Log("[Tapir Touch] Player returned close to tapir. Walking triggered.");
            mission.TouchTapirToWalk();
        }
    }
}