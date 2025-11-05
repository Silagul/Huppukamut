using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public float maxStamina;
    public float stamina;
    public float staminaDecayRate;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        stamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.linearVelocity.x != 0f)
        {
            stamina -= Time.deltaTime * staminaDecayRate;
        }
    }
}
