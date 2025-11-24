using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class StaminaMaterialEffect : MonoBehaviour
{
    [Header("References")]
    public PlayerStamina playerStamina;
    public Animator animator;

    [Header("Shader Settings")]
    public string shaderProperty = "_Stamina";
    [Range(0f, 1f)]
    public float lowStaminaThreshold = 0.5f;

    [Header("Effect Speeds")]
    public float drainSpeed = 0.8f;           // How fast it drops when low/sad
    public float recoverySpeed = 0.4f;        // How fast it recovers when exiting Sad or regaining stamina

    [Header("Sad Animation Override")]
    public string sadBoolName = "Sad";
    [Range(0f, 1f)]
    public float sadEffectStrength = 0.3f;    // Visual intensity when Sad

    private Renderer[] renderers;
    private MaterialPropertyBlock propBlock;
    private float targetValue = 1f;
    private float currentValue = 1f;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        propBlock = new MaterialPropertyBlock();

        if (playerStamina == null)
            playerStamina = FindFirstObjectByType<PlayerStamina>();

        if (animator == null)
            animator = GetComponentInParent<Animator>() ?? FindFirstObjectByType<Animator>();
    }

    private void Update()
    {
        if (playerStamina == null || renderers == null || renderers.Length == 0 || animator == null)
            return;

        float staminaPercent = playerStamina.stamina / playerStamina.maxStamina;
        bool isSad = animator.GetBool(sadBoolName);

        // === Determine Target Value ===
        if (isSad)
        {
            targetValue = sadEffectStrength; // Force sad look
        }
        else if (staminaPercent <= lowStaminaThreshold)
        {
            targetValue = Mathf.InverseLerp(0f, lowStaminaThreshold, staminaPercent);
        }
        else
        {
            targetValue = 1f; // Full health = full visuals
        }

        // === Choose Speed Based on Direction ===
        float speed = currentValue > targetValue ? drainSpeed : recoverySpeed;
        // If we're going down (getting worse) → use drainSpeed
        // If we're going up (recovering) → use slower, more gentle recoverySpeed

        // Smooth movement
        currentValue = Mathf.MoveTowards(currentValue, targetValue, speed * Time.deltaTime);

        // Apply to materials
        foreach (var rend in renderers)
        {
            rend.GetPropertyBlock(propBlock);
            propBlock.SetFloat(shaderProperty, currentValue);
            rend.SetPropertyBlock(propBlock);
        }
    }

    private void OnEnable()
    {
        currentValue = 1f;
        targetValue = 1f;
    }
}