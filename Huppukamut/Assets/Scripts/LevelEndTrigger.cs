using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Reflection;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class LevelEndTrigger : MonoBehaviour
{
    [Header("Level End Settings")]
    public string nextSceneName = "EndingScene";
    public float delayBeforeLoad = 3f;
    public string victoryTriggerName = "Chosen";
    public string helpingTriggerName = "Helping";  // ADD THIS - plays after victory

    private bool alreadyTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        timer.Instance.Pause();
        if (alreadyTriggered) return;
        if (!other.CompareTag("Player")) return;

        alreadyTriggered = true;

        // 1. Get components once
        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        PlayerStamina ps = other.GetComponent<PlayerStamina>();
        Rigidbody rb = other.GetComponent<Rigidbody>();
        Animator anim = other.GetComponentInChildren<Animator>();

        // 2. Completely disable player control
        if (pm != null) pm.enabled = false;
        if (ps != null) ps.enabled = false;

        var inputActions = other.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (inputActions != null) inputActions.DeactivateInput();

        // 3. ONLY stop horizontal movement - let player FALL naturally
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }

        // 4. Force-cancel any active skill + play victory → idle → helping sequence
        if (anim != null)
        {
            // Reset ALL possible states to force idle
            anim.SetBool("Dash", false);
            anim.SetBool("Gliding", false);
            anim.SetBool("Stomp", false);
            anim.SetFloat("Speed", 0f);      // ADD: Reset speed param
            anim.SetBool("Jump", false);     // ADD: Reset jump param
            anim.SetBool("Grounded", true);  // ADD: Force grounded param

            // Play victory animation first
            anim.SetTrigger(victoryTriggerName);

            // Then force Helping animation after short delay (plays idle in between)
            StartCoroutine(PlayHelpingAfterVictory(anim));
        }

        // Safety: reset private gliding flag
        if (ps != null)
        {
            FieldInfo glidingField = ps.GetType().GetField("gliding",
                BindingFlags.NonPublic | BindingFlags.Instance);
            glidingField?.SetValue(ps, false);

            if (rb != null)
                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        }

        // Load scene after delay
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator PlayHelpingAfterVictory(Animator anim)
    {
        // Wait for victory anim to finish (adjust timing as needed)
        yield return new WaitForSeconds(1.5f);  // Or use anim.GetCurrentAnimatorStateInfo(0).length

        // Force Helping trigger (plays idle → helping naturally)
        anim.SetTrigger(helpingTriggerName);
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(nextSceneName);
    }

    private void OnDrawGizmosSelected()
    {
        if (TryGetComponent<Collider>(out var col))
        {
            Gizmos.color = new Color(0f, 1f, 1f, 0.35f);
            if (col is BoxCollider box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
            }
        }
    }
}