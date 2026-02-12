using UnityEngine;

public class PlaneCameraFollow : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 2.5f, -7f);

    [SerializeField] private Transform target;

    void Start()
    {
        FindPlayer();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindPlayer(); // in case player spawns later
            return;
        }

        transform.position = target.position + offset;
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Rocket");
        if (player != null)
        {
            target = player.transform;
        }
    }
}
