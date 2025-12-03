using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SadMaterialEffect : MonoBehaviour
{
    [Header("References")]
    public Animator animator;                    // Drag your character's Animator here

    [Header("Shader")]
    public string shaderProperty = "_Stamina";   // Or "_SadAmount", "_Dissolve", etc.

    [Header("Sad Effect")]
    public string sadBoolName = "Sad";           // Exact name of your boolean parameter
    [Range(0f, 1f)]
    public float sadIntensity = 0.25f;           // 0 = fully affected, 1 = no effect when sad

    [Header("Timing")]
    public float drainSpeed = 1.4f;              // How fast the effect appears when becoming sad
    public float recoverySpeed = 0.45f;          // How gently the character "recovers" when sad ends

    private Renderer[] renderers;
    private MaterialPropertyBlock propBlock;
    private float targetValue = 1f;
    private float currentValue = 1f;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        propBlock = new MaterialPropertyBlock();

        // Auto-find animator if not assigned
        if (animator == null)
            animator = GetComponentInParent<Animator>() ?? FindFirstObjectByType<Animator>();
    }

    private void Update()
    {
        if (animator == null || renderers == null || renderers.Length == 0) return;

        bool isSad = animator.GetBool(sadBoolName);

        // Set target based only on Sad state
        targetValue = isSad ? sadIntensity : 1f;

        // Choose speed: fast when getting sad, slow/natural when recovering
        float speed = currentValue > targetValue ? drainSpeed : recoverySpeed;

        // Smooth transition
        currentValue = Mathf.MoveTowards(currentValue, targetValue, speed * Time.deltaTime);

        // Apply to all renderers (body, clothes, hair, etc.)
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

    // Optional: visual feedback in editor
    private void Reset()
    {
        animator = GetComponentInParent<Animator>();
    }
}