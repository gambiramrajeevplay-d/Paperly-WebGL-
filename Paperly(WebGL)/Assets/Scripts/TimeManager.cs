using UnityEngine;
using TMPro;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    public float startTime = 60f;
    private float currentTime;

    [Header("UI")]
    public TextMeshProUGUI timeText;
    public GameObject levelFailPanel;

    [Header("References")]
    private PlaneController playerPlane;

    private bool isGameOver = false;

    [Header("Fail Audio")]
    public AudioClip failClip;
    public AudioSource uiAudioSource;

    public GameObject levelGameObj;

    private float bonusTimeCollected = 0f; // total picked time
    private float flightTime = 0f;         // plane alive time


    void Start()
    {
        currentTime = startTime;

        // Player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerPlane = playerObj.GetComponent<PlaneController>();

        // UI AudioSource (recommended on Canvas)
        if (uiAudioSource == null)
            uiAudioSource = GetComponent<AudioSource>();

        UpdateUI();
    }


    void Update()
    {
        if (isGameOver)
            return;

        currentTime -= Time.deltaTime;

        // ✈ Track how long the plane exists
        flightTime += Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            GameOver();
        }

        UpdateUI();
    }


    void UpdateUI()
    {
        if (timeText != null)
        {
            int totalSeconds = Mathf.CeilToInt(currentTime);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            timeText.text = "Time Left : " + minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }
    public void GameOverFromCrash()
    {
        if (isGameOver) return;

        isGameOver = true;

        StartCoroutine(GameOverDelayRoutine());
    }

    IEnumerator GameOverDelayRoutine()
    {
        // Stop player control immediately
        if (playerPlane != null)
        {
            playerPlane.canControl = false;

            Rigidbody rb = playerPlane.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // 🔊 Play fail sound
        if (uiAudioSource != null && failClip != null)
        {
            uiAudioSource.PlayOneShot(failClip);
        }

        // ⏳ Wait before showing fail
        yield return new WaitForSeconds(3f);

        // ❌ Turn OFF level gameplay
        if (levelGameObj != null)
            levelGameObj.SetActive(false);

        // ✅ Show fail UI
        //if (levelFailPanel != null)
        //    levelFailPanel.SetActive(true);
        GameManager.Instance.ShowLevelFail();

        Time.timeScale = 0f;
    }

    public float GetBonusTimeCollected()
    {
        return bonusTimeCollected;
    }

    public float GetFlightTime()
    {
        return flightTime;
    }


    void GameOver()
    {
        isGameOver = true;

        if (playerPlane != null)
        {
            playerPlane.canControl = false;

            Rigidbody rb = playerPlane.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // ❌ Disable gameplay
        if (levelGameObj != null)
            levelGameObj.SetActive(false);

        // ✅ Show fail UI
        if (levelFailPanel != null)
            levelFailPanel.SetActive(true);

        Time.timeScale = 0f;
    }


    // 🔥 Called when collecting time bonus
    public void AddTime(float seconds)
    {
        currentTime += seconds;
    }
}
