//implements a force restraining a player object from an anchor and a filter to determine when the leash is being strained

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class LeashManager : MonoBehaviour
{
    [Header("Leash Anchor")]
    public GameObject anchor;
    [Header("Leash Coefficients")]
    public float inertia = 1f;
    public float damping = 0.05f;
    public float strength = 1f;
    public float maxDist = 2f;

    PlayerInputActions inputActions;
    InputAction move;
    private Rigidbody rb;
    [Header("Strain")]
    public bool strain;
    // Start is called before the first frame update
    void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        inputActions = new PlayerInputActions();
        move = inputActions.Land.Movement;
        inputActions.Enable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Computes leash force
        Vector3 xToY = VectorXToY(this.transform.position, anchor.transform.position);
        float distance = xToY.magnitude;
        float workingDistance = Math.Max(0f, distance - maxDist);
        Vector3 distanceDirection = xToY.normalized;
        Vector3 springForce = (workingDistance * strength) * distanceDirection;
        //Computes small force in the direction of player motion
        Vector2 inputVector = move.ReadValue<Vector2>();
        Vector3 inputVelocity = transform.right * inputVector.x + transform.forward * inputVector.y;
        rb.AddForce(inertia * rb.velocity.magnitude * inputVelocity);
        strain = inputVector.magnitude > 0.05 && workingDistance > 0.05 && !Similar(inputVelocity, springForce);
        //Applies force
        rb.AddForce(springForce);
    }
    //the rest of these are math helper functions that should be in their own class probably
    public static Vector3 VectorXToY(Vector3 x, Vector3 y) {
        return y - x;
    }
    public static float Cosine(Vector3 x, Vector3 y) {
        return Vector3.Dot(x, y) / (x.magnitude * y.magnitude);
    }
    public static bool CloserTo(Vector3 x, Vector3 y, Vector3 z) {
        return (x - y).magnitude < (x - z).magnitude;
    }
    public static bool Similar(Vector3 x, Vector3 y) {
        return CloserTo(x, y, -y);
    }
    public bool getStrain() {
        return strain;
    }
}
