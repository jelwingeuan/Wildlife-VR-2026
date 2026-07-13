using System.Collections;
using UnityEngine;

public class WalkThroughUIPopup : MonoBehaviour
{
    [Header("UI To Show")]
    public GameObject popupUI;

    [Header("Settings")]
    public float displayDuration = 10f;
    public bool showOnlyOnce = true;

    private bool hasTriggered = false;
    private Coroutine hideCoroutine;

    private void Start()
    {
        if (popupUI != null)
        {
            popupUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (showOnlyOnce && hasTriggered)
            return;

        bool isPlayer =
            other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player");

        if (!isPlayer)
            return;

        if (popupUI != null)
        {
            popupUI.SetActive(true);
            hasTriggered = true;

            // Restart the timer if needed
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
            }

            hideCoroutine = StartCoroutine(HideAfterDelay());
        }
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        if (popupUI != null)
        {
            popupUI.SetActive(false);
        }

        hideCoroutine = null;
    }
}