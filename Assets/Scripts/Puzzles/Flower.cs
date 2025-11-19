using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    [Header("Manager")]
    public GameObject managerParent;
    private FlowerManager manager;

    [Header("Sprites")]
    public List<Sprite> possibleSprites = new List<Sprite>();
    private SpriteRenderer spriteRenderer;

    [Header("Particles")]
    private ParticleSystem particleSystem;

    private bool picked = false;
    private Collider col;

    void Start()
    {
        manager = managerParent.GetComponent<FlowerManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Flower: No SpriteRenderer found.");
        }
        else
        {
            if (possibleSprites.Count > 0)
            {
                spriteRenderer.sprite = possibleSprites[Random.Range(0, possibleSprites.Count)];
            }

            if (Random.value < 0.5f)
            {
                spriteRenderer.flipX = true;
            }
        }

        particleSystem = GetComponentInChildren<ParticleSystem>();
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (picked) return;
        picked = true;

        if (col != null) col.enabled = false;
        if (spriteRenderer != null) spriteRenderer.enabled = false;

        manager.AddFlower();

        if (particleSystem != null)
        {
            ParticleSystem p = Instantiate(particleSystem, transform.position, Quaternion.identity);
            p.Play();
            Destroy(p.gameObject, p.main.duration + p.main.startLifetime.constantMax);
        }

        gameObject.SetActive(false);
    }

    private IEnumerator DisableAfterParticles()
    {
        if (particleSystem != null)
        {
            while (particleSystem.isPlaying)
            {
                yield return null;
            }
        }

        this.gameObject.SetActive(false);
    }
}
