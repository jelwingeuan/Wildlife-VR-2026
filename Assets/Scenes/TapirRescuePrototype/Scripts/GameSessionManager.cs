using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance;

    [Header("Mission Progress")]
    public bool tapirMissionSelected = false;
    public bool tapirFound = false;
    public bool fallenTreeRemoved = false;
    public bool tapirRescuedToNursery = false;
    public bool tapirFed = false;
    public bool habitatRestored = false;
    public bool fireCleared = false;
    public bool tapirReleased = false;

    [Header("Current Mission")]
    public string currentMissionName = "None";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SelectTapirMission()
    {
        tapirMissionSelected = true;
        currentMissionName = "Tapir Rescue Mission";
        Debug.Log("Tapir mission selected.");
    }

    public void MarkTapirRescued()
    {
        tapirRescuedToNursery = true;
        Debug.Log("Tapir rescued to nursery.");
    }
}