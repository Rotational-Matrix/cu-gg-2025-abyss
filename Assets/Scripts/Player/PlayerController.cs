using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private int groundedFrameCount;
    public bool canJump = false; // TESTING
    private Vector2 inputVector;
    SpriteRenderer spriteRenderer;
    private float oldX;
    private Vector3 force;
    private Rigidbody rb;
    public float movementSpeed;
    public float maxSpeed;
    public float jumpSpeed;
    PlayerInputActions inputActions;
    InputAction move;

    /*void OnCollisionEnter(Collision collision)
    {
        grounded = true;
    }
    void OnCollisionExit(Collision collision)
    {
        grounded = false;
    }*/
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody>();
    }
    void OnEnable()
    {
        inputActions = new PlayerInputActions();
        inputActions.Land.Jump.performed += OnJump;
        move = inputActions.Land.Movement;
        inputActions.Enable();
    }
    void OnDisable()
    {
        inputActions.Land.Jump.performed -= OnJump;
        inputActions.Disable();
    }
    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (!canJump) return;
        if (IsGrounded()) rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
    }
    // Update is called once per frame
    void Update()
    {
        if (move == null) return;
        inputVector = move.ReadValue<Vector2>();
        if (spriteRenderer != null)
        {
            if (inputVector.x >= 0.05) spriteRenderer.flipX = false;
            else if (inputVector.x <= -0.05) spriteRenderer.flipX = true;
        }
        if (IsGrounded() && Mathf.Abs(inputVector.x) < 0.05 && Mathf.Abs(inputVector.y) < 0.05) groundedFrameCount++;
        else groundedFrameCount = 0;
        if (groundedFrameCount >= 100 && groundedFrameCount % 59 == 0) spriteRenderer.flipX = !spriteRenderer.flipX;
    }
    void FixedUpdate()
    {
       
        if (inputVector.x * oldX < 0) spriteRenderer.flipX = !spriteRenderer.flipX;
        Vector3 direction = (transform.right * inputVector.x + transform.forward * inputVector.y);
        rb.AddForce(direction * movementSpeed, ForceMode.VelocityChange);
        rb.AddForce(force);
        oldX = inputVector.x;
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
        if (flatVelocity.magnitude > maxSpeed)
        {
            Vector3 clamped = flatVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(clamped.x, rb.velocity.y, clamped.z);
        }
    }
}
