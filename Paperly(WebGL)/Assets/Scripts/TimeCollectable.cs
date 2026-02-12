using TMPro;
using UnityEngine;

public class TimeCollectable : MonoBehaviour
{
    public float timeToAdd = 2f;
    public TextMeshProUGUI floatingText;
    public AudioClip pickupClip;   // Assign in Inspector

    private TimeManager timeManager;
    private AudioSource pickupSource;

    void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();

        // Find AudioSource with tag "PickUp"
        GameObject audioObj = GameObject.FindGameObjectWithTag("PickUp");
        if (audioObj != null)
        {
            pickupSource = audioObj.GetComponent<AudioSource>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Add time
        if (timeManager != null)
        {
            timeManager.AddTime(timeToAdd);
        }

        // Play pickup sound
        if (pickupSource != null && pickupClip != null)
        {
            pickupSource.PlayOneShot(pickupClip);
        }

        // Show floating text
        if (floatingText != null)
        {
            floatingText.text = "+" + timeToAdd.ToString("0");
            floatingText.transform.position = transform.position + Vector3.up * 1f;
            floatingText.gameObject.SetActive(true);
            floatingText.GetComponent<FloatingText>().Setup(timeToAdd);
        }

        Destroy(gameObject);
    }
}
