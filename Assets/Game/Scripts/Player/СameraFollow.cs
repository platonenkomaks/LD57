using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    private void LateUpdate()
    {
        var desiredPosition = target.position + offset;
        transform.position = desiredPosition;
        
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}

