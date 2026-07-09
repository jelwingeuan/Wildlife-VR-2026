using UnityEngine;

public class TapirTouchTrigger : MonoBehaviour
{
    [Header("Mission")]
    public TapirMissionV2 mission;

    [Header("Player")]
    public Transform playerCamera;

    [Header("Touch Distance")]
    public float touchDistance = 2.0f;
    public bool triggerOnlyOnce = true;

    private bool hasTriggered = false;

    private void Update()
    {
        if (triggerOnlyOnce && hasTriggered)
            return;

        if (mission == null || playerCamera == null)
            return;

        // Only works after all trunks are cleared
        if (mission.state != TapirMissionV2.MissionState.WaitingForTapirTouch)
            return;

        float distance = Vector3.Distance(transform.position, playerCamera.position);

        if (distance <= touchDistance)
        {
            hasTriggered = true;
            Debug.Log("[Tapir Touch] Player is close enough. Tapir walking triggered.");
            mission.TouchTapirToWalk();
        }
    }
}