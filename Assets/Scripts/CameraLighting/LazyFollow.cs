using UnityEngine;

[ExecuteAlways] 
public class LazyFollow : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform target;

    [Header("Follow Settings")]
    [Tooltip("follow speed")]
    [Range(0f, 1f)]
    public float followSpeed = 0.1f;

    private void Update()
    {
        if (target == null)
            return;

        // Smoothly interpolate position
        transform.position = Vector3.Lerp(transform.position, target.position, followSpeed);

    }
}
