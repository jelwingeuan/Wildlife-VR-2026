using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SettingsPopupMenu : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField] private GameObject settingsPopup;

    private void Awake()
    {
        if (settingsPopup != null)
        {
            settingsPopup.SetActive(false);
        }
    }

    public void OpenSettings()
    {
        if (settingsPopup != null)
        {
            settingsPopup.SetActive(true);
        }
    }

    public void ContinueGame()
    {
        if (settingsPopup != null)
        {
            settingsPopup.SetActive(false);
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        // Stops Play Mode while testing inside Unity.
        EditorApplication.isPlaying = false;
#else
        // Closes the built application.
        Application.Quit();
#endif
    }
}