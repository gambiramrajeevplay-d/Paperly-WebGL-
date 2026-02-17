using UnityEngine;

public class BoostPickUp : MonoBehaviour
{
    public GameObject boostPopup;
    public AudioClip pickupSound;

    private AudioSource audioSource;
    private PlaneController plane;

    void Start()
    {
        if (boostPopup != null)
            boostPopup.SetActive(false);

        GameObject audioObj = GameObject.FindGameObjectWithTag("PickUp");
        if (audioObj != null)
            audioSource = audioObj.GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        plane = other.GetComponent<PlaneController>();
        if (plane == null) return;

        plane.hasBoost = true;

        if (boostPopup != null)
            boostPopup.SetActive(true);

        // 🔊 Pickup sound
        if (pickupSound != null && audioSource != null)
            audioSource.PlayOneShot(pickupSound);

        // 🔔 Subscribe to boost end
        plane.OnBoostFinished += HidePopup;
    }

    void HidePopup()
    {
        if (boostPopup != null)
            boostPopup.SetActive(false);

        // 🔕 Unsubscribe to avoid memory leaks
        plane.OnBoostFinished -= HidePopup;
    }
}
