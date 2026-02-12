
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

    [Header("UnlockFullGameUI")]
    [SerializeField] private GameObject unlockFullGamePanel;
    [SerializeField] private Button unlockFullGamePanelCloseButton;

    [Header("Menus")]
    [SerializeField] private GameObject horseSelectionMenu;

    private Animator animator;
   // private ButtonHighlighter buttonHighlighter;
    private Button levelButtonToHighlight;

    private bool isInitialized = false;

    [Header("Debug / Testing")]
    [SerializeField] private bool forceUnlockAllLevels = false;

    [Header("Currency")]
    [SerializeField] private TextMeshProUGUI currencyText;



    private void OnEnable()
    {
        if (!isInitialized)
            return; // ⛔ prevent first-open bug

        //if (animator != null)
        //    animator.SetTrigger("Entry");

        UnlockLevels(); // ✅ highlight on reopen
        UpdateCurrencyText();
    }



    private void Start()
    {
        UpdateCurrencyText();



        Debug.Log("[LevelSelectionUI][Start] Initializing LevelSelectionUI.");

        // ✅ Ensure player level exists
        if (!PlayerPrefs.HasKey(StringsData.playerLevel))
        {
            PlayerPrefs.SetInt(StringsData.playerLevel, 1);
            PlayerPrefs.Save();
            Debug.Log("[LevelSelectionUI] Player level initialized to 1.");
        }

        if (closeButton != null)
            closeButton.onClick.AddListener(GoToHorseSelection);
        else
            Debug.LogWarning("[LevelSelectionUI] closeButton is not assigned.");

        //buttonHighlighter = GetComponent<ButtonHighlighter>();
       // if (buttonHighlighter == null)
            Debug.LogWarning("[LevelSelectionUI] ButtonHighlighter not found on the same GameObject.");

        isInitialized = true;

        UnlockLevels(); // refresh when enabled
    }

    public void UnlockLevels()
    {
        //if (buttonHighlighter == null)
         //   buttonHighlighter = GetComponent<ButtonHighlighter>();

        int playerLevel = PlayerPrefs.GetInt(StringsData.playerLevel, 1);

        bool fullGameUnlocked =
            forceUnlockAllLevels ||
            PlayerPrefs.GetInt(StringsData.unlockedAllLevels, 0) == 1;

        Button highestButton = null;

        // 1️⃣ LOCK / UNLOCK LEVELS
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] == null)
                continue;

            // 🚨 RULE: LEVEL 1 (INDEX 0) IS ALWAYS FREE
            bool shouldUnlock = (i == 0) || fullGameUnlocked || (i < playerLevel);

            levels[i].SetUnlocked(shouldUnlock);

            // ✅ ALWAYS STORE LAST UNLOCKED BUTTON
            if (shouldUnlock)
            {
                highestButton = levels[i].GetComponent<Button>();
            }
        }

        // 2️⃣ SAFETY: FIRST LEVEL MUST ALWAYS BE INTERACTABLE
        if (levels.Length > 0 && levels[0] != null)
        {
            Button firstButton = levels[0].GetComponent<Button>();
            if (firstButton != null)
                firstButton.interactable = true;
        }

        // 3️⃣ HIGHLIGHT HIGHEST (LAST) UNLOCKED LEVEL
       // if (highestButton != null && buttonHighlighter != null && EventSystem.current != null)
        {
            highestButton.interactable = true;

         //   buttonHighlighter.defaultButton = highestButton.gameObject;
         //   buttonHighlighter.enabled = true;

            EventSystem.current.SetSelectedGameObject(highestButton.gameObject);
         //   buttonHighlighter.HighlightButton(highestButton);
        }
    }


    void UpdateCurrencyText()
    {
        if (currencyText != null && CurrencyManager.instance != null)
        {
            currencyText.text = CurrencyManager.instance.GetCurrency().ToString();
        }
    }


    public void HighlightButton(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogWarning($"[LevelSelectionUI] Invalid levelIndex {levelIndex}");
            return;
        }

        levelButtonToHighlight = levels[levelIndex].GetComponent<Button>();

        if (levelButtonToHighlight == null)
        {
            Debug.LogWarning($"[LevelSelectionUI] No Button component found on level {levelIndex}");
            return;
        }

        if (!levelButtonToHighlight.interactable)
        {
            Debug.LogWarning($"[LevelSelectionUI] Cannot highlight level {levelIndex} - button not interactable");
            return;
        }

       // if (buttonHighlighter == null)
      //  {
       //     Debug.LogWarning("[LevelSelectionUI] ButtonHighlighter is null");
      //     return;
      //  }

      //  buttonHighlighter.defaultButton = levels[levelIndex].gameObject;
       // buttonHighlighter.enabled = true;
      //  buttonHighlighter.HighlightButton(levelButtonToHighlight);

        Debug.Log($"[LevelSelectionUI] Successfully highlighted level {levelIndex}");
    }

    public void GoToHorseSelection()
    {
        Debug.Log("LevelSelectionUI.GoToHorseSelection");

        if (horseSelectionMenu != null)
            horseSelectionMenu.SetActive(true);
        else
            Debug.LogWarning("horseSelectionMenu not assigned.");

        gameObject.SetActive(false);
    }

    public void ShowUnlockAllPanel()
    {
        Debug.Log("ShowUnlockAllPanel");

        if (unlockFullGamePanel != null)
        {
            unlockFullGamePanel.SetActive(true);
            gameObject.SetActive(false);
        }
        else
            Debug.LogWarning("unlockFullGamePanel not assigned.");
    }

    public void CloseUnlockFullGamePanel()
    {
        if (unlockFullGamePanel != null)
        {
            unlockFullGamePanel.SetActive(false); // 🔴 turn OFF unlock panel
        }

        gameObject.SetActive(true); // 🔵 turn ON level selection

        UnlockLevels(); // refresh & highlight again
    }

    // 🏆 FULL GAME PURCHASE
    public void OnFullGameUnlocked()
    {
        Debug.Log("[LevelSelectionUI] Full game unlocked!");

        PlayerPrefs.SetInt(StringsData.unlockedAllLevels, 1);
        PlayerPrefs.SetInt(StringsData.playerLevel, levels.Length);
        PlayerPrefs.Save();

        UnlockLevels();
    }
}
