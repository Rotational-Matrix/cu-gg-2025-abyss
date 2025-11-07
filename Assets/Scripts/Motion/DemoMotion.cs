using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class DemoMotion : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform objectToMove;  
    public float moveSpeed = 3f;

    [Header("Path Points")]
    public List<Vector3> targetPositions = new List<Vector3>
    {
        new Vector3(0f, 0f, 0f),
        new Vector3(1f, 0f, -2f),
        new Vector3(2f, 0f, 2f),
        new Vector3(1f, 0f, 2f)
    };

    private int currentTargetIndex = -1;
    private Vector3 targetPosition;
    private bool moving = false;
    private Vector3 direction = Vector3.zero;
    public Vector3 Direction()
    {
        return direction;
    }
    void Start()
    {
        if (objectToMove == null)
        {
            Debug.LogError("No object assigned to move!");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MoveToNextPosition();
        }

        if (moving)
        {
            objectToMove.position = Vector3.MoveTowards(
                objectToMove.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            direction = (targetPosition - objectToMove.position).normalized;
            if (Vector3.Distance(objectToMove.position, targetPosition) < 0.01f)
            {
                direction = Vector3.zero;
                moving = false; // Stop when reached
                Debug.Log($"Reached position {currentTargetIndex}");
            }
        }
    }

    /// <summary>
    /// Moves to the next position in the list.
    /// Can also be called manually from other scripts.
    /// </summary>
    public void MoveToNextPosition()
    {
        if (moving)
        {
            Debug.Log("Already moving. Wait until the move finishes.");
            return;
        }

        if (targetPositions.Count == 0)
        {
            Debug.LogWarning("No target positions defined!");
            return;
        }

        if (currentTargetIndex + 1 >= targetPositions.Count)
        {
            Debug.Log("Reached the final target. No more positions left.");
            return;
        }

        currentTargetIndex++;
        targetPosition = targetPositions[currentTargetIndex];
        moving = true;
        Debug.Log($"Moving to position {currentTargetIndex}: {targetPosition}");
    }
}
