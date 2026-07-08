using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TapirRescueMission : MonoBehaviour
{
    public enum MissionState
    {
        WaitingForScan,
        RemovingTrunks,
        GuidingTapir,
        BoardingHelicopter,
        MissionComplete
    }

    [Header("Mission State")]
    public MissionState state = MissionState.WaitingForScan;

    [Header("Drone Scan")]
    public GameObject droneObject;
    public Transform droneStartPoint;
    public Transform droneScanTarget;
    public float droneFlySpeed = 4f;
    public GameObject tapirScanMarker;
    public GameObject trunkMarker;

    [Header("Trunk Rescue")]
    public int trunksNeeded = 1;
    private int trunksCleared = 0;

    [Header("Tapir Models")]
    public GameObject stuckTapirModel;
    public GameObject walkingTapirModel;
    public Transform tapirRoot;
    public Animator tapirAnimator;

    [Header("Animation State Names")]
    public string idleAnimationState = "Idle";
    public string walkAnimationState = "Walk";

    [Header("Guiding Tapir")]
    public Transform playerTarget;
    public Transform helicopterDoorPoint;
    public Transform helicopterInsidePoint;
    public float tapirMoveSpeed = 1.2f;
    public float tapirTurnSpeed = 5f;
    public float stopDistanceFromPlayer = 1.5f;
    public float helicopterBoardDistance = 2f;

    [Header("UI")]
    public TextMeshProUGUI objectiveText;
    public GameObject missionCompletePanel;

    [Header("Mission End")]
    public UnityEvent onMissionComplete;
    public bool loadNextSceneDirectly = false;
    public string nextSceneName;
    public float endDelay = 2.5f;

    [Header("Testing")]
    public bool allowKeyboardTesting = true;

    private bool isBoarding = false;

    private void Start()
    {
        if (droneObject != null)
            droneObject.SetActive(false);

        if (tapirScanMarker != null)
            tapirScanMarker.SetActive(false);

        if (trunkMarker != null)
            trunkMarker.SetActive(false);

        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(false);

        if (stuckTapirModel != null)
            stuckTapirModel.SetActive(true);

        if (walkingTapirModel != null)
            walkingTapirModel.SetActive(false);

        SetObjective("Activate the drone scanner to find the trapped tapir.");
    }

    private void Update()
    {
        if (allowKeyboardTesting)
        {
            if (Input.GetKeyDown(KeyCode.G))
                ActivateDroneScan();

            if (Input.GetKeyDown(KeyCode.R))
                FreeTapirForTesting();
        }

        if (state == MissionState.GuidingTapir)
        {
            GuideTapirToPlayer();
        }
    }

    public void ActivateDroneScan()
    {
        if (state != MissionState.WaitingForScan)
            return;

        state = MissionState.RemovingTrunks;

        SetObjective("Drone scan complete. Remove the two trunks trapping the tapir.");

        if (droneObject != null)
        {
            droneObject.SetActive(true);

            if (droneStartPoint != null)
            {
                droneObject.transform.position = droneStartPoint.position;
                droneObject.transform.rotation = droneStartPoint.rotation;
            }

            StartCoroutine(DroneScanRoutine());
        }
        else
        {
            ShowScanResult();
        }
    }

    private IEnumerator DroneScanRoutine()
    {
        if (droneObject != null && droneScanTarget != null)
        {
            while (Vector3.Distance(droneObject.transform.position, droneScanTarget.position) > 0.15f)
            {
                droneObject.transform.position = Vector3.MoveTowards(
                    droneObject.transform.position,
                    droneScanTarget.position,
                    droneFlySpeed * Time.deltaTime
                );

                droneObject.transform.LookAt(droneScanTarget);
                yield return null;
            }
        }

        ShowScanResult();
    }

    private void ShowScanResult()
    {
        if (tapirScanMarker != null)
            tapirScanMarker.SetActive(true);

        if (trunkMarker != null)
            trunkMarker.SetActive(true);
    }

    public void OnTrunkCleared(GameObject trunk)
    {
        if (state != MissionState.RemovingTrunks)
            return;

        trunksCleared++;

        SetObjective("Trunks removed: " + trunksCleared + " / " + trunksNeeded);

        if (trunksCleared >= trunksNeeded)
        {
            FreeTapir();
        }
    }

    private void FreeTapir()
    {
        if (stuckTapirModel != null)
            stuckTapirModel.SetActive(false);

        if (walkingTapirModel != null)
            walkingTapirModel.SetActive(true);

        if (tapirScanMarker != null)
            tapirScanMarker.SetActive(false);

        if (trunkMarker != null)
            trunkMarker.SetActive(false);

        PlayIdle();

        state = MissionState.GuidingTapir;

        SetObjective("Tapir rescued. Guide it to the helicopter.");
    }

    private void GuideTapirToPlayer()
    {
        if (tapirRoot == null || playerTarget == null)
            return;

        if (helicopterDoorPoint != null)
        {
            float tapirToHeli = FlatDistance(tapirRoot.position, helicopterDoorPoint.position);

            if (tapirToHeli <= helicopterBoardDistance && !isBoarding)
            {
                StartCoroutine(BoardHelicopter());
                return;
            }
        }

        float distanceToPlayer = FlatDistance(tapirRoot.position, playerTarget.position);

        if (distanceToPlayer > stopDistanceFromPlayer)
        {
            MoveTapirTowards(playerTarget.position);
            PlayWalk();
        }
        else
        {
            PlayIdle();
        }
    }

    private IEnumerator BoardHelicopter()
    {
        isBoarding = true;
        state = MissionState.BoardingHelicopter;

        SetObjective("Tapir is entering the helicopter...");

        Transform target = helicopterInsidePoint != null ? helicopterInsidePoint : helicopterDoorPoint;

        while (target != null && FlatDistance(tapirRoot.position, target.position) > 0.2f)
        {
            MoveTapirTowards(target.position);
            PlayWalk();
            yield return null;
        }

        PlayIdle();

        CompleteMission();
    }

    private void MoveTapirTowards(Vector3 targetPosition)
    {
        Vector3 current = tapirRoot.position;
        Vector3 target = new Vector3(targetPosition.x, current.y, targetPosition.z);

        Vector3 direction = target - current;

        if (direction.magnitude < 0.05f)
            return;

        Vector3 move = direction.normalized * tapirMoveSpeed * Time.deltaTime;
        tapirRoot.position += move;

        Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
        tapirRoot.rotation = Quaternion.Slerp(
            tapirRoot.rotation,
            lookRotation,
            tapirTurnSpeed * Time.deltaTime
        );
    }

    private float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    private void PlayIdle()
    {
        if (tapirAnimator != null && !string.IsNullOrEmpty(idleAnimationState))
        {
            tapirAnimator.CrossFadeInFixedTime(idleAnimationState, 0.2f);
        }
    }

    private void PlayWalk()
    {
        if (tapirAnimator != null && !string.IsNullOrEmpty(walkAnimationState))
        {
            tapirAnimator.CrossFadeInFixedTime(walkAnimationState, 0.2f);
        }
    }

    private void CompleteMission()
    {
        state = MissionState.MissionComplete;

        SetObjective("Mission Complete. Tapir rescued successfully.");

        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(true);

        onMissionComplete.Invoke();

        StartCoroutine(EndMissionAfterDelay());
    }

    private IEnumerator EndMissionAfterDelay()
    {
        yield return new WaitForSeconds(endDelay);

        if (loadNextSceneDirectly && !string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void SetObjective(string message)
    {
        if (objectiveText != null)
            objectiveText.text = message;

        Debug.Log("[Tapir Mission] " + message);
    }

    private void FreeTapirForTesting()
    {
        FreeTapir();
    }
}