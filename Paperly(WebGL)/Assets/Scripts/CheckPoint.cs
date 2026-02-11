using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int checkpointIndex;

    public GameObject reverseTriggerObject;

    public static Action<int> OnCheckpointCollected;

    private bool collected = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (collected)
            return;

        collected = true;

        //  Disable its reverse trigger
        if (reverseTriggerObject != null)
            reverseTriggerObject.SetActive(false);

        OnCheckpointCollected?.Invoke(checkpointIndex);
    }

    public bool IsCollected()
    {
        return collected;
    }
}
