using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pauser : MonoBehaviour
{
    public static Pauser instance;

    [Header("UI")]
    public GameObject PausePannel;
    public GameObject LevelObject;
    public GameObject PauseButton;
   // public GameObject levelFailPanel;

    [Header("Settings")]
    public GameObject SettingsPanel;

    [Header("Sound")]
    public Image soundImage;
    public Sprite sound_on;
    public Sprite sound_off;

    // 🔒 GLOBAL PAUSE LOCK
    public static bool PauseLocked = false;

    private void Awake()
    {
        instance = this;
        AudioManagerPause.Initialize();
        UpdateSoundSprite(); // sync on start
    }

    void Update()
    {
        // 🔒 HARD BLOCK pause when locked
        if (PauseLocked)
            return;

        // ❌ Block pause if level ended
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.levelPassPanel.activeSelf ||
                GameManager.Instance.levelFailPanel.activeSelf)
                return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    // =========================
    // PAUSE
    // =========================
    public void Pause()
    {
        if (PauseLocked)
            return;

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.levelPassPanel.activeSelf ||
                GameManager.Instance.levelFailPanel.activeSelf)
                return;
        }

        LevelObject.SetActive(false);
        PausePannel.SetActive(true);
        PauseButton.SetActive(false);

        Time.timeScale = 0f;
        UpdateSoundSprite();
    }

    public void Resume()
    {
        LevelObject.SetActive(true);
        PausePannel.SetActive(false);
        PauseButton.SetActive(true);

        Time.timeScale = 1f;
    }

    // =========================
    // SETTINGS
    // =========================
    public void OpenSettings()
    {
        if (SettingsPanel != null)
            SettingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (SettingsPanel != null)
            SettingsPanel.SetActive(false);
    }

    // =========================
    // SOUND (SAME AS MAIN MENU)
    // =========================
    public void ToggleSound()
    {
        if (soundImage == null) return;

        if (soundImage.sprite == sound_on)
        {
            soundImage.sprite = sound_off;
            AudioListener.pause = true;
            AudioManagerPause.IsMuted = true;
        }
        else
        {
            soundImage.sprite = sound_on;
            AudioListener.pause = false;
            AudioManagerPause.IsMuted = false;
        }
    }

    private void UpdateSoundSprite()
    {
        if (soundImage == null) return;

        if (AudioListener.pause || AudioManagerPause.IsMuted)
            soundImage.sprite = sound_off;
        else
            soundImage.sprite = sound_on;
    }

    // =========================
    // MAIN MENU
    // =========================
    public void MM()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("UI");
    }

    // =========================
    // PAUSE LOCK SYSTEM
    // =========================
    public static void LockPause()
    {
        PauseLocked = true;

        if (instance != null)
        {
            instance.PausePannel.SetActive(false);
            instance.LevelObject.SetActive(true);
        }
    }

    public static void UnlockPause()
    {
        PauseLocked = false;
    }
}
