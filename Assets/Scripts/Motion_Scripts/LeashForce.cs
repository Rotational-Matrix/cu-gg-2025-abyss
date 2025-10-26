using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeashForce : MonoBehaviour
{
    public GameObject anchor;
    private GameObject subject;
    [Header("Leash Properties")]
    public float pull = 1f;
    public float maxDist = 2f;
    public float damping = 0.1f;

    private Rigidbody rb;
    public Vector3 BaseForce(GameObject x, GameObject y)
    {
        Vector3 d = Distance.XToY(x, y);
        float realDist = Distance.Dist(x, y) - maxDist;
        if (realDist < 0) realDist = 0;
        Vector3 force = pull * realDist * d.normalized;
        return force;
    }
    public void ApplyForce(GameObject x, GameObject y)
    {
        Vector3 v = rb.velocity;
        v.y = 0f;
        Vector3 f = BaseForce(x, y);
        Vector3 d = Distance.XToY(x, y);
        d.y = 0;
        float stabilizer = Vector3.Dot(d, v);
        Vector3 s = stabilizer * damping * -d.normalized;
        rb.AddForce(f + s);
        Debug.Log("force: " + (f + s) + "\nanchor position: " + y.transform.position + "\nsubject position: " + x.transform.position);
    }
    public void Awake()
    {
        subject = this.gameObject;
        rb = GetComponent<Rigidbody>();
    }
    public void FixedUpdate()
    {
        ApplyForce(subject, anchor);
    }
}
