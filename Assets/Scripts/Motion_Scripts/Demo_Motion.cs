using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoveToVectorUI : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform objectToMove;     // The object you want to control
    public float moveSpeed = 3f;

    [Header("UI References")]
    public TMP_InputField positionInput;
    public Button moveButton;

    private Vector3 targetPosition;
    private bool moving = false;

    void Start()
    {
        // Hook up the button listener
        if (moveButton != null)
            moveButton.onClick.AddListener(OnMoveButtonPressed);
        TextMeshProUGUI buttonText = moveButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = "Move anchor";
    }

    void Update()
    {
        if (!moving) return;

        objectToMove.position = Vector3.MoveTowards(
            objectToMove.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(objectToMove.position, targetPosition) < 0.01f)
            moving = false;
    }

    void OnMoveButtonPressed()
    {
        if (string.IsNullOrEmpty(positionInput.text)) return;

        string[] parts = positionInput.text.Split(',');
        if (parts.Length != 3)
        {
            Debug.LogWarning("Input must be in format: x, y, z");
            return;
        }

        try
        {
            float x = float.Parse(parts[0]);
            float y = float.Parse(parts[1]);
            float z = float.Parse(parts[2]);
            targetPosition = new Vector3(x, y, z);
            moving = true;
        }
        catch
        {
            Debug.LogWarning("Invalid vector input! Use numeric values like: 1, 2, 3");
        }
    }
}