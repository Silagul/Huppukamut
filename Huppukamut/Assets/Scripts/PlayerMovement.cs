using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 20f;
    public float deceleration = 25f;
    public float originalDeceleration;

    [Header("Jump")]
    public float jumpForce = 10f;
    public float gravityScale = 3f;
    public float fallGravityMultiplier = 2f; // stronger gravity when falling
    public float coyoteTime = 0.1f;           // grace after leaving ground
    public float jumpBufferTime = 0.1f;       // grace before hitting ground
    public float originalFallGravityMultiplier;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.15f;
    public LayerMask groundLayer;
    public bool grounded;

    [Header("World Constraint")]
    public float fixedZ = 0f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool isJumpHeld;
    private float coyoteTimer;
    private float jumpBufferTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        originalDeceleration = deceleration;
        originalFallGravityMultiplier = fallGravityMultiplier;
    }

    void Update()
    {
        // --- Input timing ---
        if (jumpPressed) jumpBufferTimer = jumpBufferTime;
        else jumpBufferTimer -= Time.deltaTime;

        // --- Ground check ---
        grounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer);

        if (grounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        // --- Jump trigger ---
        if (jumpBufferTimer > 0 && coyoteTimer > 0f)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = 0f; // reset vertical velocity before jump
            rb.linearVelocity = vel;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        // --- enforce 2.5D plane ---
        Vector3 pos = transform.position;
        pos.z = fixedZ;
        transform.position = pos;

        jumpPressed = false;
    }

    void FixedUpdate()
    {
        // --- horizontal movement (independent of grounded) ---
        float targetX = moveInput.x * moveSpeed;
        float accel = Mathf.Abs(targetX) > 0.01f ? acceleration : deceleration;
        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetX, accel * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(newX, rb.linearVelocity.y, 0f);

        // --- gravity control for variable jump ---
        Vector3 extraGravity = Physics.gravity * (gravityScale - 1f);

        if (rb.linearVelocity.y < 0f)
        {
            // falling ? stronger gravity
            extraGravity += Physics.gravity * (fallGravityMultiplier - 1f);
        }
        else if (rb.linearVelocity.y > 0f && !isJumpHeld)
        {
            // released jump early ? shorten jump
            extraGravity += Physics.gravity * (fallGravityMultiplier - 1f);
        }

        rb.AddForce(extraGravity, ForceMode.Acceleration);
    }

    // --- Input System callbacks ---
    public void Move(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();

        // Flip character based on movement direction (optional)
        if (moveInput.x != 0f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(moveInput.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            jumpPressed = true;
            isJumpHeld = true;
        }
        else if (ctx.canceled)
        {
            isJumpHeld = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}
