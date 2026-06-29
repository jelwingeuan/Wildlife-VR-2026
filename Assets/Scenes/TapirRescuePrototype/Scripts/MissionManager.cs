using UnityEngine;
using UnityEngine.InputSystem;

public class MissionManager : MonoBehaviour
{
    public enum MissionState
    {
        Start,
        DroneDeployed,
        AnimalFound,
        TreeRemoved,
        BeaconDeployed,
        RescueComplete
    }

    [Header("Mission State")]
    public MissionState currentState = MissionState.Start;

    [Header("Scene References")]
    public Transform injuredTapir;
    public Transform fallenTree;
    public Transform searchDroneSpawnPoint;
    public Transform rescueBeaconSpawnPoint;
    public Transform rescueCarrierSpawnPoint;
    public Transform nurseryPoint;

    [Header("Prefabs")]
    public GameObject searchDronePrefab;
    public GameObject rescueBeaconPrefab;
    public GameObject rescueCarrierPrefab;

    [Header("Settings")]
    public float treeClearDistance = 3f;
    public float keyboardTreeMoveDistance = 4f;

    private string objectiveText;
    private Renderer tapirRenderer;
    private bool treeAlreadyCleared = false;

    void Start()
    {
        SetObjective("Objective: Deploy Search Drone. Press 1.");

        if (injuredTapir != null)
        {
            tapirRenderer = injuredTapir.GetComponent<Renderer>();
        }
    }

    void Update()
    {
        // New Input System keyboard testing
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            DeploySearchDrone();
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            TestRemoveFallenTree();
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            DeployRescueBeacon();
        }

        CheckTreeDistance();
    }

    public void DeploySearchDrone()
    {
        if (currentState != MissionState.Start)
        {
            Debug.Log("Cannot deploy search drone now.");
            return;
        }

        if (searchDronePrefab == null || searchDroneSpawnPoint == null || injuredTapir == null)
        {
            Debug.LogError("Search Drone setup is missing. Check prefab, spawn point, or injured tapir.");
            return;
        }

        currentState = MissionState.DroneDeployed;
        SetObjective("Search Drone deployed. Searching for injured tapir...");

        GameObject drone = Instantiate(
            searchDronePrefab,
            searchDroneSpawnPoint.position,
            searchDroneSpawnPoint.rotation
        );

        SearchDrone droneScript = drone.AddComponent<SearchDrone>();
        droneScript.Setup(injuredTapir, this);
    }

    public void AnimalFound()
    {
        if (currentState != MissionState.DroneDeployed)
            return;

        currentState = MissionState.AnimalFound;

        HighlightTapir();

        SetObjective("Tapir found. Drag the fallen tree away with mouse, or press E for quick test.");
    }

    private void TestRemoveFallenTree()
    {
        if (currentState != MissionState.AnimalFound)
        {
            Debug.Log("You must find the tapir first.");
            return;
        }

        if (fallenTree == null)
        {
            Debug.LogError("Fallen tree is missing. Assign FallenTree_BlockingTapir in MissionManager.");
            return;
        }

        fallenTree.position += Vector3.right * keyboardTreeMoveDistance;
        Debug.Log("Fallen tree moved away for keyboard test.");
    }

    private void CheckTreeDistance()
    {
        if (currentState != MissionState.AnimalFound)
            return;

        if (treeAlreadyCleared)
            return;

        if (fallenTree == null || injuredTapir == null)
            return;

        float distance = Vector3.Distance(fallenTree.position, injuredTapir.position);

        if (distance >= treeClearDistance)
        {
            treeAlreadyCleared = true;
            currentState = MissionState.TreeRemoved;
            SetObjective("Path cleared. Deploy Rescue Beacon. Press 2.");
        }
    }

    public void DeployRescueBeacon()
    {
        if (currentState != MissionState.TreeRemoved)
        {
            Debug.Log("Cannot deploy rescue beacon yet.");
            return;
        }

        if (rescueBeaconPrefab == null || rescueBeaconSpawnPoint == null)
        {
            Debug.LogError("Rescue Beacon setup is missing. Check prefab or spawn point.");
            return;
        }

        if (rescueCarrierPrefab == null || rescueCarrierSpawnPoint == null || nurseryPoint == null)
        {
            Debug.LogError("Rescue Carrier setup is missing. Check prefab, spawn point, or nursery point.");
            return;
        }

        currentState = MissionState.BeaconDeployed;
        SetObjective("Rescue Beacon deployed. Carrier drone incoming...");

        Instantiate(
            rescueBeaconPrefab,
            rescueBeaconSpawnPoint.position,
            rescueBeaconSpawnPoint.rotation
        );

        GameObject carrier = Instantiate(
            rescueCarrierPrefab,
            rescueCarrierSpawnPoint.position,
            rescueCarrierSpawnPoint.rotation
        );

        RescueCarrier carrierScript = carrier.AddComponent<RescueCarrier>();
        carrierScript.Setup(injuredTapir, nurseryPoint, this);
    }

    public void RescueFinished()
    {
        currentState = MissionState.RescueComplete;
        SetObjective("Mission Complete: Tapir sent to Nursery Hub.");
    }

    private void HighlightTapir()
    {
        if (tapirRenderer != null)
        {
            tapirRenderer.material.color = Color.green;
        }
    }

    private void SetObjective(string message)
    {
        objectiveText = message;
        Debug.Log(message);
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.white;

        GUI.Box(new Rect(20, 20, 820, 130), "");
        GUI.Label(new Rect(40, 40, 780, 40), objectiveText, style);
        GUI.Label(new Rect(40, 80, 780, 40), "Controls: 1 = Search Drone | Drag Tree / E = Remove Tree | 2 = Rescue Beacon", style);
    }
}