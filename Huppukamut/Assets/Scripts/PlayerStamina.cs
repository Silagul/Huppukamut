using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStamina : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Animator animator;
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

        animator = transform.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Stamina", stamina / maxStamina);
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x) / playerMovement.moveSpeed);

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
            if (stamina >= helpingStaminaCost && helpee.stamina == 0)
            {
                animator.SetTrigger("Helping");
                stamina -= helpingStaminaCost;
                helpee.stamina += helpingStaminaCost;
                helpee.SetDestination(helpee.goal);
            }
            else
            {
                print("It can't be helped.");
            }
        }
        else
        {
            print("No target found.");
        }
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        animator.SetTrigger("Jumping");
    }
}
