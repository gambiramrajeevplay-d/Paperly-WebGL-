using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public GameObject levelPassPanel;
    public GameObject levelGameObject;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlaneController plane = other.GetComponent<PlaneController>();

        if (plane != null)
        {
            // Stop control
            plane.canControl = false;
            plane.isRewinding = false;

            // Stop movement
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // Disable level objects
        if (levelGameObject != null)
            levelGameObject.SetActive(false);

        // Show level pass UI
        if (levelPassPanel != null)
            levelPassPanel.SetActive(true);

        // Optional: Pause time
        Time.timeScale = 0f;
    }
}
