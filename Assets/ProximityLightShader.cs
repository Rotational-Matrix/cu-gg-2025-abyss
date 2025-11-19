using UnityEngine;

public class ProximityLightShader : MonoBehaviour
{
    [Header("Assign the object to detect")]
    public Transform targetObject;

    [Header("Color Settings")]
    public Color defaultColor = Color.grey;
    public Color nearColor = Color.white;

    [Header("Distance threshold per axis")]
    public float triggerDistance = 1f;

    [Header("Smoothness of color transition")]
    public float lerpSpeed = 5f;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("ProximityLightShader: No SpriteRenderer found on this object.");
            return;
        }

        spriteRenderer.color = defaultColor;
    }

    void Update()
    {
        if (spriteRenderer == null || targetObject == null) return;

        Vector3 delta = targetObject.position - transform.position;

        bool withinX = Mathf.Abs(delta.x) <= triggerDistance;
        bool withinY = Mathf.Abs(delta.y) <= triggerDistance;
        bool withinZ = Mathf.Abs(delta.z) <= triggerDistance;

        bool insideBox = withinX && withinY && withinZ;

        Color targetColor = insideBox ? nearColor : defaultColor;

        spriteRenderer.color = Color.Lerp(
            spriteRenderer.color,
            targetColor,
            Time.deltaTime * lerpSpeed
        );
    }
}
