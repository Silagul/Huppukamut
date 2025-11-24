using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class StaminaBarImage : MonoBehaviour
{
    [Header("Visuals")]
    public Gradient staminaGradient = new Gradient();

    [Header("References (auto-found if empty)")]
    public RawImage fillRawImage;     // The RawImage child we want to color
    public PlayerStamina playerStamina;

    private Image maskImage;          // The Image component on THIS GameObject (for fill amount)
    private float maxStamina;

    private void Awake()
    {
        // Get the Image component on this GameObject (required for fill)
        maskImage = GetComponent<Image>();

        // Auto-find the RawImage child
        if (fillRawImage == null)
        {
            fillRawImage = GetComponentInChildren<RawImage>();
            
            // Optional: more specific search if you have multiple RawImages
            // fillRawImage = transform.Find("Fill")?.GetComponent<RawImage>();
        }

        // Configure the mask/fill Image
        maskImage.type = Image.Type.Filled;
        maskImage.fillMethod = Image.FillMethod.Horizontal;
        maskImage.fillOrigin = 0;

        // Default gradient if none set
        if (staminaGradient.colorKeys.Length == 0)
        {
            staminaGradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.green, 0f),
                    new GradientColorKey(new Color(1f, 0.8f, 0f), 0.5f),
                    new GradientColorKey(Color.red, 1f)
                },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
            );
        }
    }

    private void Start()
    {
        if (playerStamina == null)
            playerStamina = FindFirstObjectByType<PlayerStamina>();

        if (playerStamina == null)
        {
            Debug.LogError("[StaminaBarImage] PlayerStamina not found!");
            enabled = false;
            return;
        }

        maxStamina = playerStamina.maxStamina;
        UpdateStaminaVisual(playerStamina.stamina);
    }

    private void Update()
    {
        if (playerStamina != null)
            UpdateStaminaVisual(playerStamina.stamina);
    }

    private void UpdateStaminaVisual(float currentStamina)
    {
        if (maskImage == null || fillRawImage == null) return;

        float normalized = Mathf.Clamp01(currentStamina / maxStamina);

        // Update fill amount on the parent Image (the mask)
        maskImage.fillAmount = normalized;

        // Update color on the CHILD RawImage using gradient (1 - normalized = red when low)
        fillRawImage.color = staminaGradient.Evaluate(1f - normalized);
    }

    // Optional: Smooth fill version
    /*
    private float targetFill = 1f;
    private void Update()
    {
        if (playerStamina == null || maskImage == null || fillRawImage == null) return;

        targetFill = Mathf.Clamp01(playerStamina.stamina / maxStamina);
        maskImage.fillAmount = Mathf.Lerp(maskImage.fillAmount, targetFill, Time.deltaTime * 10f);
        fillRawImage.color = staminaGradient.Evaluate(1f - maskImage.fillAmount);
    }
    */
}