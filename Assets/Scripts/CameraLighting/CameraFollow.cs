using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;    

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0f, 2f, -10f); 
    public float smoothSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null) return;

        // lock Z for 2.5D
        Vector3 desiredPosition = target.position + offset;
        //desiredPosition.z = offset.z; // keep camera's orthographic depth

        // interpolate from current to target position
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.position = smoothedPosition;
    }
}
