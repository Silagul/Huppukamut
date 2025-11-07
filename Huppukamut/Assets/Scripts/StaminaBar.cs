using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 20f;    // per second while running
    public float staminaRegenRate = 10f;    // per second when not running
    public float regenDelay = 1.5f;         // seconds before regen starts

    [Header("UI")]
    public Slider staminaBar;

    private float currentStamina;
    private bool isRunning = false;
    private float regenTimer = 0f;

    void Start()
    {
        currentStamina = maxStamina;
        UpdateUI();
    }

    void Update()
    {
        HandleInput();
        UpdateStamina();
        UpdateUI();
    }

    void HandleInput()
    {
        // Example: Hold Left Shift to run
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0)
        {
            isRunning = true;
            regenTimer = 0f;
        }
        else
        {
            isRunning = false;
        }
    }

    void UpdateStamina()
    {
        if (isRunning)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina < 0)
                currentStamina = 0;
        }
        else
        {
            // Delay before regen starts
            regenTimer += Time.deltaTime;
            if (regenTimer >= regenDelay && currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                if (currentStamina > maxStamina)
                    currentStamina = maxStamina;
            }
        }
    }

    void UpdateUI()
    {
        if (staminaBar != null)
            staminaBar.value = currentStamina / maxStamina;
    }

    // Optional: public getter if other scripts need to check stamina
    public float GetStaminaPercent()
    {
        return currentStamina / maxStamina;
    }
}
