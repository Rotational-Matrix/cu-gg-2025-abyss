using UnityEngine;

public class MovementAudio : MonoBehaviour
{
    [Header("Movement audio settings")]
    [Tooltip("The audio clip to loop while character is moving")]
    public AudioClip footstepClip;

    [Tooltip("Minimum movement speed to trigger sound")]
    public float movementThreshold = 0.1f;

    private AudioSource audioSource;
    private Vector3 lastPosition;
    private Rigidbody rb;
    private CharacterController controller;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = footstepClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        lastPosition = transform.position;
    }

    void Update()
    {
        float speed = 0f;

        if (rb != null)
        {
            speed = rb.velocity.magnitude;
        }
        else if (controller != null)
        {
            speed = controller.velocity.magnitude;
        }
        else
        {
            speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
            lastPosition = transform.position;
        }

        bool isMoving = speed > movementThreshold;

        if (isMoving && !audioSource.isPlaying && footstepClip != null)
        {
            audioSource.Play();
        }
        else if (!isMoving && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
