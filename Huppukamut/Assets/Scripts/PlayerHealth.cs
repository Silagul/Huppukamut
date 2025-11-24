using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class StaminaBarImage : MonoBehaviour
{
    [Header("Visual Settings")]
    [Range(0f, 1f)]
    public float fillAmount = 1f;                // Current fill (0–1)
    public Gradient gradient = new Gradient();   // Color changes from full → empty

    [Header("Auto-Reference")]
    public PlayerStamina playerStamina;          // Optional: auto-found if empty

    private Image fillImage;
    private float maxStamina;

    private void Awake()
    {
        // This script should be on the Fill Image (the one inside the Mask)
        fillImage = GetComponent<Image>();
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = 0; // Left to right (0 = left, 1 = right, 2 = bottom, 3 = top)

        // Default nice gradient: Green → Yellow → Red
        if (gradient.colorKeys.Length == 0)
        {
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.green, 0f),
                    new GradientColorKey(Color.yellow, 0.5f),
                    new GradientColorKey(Color.red, 1f)
                },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
            );
        }
    }

    private void Start()
    {
        // Auto-find PlayerStamina if not assigned
        if (playerStamina == null)
            playerStamina = FindObjectOfType<PlayerStamina>();

        if (playerStamina == null)
        {
            Debug.LogError("[StaminaBarImage] No PlayerStamina found in scene!");
            enabled = false;
            return;
        }

        maxStamina = playerStamina.maxStamina;
        UpdateBar(playerStamina.stamina);
    }

    private void Update()
    {
        if (playerStamina != null)
            UpdateBar(playerStamina.stamina);
    }

    private void UpdateBar(float currentStamina)
    {
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        fillAmount = currentStamina / maxStamina;

        fillImage.fillAmount = fillAmount;
        fillImage.color = gradient.Evaluate(1f - fillAmount); // 1 = full (green), 0 = empty (red)
    }

    // Optional: Smooth fill animation
    /*
    private void Update()
    {
        if (playerStamina == null) return;

        float target = playerStamina.stamina / maxStamina;
        fillAmount = Mathf.Lerp(fillAmount, target, Time.deltaTime * 8f);

        fillImage.fillAmount = fillAmount;
        fillImage.color = gradient.Evaluate(1f - fillAmount);
    }
    */
}