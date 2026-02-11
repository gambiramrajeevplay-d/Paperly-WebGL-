using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPickUp : MonoBehaviour
{
    public GameObject boostPopup;
    public AudioClip pickupSound;

    private AudioSource audioSource;

    void Start()
    {
        // 🔊 Get AudioSource from object tagged "PickUp"
        GameObject audioObj = GameObject.FindGameObjectWithTag("PickUp");

        if (audioObj != null)
        {
            audioSource = audioObj.GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogWarning("No GameObject found with tag 'PickUp'");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlaneController boost = other.GetComponent<PlaneController>();

            if (boost != null)
            {
                boost.hasBoost = true;

                if (boostPopup != null)
                    boostPopup.SetActive(true);

                // 🔊 Play pickup sound
                if (pickupSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(pickupSound);
                }

                Destroy(gameObject);
            }
        }
    }
}
