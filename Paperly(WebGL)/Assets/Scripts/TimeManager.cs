using UnityEngine;
using TMPro;

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

    void Start()
    {
        currentTime = startTime;

        // Get player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerPlane = playerObj.GetComponent<PlaneController>();
        }

        UpdateUI();
    }

    void Update()
    {
        if (isGameOver)
            return;

        currentTime -= Time.deltaTime;

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



    void GameOver()
    {
        isGameOver = true;

        // Stop player control
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

        // Show fail panel
        if (levelFailPanel != null)
            levelFailPanel.SetActive(true);

        // Optional pause
        Time.timeScale = 0f;
    }


    // 🔥 Called when collecting time bonus
    public void AddTime(float seconds)
    {
        currentTime += seconds;
    }
}
