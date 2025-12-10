using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Reflection; // Only needed for one tiny line
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class LevelEndTrigger : MonoBehaviour
{
    [Header("Level End Settings")]
    public string nextSceneName = "EndingScene";        // Set in Inspector
    public float delayBeforeLoad = 3f;
    public string victoryTriggerName = "Chosen";  // Name of your Animator Trigger parameter

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

        // 3. Stop all physics movement (modern Unity way)
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // Best way to freeze everything
        }

        // 4. Force-cancel any active skill (dash/glide/stomp) — no script changes needed
        if (anim != null)
        {
            anim.SetBool("Dash", false);
            anim.SetBool("Gliding", false);
            anim.SetBool("Stomp", false);
            anim.SetTrigger(victoryTriggerName);
        }

        // Tiny safety: reset the private "gliding" flag in PlayerStamina so nothing gets stuck
        if (ps != null)
        {
            FieldInfo glidingField = ps.GetType().GetField("gliding",
                BindingFlags.NonPublic | BindingFlags.Instance);
            glidingField?.SetValue(ps, false);

            // Unfreeze X axis if stomp had locked it
            if (rb != null)
                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        }

        // 5. Load next scene after delay
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(nextSceneName);
    }

    // Visual helper in Scene view
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