using UnityEngine;

public class MovementAudio : MonoBehaviour
{
    [Header("Movement audio settings")]
    [Tooltip("The default audio clip to loop while character is moving")]
    public AudioClip footstepClip;
    [Tooltip("The audio clip to loop while character is in cave")]
    public AudioClip caveFootstepClip;
    [Tooltip("Minimum movement speed to trigger sound")]
    public float movementThreshold = 0.1f;
    [Tooltip("Maximum distance from cave at which cave footsteps play")]
    public float caveDetectionRadius = 20f;
    [Tooltip("Reference to the cave GameObject")]
    public GameObject cave;
    private AudioSource audioSource;
    private Vector3 lastPosition;
    private Rigidbody rb;
    private CharacterController controller;
    private Vector2 cavePosition;
    private bool inCave;

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

        cavePosition = new Vector2(cave.transform.position.x, cave.transform.position.z);
        inCave = false;
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
        Vector2 position2D = new Vector2(transform.position.x, transform.position.z);
        float distanceToCave = Vector2.Distance(position2D, cavePosition);
        //Debug.Log("Distance to cave: " + distanceToCave); [Cu] blocked this bc it is always running
        inCave = distanceToCave <= caveDetectionRadius;
        if (isMoving && !audioSource.isPlaying && footstepClip != null)
        {
            audioSource.clip = inCave && caveFootstepClip != null ? caveFootstepClip : footstepClip;
            audioSource.Play();
        }
        else if (isMoving && audioSource.isPlaying && MismatchedAudio()) {
            if (inCave && audioSource != null) { 
                audioSource.clip = caveFootstepClip;
            }
            else if (!inCave && audioSource != null) {
                audioSource.clip = footstepClip;
            }
        }
        else if (!isMoving && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
    bool MismatchedAudio() {
        return inCave && audioSource.clip != caveFootstepClip || !inCave && audioSource.clip != footstepClip;
    }
}
