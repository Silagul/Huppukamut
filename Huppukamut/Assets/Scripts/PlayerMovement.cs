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
    public float fallGravityMultiplier = 2f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public float originalFallGravityMultiplier;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.15f;
    public LayerMask groundLayer;
    public bool grounded;

    [Header("World Constraint")]
    public float fixedZ = 0f;

    [Header("Movement Particles")]
    public GameObject runningDustPrefab;    // Dust trail for running
    public GameObject jumpDustPrefab;       // Dust puff for jump takeoff
    public Transform dustSpawnPoint;        // Assign feet position (optional, defaults to bottom of collider)

    [Header("Particle Settings")]
    public float runningDustRate = 0.15f;   // Dust every X seconds while running
    public float minRunSpeedForDust = 2f;   // Only dust if moving fast enough

    private Rigidbody rb;
    private PlayerStamina playerStamina;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool isJumpHeld;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool wasGroundedLastFrame;

    // Particle timing
    private float runningDustTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerStamina = GetComponent<PlayerStamina>();
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

        // --- Jump logic + JUMP DUST ---
        if (jumpBufferTimer > 0 && coyoteTimer > 0f)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

            // JUMP SOUND + JUMP DUST PUFF
            SoundManager.instance?.PlayJump();
            SpawnJumpDust();

            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        // LANDING SOUND + LAND DUST (optional heavy puff)
        if (!wasGroundedLastFrame && grounded)
        {
            SoundManager.instance?.PlayLand();
            // Optional: SpawnLandDust(); // uncomment if you want extra landing puff
        }
        wasGroundedLastFrame = grounded;

        // --- RUNNING DUST TRAIL ---
        SpawnRunningDust();

        // --- enforce 2.5D plane ---
        Vector3 pos = transform.position;
        pos.z = fixedZ;
        transform.position = pos;

        jumpPressed = false;
    }

    void FixedUpdate()
    {
        // --- horizontal movement ---
        float targetX = moveInput.x * moveSpeed;
        float accel = Mathf.Abs(targetX) > 0.01f ? acceleration : deceleration;
        if (playerStamina.IsDashing())
            accel = 0;

        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetX, accel * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(newX, rb.linearVelocity.y, 0f);

        // --- gravity control ---
        Vector3 extraGravity = Physics.gravity * (gravityScale - 1f);

        if (rb.linearVelocity.y < 0f)
            extraGravity += Physics.gravity * (fallGravityMultiplier - 1f);
        else if (rb.linearVelocity.y > 0f && !isJumpHeld)
            extraGravity += Physics.gravity * (fallGravityMultiplier - 1f);

        rb.AddForce(extraGravity, ForceMode.Acceleration);
    }

    // --- Input System callbacks ---
    public void Move(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();

        if (moveInput.x != 0f && !playerStamina.IsDashing())
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

    // ──────── PARTICLE SPAWNERS ────────
    private void SpawnRunningDust()
    {
        if (!grounded || playerStamina.IsDashing() || playerStamina.IsGliding()) return;

        float currentSpeed = Mathf.Abs(rb.linearVelocity.x);
        if (currentSpeed < minRunSpeedForDust) return;

        runningDustTimer -= Time.deltaTime;
        if (runningDustTimer <= 0f)
        {
            if (runningDustPrefab != null)
            {
                Vector3 spawnPos = dustSpawnPoint ? dustSpawnPoint.position : 
                                 new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
                
                GameObject dust = Instantiate(runningDustPrefab, spawnPos, Quaternion.identity);
                Destroy(dust, 2f); // dust disappears quickly
            }
            runningDustTimer = runningDustRate;
        }
    }

    private void SpawnJumpDust()
    {
        if (jumpDustPrefab != null)
        {
            Vector3 spawnPos = dustSpawnPoint ? dustSpawnPoint.position : 
                             new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            
            GameObject dust = Instantiate(jumpDustPrefab, spawnPos, Quaternion.identity);
            Destroy(dust, 1.5f);
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