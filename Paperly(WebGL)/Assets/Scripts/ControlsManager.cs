using UnityEngine;

public class ControlsManager : MonoBehaviour
{
    public GameObject controlsPanel; // single tab controls panel

    private const string FIRST_TIME_KEY = "FirstTimeStartShown";

    private void Start()
    {
        int currentLevel = PlayerPrefs.GetInt(StringsData.levelToLoad, 1);
        bool isFirstTime = PlayerPrefs.GetInt(FIRST_TIME_KEY, 0) == 0;

        // Only show on FIRST LEVEL & FIRST TIME
        if (currentLevel != 1 || !isFirstTime)
            return;

        ShowControls();
    }

    private void ShowControls()
    {
        if (controlsPanel == null) return;

        controlsPanel.SetActive(true);

        // ⏸ Pause game completely
        Time.timeScale = 0f;

        // Lock pause button if you use Pauser
        Pauser.LockPause();

        // Mark as shown (so it never shows again)
        PlayerPrefs.SetInt(FIRST_TIME_KEY, 1);
        PlayerPrefs.Save();
    }

    // 🔘 Hook this to OK button
    public void OnOkButtonPressed()
    {
        if (controlsPanel == null) return;

        controlsPanel.SetActive(false);

        // ▶️ Resume game
        Time.timeScale = 1f;

        // Unlock pause system
        Pauser.UnlockPause();
    }
}
