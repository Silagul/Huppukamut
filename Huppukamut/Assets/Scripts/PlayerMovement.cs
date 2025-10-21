using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 20f;
    public float deceleration = 25f;

    [Header("Jump")]
    public float jumpForce = 10f;
    public float gravityScale = 3f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public float variableJumpMultiplier = 0.5f;

    [Header("World constraint")]
    public float fixedZ = 0f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool jumpPressed;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool grounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        // --- timers ---
        if (grounded) coyoteTimer = coyoteTime;
        else coyoteTimer -= Time.deltaTime;

        if (jumpPressed) jumpBufferTimer = jumpBufferTime;
        else jumpBufferTimer -= Time.deltaTime;

        // --- jump ---
        if (jumpBufferTimer > 0 && coyoteTimer > 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, 0);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            jumpBufferTimer = 0;
            coyoteTimer = 0;
        }

        // --- variable jump ---
        if (rb.linearVelocity.y > 0 && !Keyboard.current.spaceKey.isPressed)
        {
            rb.AddForce(Vector3.down * (1f - variableJumpMultiplier) * Physics.gravity.y * Time.deltaTime, ForceMode.Acceleration);
        }

        // --- enforce 2.5D plane ---
        Vector3 pos = transform.position;
        pos.z = fixedZ;
        transform.position = pos;

        jumpPressed = false;
    }

    void FixedUpdate()
    {
        // --- horizontal velocity ---
        float targetX = moveInput.x * moveSpeed;
        float currentX = rb.linearVelocity.x;
        float accel = Mathf.Abs(targetX) > 0.01f ? acceleration : deceleration;
        currentX = Mathf.MoveTowards(currentX, targetX, accel * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(currentX, rb.linearVelocity.y, 0);

        // --- gravity multiplier ---
        if (!grounded)
        {
            Vector3 extraGravity = Physics.gravity * (gravityScale - 1f);
            rb.AddForce(extraGravity, ForceMode.Acceleration);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        grounded = false;
        foreach (var contact in collision.contacts)
        {
            if (Vector3.Angle(contact.normal, Vector3.up) < 45f)
            {
                grounded = true;
                break;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        grounded = false;
    }

    // --- Input System callbacks ---
    public void Move(InputAction.CallbackContext ctx) =>
        moveInput = ctx.ReadValue<Vector2>();

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) jumpPressed = true;
    }
}
