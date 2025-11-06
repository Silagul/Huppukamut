using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStamina : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public float maxStamina;
    public float stamina;
    public float staminaDecayRate;
    public float helpingStaminaCost;

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

    public GameObject FindClosestTagged(string tag)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;    // Vector from this to target
            float curDistance = diff.sqrMagnitude;
            if (curDistance < 4 && curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        GameObject h = FindClosestTagged("Helpee");

        if ( h != null && h.TryGetComponent<HelpeeAi>(out HelpeeAi helpee))
        {
            if (stamina >= helpingStaminaCost)
            {
                stamina -= helpingStaminaCost;
                helpee.SetDestination(helpee.goal);
            }
            else
            {
                print("Not enough stamina.");
            }
        }
        else
        {
            print("No target found.");
        }
    }
}
