using UnityEngine;

public class PlaneCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2.5f, -7f);

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;
    }
}
