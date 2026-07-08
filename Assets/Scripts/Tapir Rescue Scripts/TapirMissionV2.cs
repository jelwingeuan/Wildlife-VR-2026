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

    [Header("Scan FX")]
    public GameObject tapirScanFX;
    public float scanFXDuration = 10f;

    [Header("Trunks")]
    public int trunksNeeded = 3;
    private int trunksCleared = 0;

    [Header("Tapir Models")]
    public GameObject lyingTapirModel;
    public GameObject walkingTapirModel;
    public Transform tapirRoot;
    public Animator walkingTapirAnimator;

    [Header("Tapir Animation State Names")]
    public string walkStateName = "";
    public string idleStateName = "";

    [Header("Tapir Walk Target")]
    public Transform tapirWalkTarget;
    public float tapirMoveSpeed = 1.2f;
    public float tapirTurnSpeed = 5f;
    public float stopDistance = 0.25f;

    [Header("Mission Complete")]
    public GameObject missionCompletePanel;
    public UnityEvent onMissionComplete;

    [Header("Optional Scene Load")]
    public bool loadNextSceneDirectly = false;
    public string nextSceneName;
    public float sceneLoadDelay = 2f;

    private bool droneStarted = false;
    private bool tapirStartedWalking = false;

    private void Start()
    {
        state = MissionState.WaitingForDroneTouch;

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

        Debug.Log("[Tapir Mission V2] Touch the drone to start scan.");
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
        if (droneObject != null && droneScanPoint != null)
        {
            while (Vector3.Distance(droneObject.transform.position, droneScanPoint.position) > 0.1f)
            {
                droneObject.transform.position = Vector3.MoveTowards(
                    droneObject.transform.position,
                    droneScanPoint.position,
                    droneFlySpeed * Time.deltaTime
                );

                Vector3 lookTarget = droneScanPoint.position;
                Vector3 direction = lookTarget - droneObject.transform.position;

                if (direction.sqrMagnitude > 0.01f)
                {
                    droneObject.transform.rotation = Quaternion.Slerp(
                        droneObject.transform.rotation,
                        Quaternion.LookRotation(direction),
                        5f * Time.deltaTime
                    );
                }

                yield return null;
            }
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
            state = MissionState.WaitingForTapirTouch;
            Debug.Log("[Tapir Mission V2] Tapir rescued. Touch the tapir to make it walk to the drone.");
        }
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

        Debug.Log("[Tapir Mission V2] Tapir touched. Switching to walking model.");

        if (lyingTapirModel != null)
            lyingTapirModel.SetActive(false);

        if (walkingTapirModel != null)
            walkingTapirModel.SetActive(true);

        PlayWalkAnimation();

        StartCoroutine(TapirWalkRoutine());
    }

    private IEnumerator TapirWalkRoutine()
    {
        if (tapirRoot == null || tapirWalkTarget == null)
        {
            Debug.LogWarning("[Tapir Mission V2] Tapir Root or Tapir Walk Target is missing.");
            yield break;
        }

        while (FlatDistance(tapirRoot.position, tapirWalkTarget.position) > stopDistance)
        {
            MoveTapirTowards(tapirWalkTarget.position);
            PlayWalkAnimation();
            yield return null;
        }

        PlayIdleAnimation();

        CompleteMission();
    }

    private void MoveTapirTowards(Vector3 targetPosition)
    {
        Vector3 current = tapirRoot.position;
        Vector3 target = new Vector3(targetPosition.x, current.y, targetPosition.z);

        Vector3 direction = target - current;

        if (direction.magnitude < 0.05f)
            return;

        tapirRoot.position += direction.normalized * tapirMoveSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        tapirRoot.rotation = Quaternion.Slerp(
            tapirRoot.rotation,
            targetRotation,
            tapirTurnSpeed * Time.deltaTime
        );
    }

    private float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    private void PlayWalkAnimation()
    {
        if (walkingTapirAnimator != null && !string.IsNullOrEmpty(walkStateName))
        {
            walkingTapirAnimator.CrossFadeInFixedTime(walkStateName, 0.2f);
        }
    }

    private void PlayIdleAnimation()
    {
        if (walkingTapirAnimator != null && !string.IsNullOrEmpty(idleStateName))
        {
            walkingTapirAnimator.CrossFadeInFixedTime(idleStateName, 0.2f);
        }
    }

    private void CompleteMission()
    {
        state = MissionState.MissionComplete;

        Debug.Log("[Tapir Mission V2] Mission complete. Tapir reached the drone.");

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
