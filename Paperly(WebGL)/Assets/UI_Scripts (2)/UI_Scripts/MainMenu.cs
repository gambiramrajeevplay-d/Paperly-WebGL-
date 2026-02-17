using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button storeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button storeCloseButton;
    [SerializeField] private Button settingsCloseButton;

    [Header("Menus")]
    [SerializeField] private GameObject storeMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelSelection;
    

    //[Header("Currency")]
    //[SerializeField] private TextMeshProUGUI currencyText;

    [Header("Sound")]
    public Image sound_;
    public Sprite sound_on, sound_off;

    private void Awake()
    {
        Time.timeScale = 1f;

        // 🔊 Sync audio state when entering main menu
        AudioManagerPause.Initialize();
    }

    private void Start()
    {
        // Buttons
        startButton.onClick.AddListener(OnStartButtonPressed);
        storeButton.onClick.AddListener(OpenStoreMenu);
        settingsButton.onClick.AddListener(OpenSettingsMenu);

        storeCloseButton.onClick.AddListener(OpenMainMenu);
        settingsCloseButton.onClick.AddListener(OnSettingClose);

        UpdateCurrencyText();
        UpdateSoundSprite(); // 🔊 sync icon on start

        mainMenu.SetActive(true);
        storeMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }


    // =====================
    // MAIN FLOW
    // =====================
    public void OnStartButtonPressed()
    {
        // Load your gameplay scene here
         mainMenu.SetActive(false);
         levelSelection.SetActive(true);
    }
    private void UpdateSoundSprite()
    {
        if (sound_ == null) return;

        if (AudioListener.pause || AudioManagerPause.IsMuted)
            sound_.sprite = sound_off;
        else
            sound_.sprite = sound_on;
    }

    public void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        storeMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    // =====================
    // STORE / SETTINGS
    // =====================
    public void OpenStoreMenu()
    {
        storeMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void OpenSettingsMenu()
    {
        settingsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void OnSettingClose()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
    public void OnStoreClose()
    {
        storeMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    // =====================
    // CURRENCY
    // =====================
    public void UpdateCurrencyText()
    {
        //if (currencyText != null && CurrencyManager.instance != null)
        //    currencyText.text = CurrencyManager.instance.GetCurrency().ToString();
    }

    // =====================
    // SOUND
    // =====================
    public void Sound_on()
    {
        if (sound_.sprite == sound_on)
        {
            sound_.sprite = sound_off;
            AudioListener.pause = true;
        }
        else
        {
            sound_.sprite = sound_on;
            AudioListener.pause = false;
        }

        AudioManagerPause.IsMuted = !AudioManagerPause.IsMuted;
        UpdateSoundSprite();
    }
}
