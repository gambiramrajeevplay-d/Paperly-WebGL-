using UnityEngine;

public class TimeCollectable : MonoBehaviour
{
    public float timeToAdd = 10f;

    private TimeManager timeManager;

    void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (timeManager != null)
        {
            timeManager.AddTime(timeToAdd);
        }

        Destroy(gameObject);
    }
}
