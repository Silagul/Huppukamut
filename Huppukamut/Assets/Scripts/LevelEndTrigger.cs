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
    public string helpingTriggerName = "Helping";

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

        var inputActions = other.GetComponent<PlayerInput>();
        if (inputActions != null) inputActions.DeactivateInput();

        // 3. Stop horizontal movement - let player fall naturally
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }

        // 4. Force-cancel active skills and play victory → idle → helping sequence
        if (anim != null)
        {
            anim.SetBool("Dash", false);
            anim.SetBool("Gliding", false);
            anim.SetFloat("Speed", 0f);
            anim.SetBool("Jump", false);
            anim.SetBool("Grounded", true);

            anim.SetTrigger(victoryTriggerName);

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
        yield return new WaitForSeconds(1.5f);
        anim.SetTrigger(helpingTriggerName);
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(delayBeforeLoad);

        // ⭐ Tallenna score ennen scenenvaihtoa ⭐
        if (ScoreManager.instance != null)
        {
            PlayerPrefs.SetInt("FinalScore", ScoreManager.instance.CurrentScore);
            PlayerPrefs.Save();
            Debug.Log("Tallennettu score: " + ScoreManager.instance.CurrentScore);
        }
        else
        {
            Debug.LogError("ScoreManager.instance on NULL!");
        }

        // Lataa lopetusscenen
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
