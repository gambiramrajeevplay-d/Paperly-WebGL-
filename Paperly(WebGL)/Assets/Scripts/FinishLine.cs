using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour
{
    public GameObject levelPassPanel;
    public GameObject levelGameObject;

    private bool levelCompleted = false;

    [Header("Level Pass Audio")]
    public AudioClip levelPassSound;
    private AudioSource audioSource;

    void Start()
    {
        // 🔊 Get AudioSource from object tagged "PickUp"
        GameObject audioObj = GameObject.FindGameObjectWithTag("PickUp");

        if (audioObj != null)
        {
            audioSource = audioObj.GetComponent<AudioSource>();

            if (audioSource == null)
            {
                audioSource = audioObj.AddComponent<AudioSource>();
            }

            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f; // UI / 2D sound
        }
        else
        {
            Debug.LogWarning("No GameObject found with tag 'PickUp' for audio source");
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (levelCompleted)
            return;

        if (!other.CompareTag("Player"))
            return;

        levelCompleted = true;

        PlaneController plane = other.GetComponent<PlaneController>();

        if (plane != null)
        {
            plane.canControl = false;
            plane.isRewinding = false;

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // 🔓 Unlock next level
        UnlockNextLevel();

        // Disable level objects
        if (levelGameObject != null)
            levelGameObject.SetActive(false);

        // Pause time
        Time.timeScale = 0f;

        // ⏳ Delay level pass UI
        StartCoroutine(ShowLevelPassWithDelay());
    }

    private IEnumerator ShowLevelPassWithDelay()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        // 🔊 Play level pass sound
        if (levelPassSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(levelPassSound);
        }

        GameManager.Instance.ShowLevelPass();
    }

    private void UnlockNextLevel()
    {
        int currentLevel = PlayerPrefs.GetInt(StringsData.levelToLoad, 1);
        int unlockedLevel = PlayerPrefs.GetInt(StringsData.playerLevel, 1);

        if (currentLevel >= unlockedLevel)
        {
            PlayerPrefs.SetInt(StringsData.playerLevel, currentLevel + 1);
            PlayerPrefs.Save();
        }
    }
}
