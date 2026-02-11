using UnityEngine;
using System.Collections;

public class ReverseTrigger : MonoBehaviour
{
    public float rewindDuration = 3f;

    private GameObject rewindUIObject;

    void Awake()
    {
       
    }

    private void OnEnable()
    {
        // 🔥 Find even if inactive
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Rewind"))
            {
                rewindUIObject = obj;
                break;
            }
        }

        if (rewindUIObject == null)
        {
            Debug.LogWarning("No GameObject found with tag 'Rewind'");
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlaneController plane = other.GetComponent<PlaneController>();

        if (plane != null)
        {
            StartCoroutine(RewindPlayer(plane));
        }
    }

    IEnumerator RewindPlayer(PlaneController plane)
    {
        plane.canControl = false;
        plane.isRewinding = true;

        // 🔥 Show UI
        if (rewindUIObject != null)
            rewindUIObject.SetActive(true);

        yield return new WaitForSeconds(rewindDuration);

        plane.isRewinding = false;
        plane.canControl = true;

        // 🔥 Hide UI
        if (rewindUIObject != null)
            rewindUIObject.SetActive(false);
    }
}
