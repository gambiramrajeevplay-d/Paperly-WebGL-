using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectionUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button closeButton;

    [Header("Levels")]
    [SerializeField] private Level_Button[] levels;

    [Header("Menus")]
    [SerializeField] private GameObject mainMenuPanel;

    //[Header("Currency")]
    //[SerializeField] private TextMeshProUGUI currencyText;

    [Header("Debug / Testing")]
    [SerializeField] private bool forceUnlockAllLevels = false;

    private bool isInitialized = false;

    private void OnEnable()
    {
        if (!isInitialized)
            return;

        UnlockLevels();
     //   UpdateCurrencyText();
    }

    private void Start()
    {
        //UpdateCurrencyText();

        // Ensure player level exists
        if (!PlayerPrefs.HasKey(StringsData.playerLevel))
        {
            PlayerPrefs.SetInt(StringsData.playerLevel, 1);
            PlayerPrefs.Save();
        }

        if (closeButton != null)
            closeButton.onClick.AddListener(BackToMainMenu);
        else
            Debug.LogWarning("[LevelSelectionUI] closeButton not assigned.");

        isInitialized = true;
        UnlockLevels();
    }

    // =====================
    // LEVEL UNLOCK LOGIC
    // =====================
    public void UnlockLevels()
    {
        int playerLevel = PlayerPrefs.GetInt(StringsData.playerLevel, 1);

        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] == null)
                continue;

            int levelNumber = i + 1; // index → level number
            bool unlocked = levelNumber <= playerLevel;

            levels[i].SetUnlocked(unlocked);
        }
    }


    // =====================
    // UI
    // =====================
    //void UpdateCurrencyText()
    //{
    //    if (currencyText != null && CurrencyManager.instance != null)
    //        currencyText.text = CurrencyManager.instance.GetCurrency().ToString();
    //}

    // =====================
    // BACK BUTTON
    // =====================
    public void BackToMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        else
            Debug.LogWarning("[LevelSelectionUI] mainMenuPanel not assigned.");

        gameObject.SetActive(false);
    }
}
