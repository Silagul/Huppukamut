using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;
using System.Reflection;

public class GameOverManager : MonoBehaviour
{
    [Header("Game Over Settings")]
    public GameObject gameOverCanvas;              // Drag your GameOver Canvas here
    public string deathAnimTrigger = "Sad";      // Optional death animation

    private PlayerStamina playerStamina;
    private bool gameOverTriggered = false;

    void Awake()
    {
        // Modern, non-obsolete way to find the player stamina
        playerStamina = FindFirstObjectByType<PlayerStamina>();

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);
    }

    void Update()
    {
        if (gameOverTriggered || playerStamina == null) return;

        if (playerStamina.stamina <= 0f)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        gameOverTriggered = true;

        GameObject player = playerStamina.gameObject;

        // Disable all control scripts
        var pm = player.GetComponent<PlayerMovement>();
        var ps = player.GetComponent<PlayerStamina>();
        var rb = player.GetComponent<Rigidbody>();
        var anim = player.GetComponentInChildren<Animator>();
        var input = player.GetComponent<PlayerInput>();

        if (pm != null) pm.enabled = false;
        if (ps != null) ps.enabled = false;
        if (input != null) input.enabled = false;

        // Stop all physics
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        // Cancel any active skill + play death animation
        if (anim != null)
        {
            anim.SetBool("Dash", false);
            anim.SetBool("Gliding", false);
            anim.SetBool("Stomp", false);

            if (!string.IsNullOrEmpty(deathAnimTrigger))
                anim.SetTrigger(deathAnimTrigger);
        }

        // Safety reset of private gliding flag
        var glidingField = ps?.GetType().GetField("gliding", 
            BindingFlags.NonPublic | BindingFlags.NonPublic | BindingFlags.Instance);
        glidingField?.SetValue(ps, false);

        // Show Game Over UI
        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(true);


    }

    
}