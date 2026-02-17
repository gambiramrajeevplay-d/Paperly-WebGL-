using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Level_Button : MonoBehaviour
{
    [Header("Level Details")]
    [SerializeField] private string sceneNameToLoad = "MainGame";
    [SerializeField] private int levelToLoad = 1; // 1-based

    [Header("Visuals")]
    [SerializeField] private GameObject lockImage;

    [Header("UI")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject levelSelectionPanel;

    private Button levelButton;

    private void Awake()
    {
        EnsureInitialized();
        levelButton.interactable = false;
    }

    private void Start()
    {
        EnsureInitialized();
        levelButton.onClick.RemoveAllListeners();
        levelButton.onClick.AddListener(LoadLevel);
    }

    private void EnsureInitialized()
    {
        if (levelButton == null)
            levelButton = GetComponent<Button>();
    }

    // 🔑 CALLED FROM LevelSelectionUI
    public void SetUnlocked(bool unlocked)
    {
        EnsureInitialized();
        levelButton.onClick.RemoveAllListeners();

        // LEVEL 1 → ALWAYS UNLOCKED
        if (levelToLoad == 1)
        {
            levelButton.interactable = true;
            if (lockImage) lockImage.SetActive(false);
            levelButton.onClick.AddListener(LoadLevel);
            return;
        }

        levelButton.interactable = unlocked;

        if (lockImage)
            lockImage.SetActive(!unlocked);

        if (unlocked)
            levelButton.onClick.AddListener(LoadLevel);
    }

    public void UnLockLevel()
    {
        int playerLevel = PlayerPrefs.GetInt(StringsData.playerLevel, 1);
        bool unlocked = playerLevel >= levelToLoad;
        SetUnlocked(unlocked);
    }

    private void LoadLevel()
    {
        // Save selected level
        PlayerPrefs.SetInt(StringsData.levelToLoad, levelToLoad);
        PlayerPrefs.Save();

        // Disable button to prevent double click
        levelButton.interactable = false;

        // Hide level selection
        if (levelSelectionPanel != null)
            levelSelectionPanel.SetActive(false);

        // Show loading screen
        if (loadingScreen != null)
            loadingScreen.SetActive(true);
        else
        {
            Debug.LogError("LoadingScreen is NOT assigned!");
            return;
        }

        // ✅ Delegate scene loading to LoadingManager
        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.LoadSceneWithDelay(sceneNameToLoad, 5f);
        }
        else
        {
            Debug.LogError("LoadingManager instance NOT found!");
        }
    }

    public int GetLevelNumberSafe()
    {
        return Mathf.Max(1, levelToLoad);
    }
}
