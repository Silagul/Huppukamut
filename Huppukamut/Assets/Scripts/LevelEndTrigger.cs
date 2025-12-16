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

        // 🔥 PLAY HELPING FRIEND CHEER SOUND IMMEDIATELY 🔥
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayHelpFriend();
        }
        else
        {
            Debug.LogWarning("SoundManager.instance is NULL - no helping sound played!");
        }

        // Play all particle systems that are children of this trigger object (once)
        foreach (var particle in GetComponentsInChildren<ParticleSystem>())
        {
            var main = particle.main;
            main.loop = false;
            particle.Play();
        }

        // Player components
        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        PlayerStamina ps = other.GetComponent<PlayerStamina>();
        Rigidbody rb = other.GetComponent<Rigidbody>();
        Animator anim = other.GetComponentInChildren<Animator>();

        // Disable controls
        if (pm != null) pm.enabled = false;
        if (ps != null) ps.enabled = false;

        var inputActions = other.GetComponent<PlayerInput>();
        if (inputActions != null) inputActions.DeactivateInput();

        // Stop horizontal movement
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }

        // Animation sequence
        if (anim != null)
        {
            anim.SetBool("Dash", false);
            anim.SetBool("Gliding", false);
            anim.SetFloat("Speed", 0f);
            anim.SetBool("Jump", false);
            anim.SetBool("Grounded", true);

            anim.SetTrigger(victoryTriggerName);
            StartCoroutine(PlayHelpingAnimationAfterDelay(anim));
        }

        // Reset gliding flag + freeze Z
        if (ps != null)
        {
            FieldInfo glidingField = ps.GetType().GetField("gliding",
                BindingFlags.NonPublic | BindingFlags.Instance);
            glidingField?.SetValue(ps, false);

            if (rb != null)
                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        }

        StartCoroutine(LoadNextScene());
    }

    private IEnumerator PlayHelpingAnimationAfterDelay(Animator anim)
    {
        yield return new WaitForSeconds(1.5f);
        anim.SetTrigger(helpingTriggerName);
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(delayBeforeLoad);

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