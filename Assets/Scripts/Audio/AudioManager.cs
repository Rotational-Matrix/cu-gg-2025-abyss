using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Header("Track Settings")]
    [Tooltip("List of background tracks to play in order")]
    public List<AudioClip> tracks = new List<AudioClip>();

    [Tooltip("Time (in seconds) to fade between tracks")]
    public float fadeDuration = 1.5f;

    [Tooltip("On: automatically move to the next track when current finishes. Off: loop current track")]
    public bool autoAdvance = true;

    private AudioSource audioSource;
    private int currentTrackIndex = 0;
    private bool isFading = false;

    void Awake()
    {
        if (FindObjectsOfType<AudioManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        if (tracks.Count > 0)
        {
            PlayTrack(0);
        }
        else
        {
            Debug.LogWarning("AudioManager: No tracks assigned");
        }
    }

    void Update()
    {
        if (!audioSource.isPlaying && !isFading && tracks.Count > 0)
        {
            if (autoAdvance)
                NextTrack();
            else
                audioSource.Play();
        }
    }

    public void NextTrack()
    {
        int nextIndex = (currentTrackIndex + 1) % tracks.Count;
        StartCoroutine(FadeToTrack(nextIndex));
    }

    public void PlayTrack(int index)
    {
        if (index < 0 || index >= tracks.Count) return;

        currentTrackIndex = index;
        audioSource.clip = tracks[index];
        audioSource.volume = 1f;
        audioSource.Play();
    }

    private System.Collections.IEnumerator FadeToTrack(int newIndex)
    {
        if (isFading) yield break;
        isFading = true;

        float startVolume = audioSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0;
        audioSource.Stop();

        PlayTrack(newIndex);

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, 1f, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 1f;

        isFading = false;
    }
}
