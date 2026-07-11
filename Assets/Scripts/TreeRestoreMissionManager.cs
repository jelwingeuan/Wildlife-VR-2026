using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TreeRestoreMissionManager : MonoBehaviour
{
    [Header("Scene Transition")]
    [Tooltip("Exact name of the next scene, without .unity")]
    [SerializeField] private string nextSceneName = "06_Release";

    [Tooltip("How long Mission Complete stays visible")]
    [SerializeField, Min(0f)] private float transitionDelay = 3f;

    [Header("Mission Complete UI")]
    [Tooltip("Optional Mission Complete panel")]
    [SerializeField] private GameObject missionCompleteUI;

    private TreeTarget[] requiredTrees;
    private readonly HashSet<TreeTarget> completedTrees =
        new HashSet<TreeTarget>();

    private bool missionIsCompleting;

    private void Awake()
    {
        // Automatically find every TreeTarget below Tree Restore.
        requiredTrees = GetComponentsInChildren<TreeTarget>(true);

        if (missionCompleteUI != null)
        {
            missionCompleteUI.SetActive(false);
        }

        if (requiredTrees.Length == 0)
        {
            Debug.LogError(
                "Tree Restore Mission Manager could not find any TreeTarget scripts.",
                this
            );
        }
        else
        {
            Debug.Log(
                "Tree restoration mission requires " +
                requiredTrees.Length +
                " trees.",
                this
            );
        }
    }

    public void RegisterTreeCompleted(TreeTarget completedTree)
    {
        if (missionIsCompleting || completedTree == null)
        {
            return;
        }

        // Make sure this tree belongs to this mission.
        bool isRequiredTree = false;

        foreach (TreeTarget requiredTree in requiredTrees)
        {
            if (requiredTree == completedTree)
            {
                isRequiredTree = true;
                break;
            }
        }

        if (!isRequiredTree)
        {
            Debug.LogWarning(
                completedTree.name +
                " reported completion but is not part of this mission.",
                completedTree
            );

            return;
        }

        // HashSet prevents the same tree from being counted twice.
        if (!completedTrees.Add(completedTree))
        {
            return;
        }

        Debug.Log(
            "Tree completed: " +
            completedTrees.Count +
            " / " +
            requiredTrees.Length,
            completedTree
        );

        if (completedTrees.Count >= requiredTrees.Length)
        {
            StartCoroutine(CompleteMission());
        }
    }

    private IEnumerator CompleteMission()
    {
        missionIsCompleting = true;

        Debug.Log("ALL TREES RESTORED — MISSION COMPLETE");

        if (missionCompleteUI != null)
        {
            missionCompleteUI.SetActive(true);
        }

        // Realtime still works even if Time.timeScale becomes 0.
        yield return new WaitForSecondsRealtime(transitionDelay);

        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogError(
                "Next Scene Name has not been entered.",
                this
            );

            yield break;
        }

        if (!Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            Debug.LogError(
                "Scene '" + nextSceneName +
                "' cannot be loaded. Check the spelling and Build Profile Scene List.",
                this
            );

            yield break;
        }

        AsyncOperation sceneLoading =
            SceneManager.LoadSceneAsync(
                nextSceneName,
                LoadSceneMode.Single
            );

        if (sceneLoading == null)
        {
            Debug.LogError(
                "Unity failed to start loading scene: " +
                nextSceneName,
                this
            );

            yield break;
        }

        yield return sceneLoading;
    }
}
