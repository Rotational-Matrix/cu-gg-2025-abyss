using System.Collections;
using System.Collections.Generic;
using UnityEditor.Sprites;
using UnityEngine;

public class CobwebTrigger : MonoBehaviour
{
    [Header("Particles")]
    private ParticleSystem particleSystemOnGrab;

    private bool taken = false;
    private Collider col;




    // Start is called before the first frame update
    void Start()
    {
        particleSystemOnGrab = GetComponentInChildren<ParticleSystem>();
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!object.Equals(StateManager.Eve.pCollider, other)) return; // i.e. only trigger on eve collider
        if (taken || !StateManager.RCommander.CobwebCanBeTaken()) return;
        taken = true;

        if (col != null) col.enabled = false;

        StateManager.RCommander.ReactToCobweb();

        if (particleSystemOnGrab != null)
        {
            ParticleSystem p = Instantiate(particleSystemOnGrab, transform.position, Quaternion.identity);
            p.Play();
            Destroy(p.gameObject, p.main.duration + p.main.startLifetime.constantMax);
        }

        gameObject.SetActive(false);
    }

    public void SetCobwebActive(bool value)
    {
        taken = !value;
        if (col != null) col.enabled = value;
        gameObject.SetActive(value);
    }


}
