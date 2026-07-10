using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TapirMissionV2 : MonoBehaviour
{
    public enum MissionState
    {
        WaitingForDroneTouch,
        DroneScanning,
        RemovingTrunks,
        WaitingForTapirTouch,
        TapirWalkingToDrone,
        MissionComplete
    }

    [Header("Mission State")]
    public MissionState state = MissionState.WaitingForDroneTouch;

    [Header("Drone")]
    public GameObject droneObject;
    public Transform droneStartPoint;
    public Transform droneScanPoint;
    public float droneFlySpeed = 3f;
    public bool lockDroneRotationToStartPoint = true;
    public bool hideDroneAfterScan = true;

    [Header("Scan FX")]
    public GameObject tapirScanFX;
    public float scanFXDuration = 10f;

    [Header("Trunks")]
    public int trunksNeeded = 2;
    private int trunksCleared = 0;

    [Header("Tapir Models")]
    public GameObject lyingTapirModel;
    public GameObject walkingTapirModel;
    public Transform tapirRoot;
    public Animator walkingTapirAnimator;

    [Header("Tapir Animation State Names")]
    public string walkStateName = "rig|walk";
    public string idleStateName = "rig|idle";

    [Header("Tapir Path")]
    public Transform[] tapirPathPoints;
    public Transform tapirWalkTarget; // backup if no path points are assigned
    public float tapirMoveSpeed = 1.2f;
    public float tapirTurnSpeed = 5f;
    public float stopDistance = 0.5f;

    [Header("Ground Follow")]
    public bool keepCurrentHeight = true;

    [Header("Mission Complete")]
    public GameObject missionCompletePanel;
    public UnityEvent onMissionComplete;

    [Header("Optional Scene Load")]
    public bool loadNextSceneDirectly = false;
    public string nextSceneName;
    public float sceneLoadDelay = 2f;

    private bool droneStarted = false;
    private bool tapirModelSwitched = false;
    private bool tapirStartedWalking = false;
    private string currentAnimationState = "";

    private void Start()
    {
        state = MissionState.WaitingForDroneTouch;

        trunksCleared = 0;
        droneStarted = false;
        tapirModelSwitched = false;
        tapirStartedWalking = false;

        if (droneObject != null)
        {
            droneObject.SetActive(true);

            if (droneStartPoint != null)
            {
                droneObject.transform.position = droneStartPoint.position;
                droneObject.transform.rotation = droneStartPoint.rotation;
            }
        }

        if (tapirScanFX != null)
            tapirScanFX.SetActive(false);

        if (lyingTapirModel != null)
            lyingTapirModel.SetActive(true);

        if (walkingTapirModel != null)
            walkingTapirModel.SetActive(false);

        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(false);

        Debug.Log("[Tapir Mission V2] Touch/select the drone to start scan.");
    }

    public void StartDroneScan()
    {
        if (droneStarted)
            return;

        if (state != MissionState.WaitingForDroneTouch)
            return;

        droneStarted = true;
        state = MissionState.DroneScanning;

        Debug.Log("[Tapir Mission V2] Drone touched. Flying to tapir.");

        StartCoroutine(DroneScanRoutine());
    }

    private IEnumerator DroneScanRoutine()
    {
        Quaternion fixedDroneRotation = Quaternion.identity;

        if (droneStartPoint != null)
            fixedDroneRotation = droneStartPoint.rotation;
        else if (droneObject != null)
            fixedDroneRotation = droneObject.transform.rotation;

        if (droneObject != null)
            droneObject.transform.rotation = fixedDroneRotation;

        if (droneObject != null && droneScanPoint != null)
        {
            while (Vector3.Distance(droneObject.transform.position, droneScanPoint.position) > 0.1f)
            {
                droneObject.transform.position = Vector3.MoveTowards(
                    droneObject.transform.position,
                    droneScanPoint.position,
                    droneFlySpeed * Time.deltaTime
                );

                if (lockDroneRotationToStartPoint)
                    droneObject.transform.rotation = fixedDroneRotation;

                yield return null;
            }

            if (lockDroneRotationToStartPoint)
                droneObject.transform.rotation = fixedDroneRotation;
        }

        Debug.Log("[Tapir Mission V2] Drone reached tapir side. Showing scan FX.");

        if (tapirScanFX != null)
        {
            if (droneScanPoint != null)
                tapirScanFX.transform.position = droneScanPoint.position;

            tapirScanFX.SetActive(true);
        }

        yield return new WaitForSeconds(scanFXDuration);

        if (tapirScanFX != null)
            tapirScanFX.SetActive(false);

        if (hideDroneAfterScan && droneObject != null)
            droneObject.SetActive(false);

        state = MissionState.RemovingTrunks;

        Debug.Log("[Tapir Mission V2] Scan finished. Clear the trunks.");
    }

    public void OnTrunkCleared(GameObject trunk)
    {
        if (state != MissionState.RemovingTrunks)
        {
            Debug.Log("[Tapir Mission V2] Trunk ignored. Drone scan must finish first.");
            return;
        }

        trunksCleared++;

        Debug.Log("[Tapir Mission V2] Trunk cleared: " + trunksCleared + " / " + trunksNeeded);

        if (trunksCleared >= trunksNeeded)
        {
            SwitchTapirToWalkingModelOnly();

            state = MissionState.WaitingForTapirTouch;
            Debug.Log("[Tapir Mission V2] All trunks cleared. Tapir changed to walking model. Touch/select tapir to make it walk.");
        }
    }

    private void SwitchTapirToWalkingModelOnly()
    {
        if (tapirModelSwitched)
            return;

        tapirModelSwitched = true;

        if (lyingTapirModel != null)
            lyingTapirModel.SetActive(false);

        if (walkingTapirModel != null)
            walkingTapirModel.SetActive(true);

        PlayIdleAnimation();

        Debug.Log("[Tapir Mission V2] Tapir switched from lying model to walking model.");
    }

    public void TouchTapirToWalk()
    {
        if (tapirStartedWalking)
            return;

        if (state != MissionState.WaitingForTapirTouch)
        {
            Debug.Log("[Tapir Mission V2] Tapir cannot walk yet. Clear all trunks first.");
            return;
        }

        tapirStartedWalking = true;
        state = MissionState.TapirWalkingToDrone;

        Debug.Log("[Tapir Mission V2] Tapir touched. Walking to target.");

        PlayWalkAnimation();

        StartCoroutine(TapirWalkRoutine());
    }
    private IEnumerator TapirWalkRoutine()
    {
        if (tapirRoot == null)
        {
            Debug.LogWarning("[Tapir Mission V2] Tapir Root is missing.");
            yield break;
        }

        // Use path points if assigned
        if (tapirPathPoints != null && tapirPathPoints.Length > 0)
        {
            Debug.Log("[Tapir Mission V2] Tapir following waypoint path.");

            for (int i = 0; i < tapirPathPoints.Length; i++)
            {
                if (tapirPathPoints[i] == null)
                    continue;

                Transform currentPoint = tapirPathPoints[i];

                Debug.Log("[Tapir Mission V2] Moving to path point: " + currentPoint.name);

                while (FlatDistance(tapirRoot.position, currentPoint.position) > stopDistance)
                {
                    MoveTapirTowards(currentPoint.position);
                    PlayWalkAnimation();
                    yield return null;
                }
            }
        }
        else
        {
            // Backup: use single target if no path points are assigned
            if (tapirWalkTarget == null)
            {
                Debug.LogWarning("[Tapir Mission V2] No Tapir Path Points or Tapir Walk Target assigned.");
                yield break;
            }

            Debug.Log("[Tapir Mission V2] No path points assigned. Moving to single target.");

            while (FlatDistance(tapirRoot.position, tapirWalkTarget.position) > stopDistance)
            {
                MoveTapirTowards(tapirWalkTarget.position);
                PlayWalkAnimation();
                yield return null;
            }
        }

        PlayIdleAnimation();
        CompleteMission();
    }
    private void MoveTapirTowards(Vector3 targetPosition)
    {
        Vector3 current = tapirRoot.position;

        Vector3 target;

        if (keepCurrentHeight)
        {
            target = new Vector3(targetPosition.x, current.y, targetPosition.z);
        }
        else
        {
            target = targetPosition;
        }

        Vector3 direction = target - current;

        if (direction.magnitude < 0.05f)
            return;

        Vector3 move = direction.normalized * tapirMoveSpeed * Time.deltaTime;
        tapirRoot.position += move;

        Vector3 lookDirection = new Vector3(direction.x, 0f, direction.z);

        if (lookDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection.normalized);

            tapirRoot.rotation = Quaternion.Slerp(
                tapirRoot.rotation,
                targetRotation,
                tapirTurnSpeed * Time.deltaTime
            );
        }
    }
    private float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    private void PlayWalkAnimation()
    {
        if (walkingTapirAnimator == null || string.IsNullOrEmpty(walkStateName))
            return;

        if (currentAnimationState == walkStateName)
            return;

        currentAnimationState = walkStateName;

        walkingTapirAnimator.speed = 1f;
        walkingTapirAnimator.CrossFadeInFixedTime(walkStateName, 0.15f);

        Debug.Log("[Tapir Mission V2] Playing walk animation: " + walkStateName);
    }

    private void PlayIdleAnimation()
    {
        if (walkingTapirAnimator == null || string.IsNullOrEmpty(idleStateName))
            return;

        if (currentAnimationState == idleStateName)
            return;

        currentAnimationState = idleStateName;

        walkingTapirAnimator.speed = 1f;
        walkingTapirAnimator.CrossFadeInFixedTime(idleStateName, 0.15f);

        Debug.Log("[Tapir Mission V2] Playing idle animation: " + idleStateName);
    }

    private void CompleteMission()
    {
        state = MissionState.MissionComplete;

        Debug.Log("[Tapir Mission V2] Mission complete. Tapir reached target.");

        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(true);

        onMissionComplete.Invoke();

        if (loadNextSceneDirectly && !string.IsNullOrEmpty(nextSceneName))
        {
            StartCoroutine(LoadNextSceneRoutine());
        }
    }

    private IEnumerator LoadNextSceneRoutine()
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        SceneManager.LoadScene(nextSceneName);
    }
}