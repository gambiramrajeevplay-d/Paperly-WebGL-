using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Panels")]
    public GameObject levelPassPanel;
    public GameObject levelFailPanel;

    [Header("PASS UI")]
    public TextMeshProUGUI passCoinsText;
    public TextMeshProUGUI passTimeCollectedText;
    public TextMeshProUGUI passFlightTimeText;

    [Header("FAIL UI")]
    public TextMeshProUGUI failCoinsText;
    public TextMeshProUGUI failTimeCollectedText;
    public TextMeshProUGUI failFlightTimeText;

    private TimeManager timeManager;
    private bool levelEnded = false;

    [Header("Fail Audio")]
    public AudioClip levelFailSound;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;

    }

    // =========================
    // LEVEL PASS
    // =========================
    public void ShowLevelPass()
    {
        if (levelEnded) return;
        levelEnded = true;

        AddCollectedCoins();

        UpdatePassUI();

        if (levelPassPanel != null)
            levelPassPanel.SetActive(true);
    }

    // =========================
    // LEVEL FAIL
    // =========================
    public void ShowLevelFail()
    {
        if (levelEnded) return;
        levelEnded = true;

        // 🔊 FAIL SOUND
        if (levelFailSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(levelFailSound);
        }

        AddCollectedCoins();
        UpdateFailUI();

        if (levelFailPanel != null)
            levelFailPanel.SetActive(true);
    }


    // =========================
    // COIN HANDLING (SHARED)
    // =========================
    private void AddCollectedCoins()
    {
        if (CurrencyManager.instance != null && LevelCoinManager.instance != null)
        {
            int collectedCoins = LevelCoinManager.instance.GetCollectedCoins();
            CurrencyManager.instance.AddCurrency(collectedCoins);
        }
    }

    // =========================
    // UI UPDATES
    // =========================
    private void UpdatePassUI()
    {
        int totalCoins = CurrencyManager.instance != null
            ? CurrencyManager.instance.GetCurrency()
            : 0;

        float bonusTime = timeManager != null
            ? timeManager.GetBonusTimeCollected()
            : 0f;

        float flightTime = timeManager != null
            ? timeManager.GetFlightTime()
            : 0f;

        if (passCoinsText)
            passCoinsText.text = totalCoins.ToString();

        if (passTimeCollectedText)
            passTimeCollectedText.text = bonusTime.ToString("0.0") + "s";

        if (passFlightTimeText)
            passFlightTimeText.text = flightTime.ToString("0.0") + "s";
    }

    private void UpdateFailUI()
    {
        int totalCoins = CurrencyManager.instance != null
            ? CurrencyManager.instance.GetCurrency()
            : 0;

        float bonusTime = timeManager != null
            ? timeManager.GetBonusTimeCollected()
            : 0f;

        float flightTime = timeManager != null
            ? timeManager.GetFlightTime()
            : 0f;

        if (failCoinsText)
            failCoinsText.text = totalCoins.ToString();

        if (failTimeCollectedText)
            failTimeCollectedText.text = bonusTime.ToString("0.0") + "s";

        if (failFlightTimeText)
            failFlightTimeText.text = flightTime.ToString("0.0") + "s";
    }

    // =========================
    // BUTTONS
    // =========================
    public void Home()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("UI");
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
