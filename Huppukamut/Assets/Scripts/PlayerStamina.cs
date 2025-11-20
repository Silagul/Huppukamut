using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStamina : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Animator animator;
    public GameObject[] characters;
    public PlayerChoices playerChoices;
    public float maxStamina;
    public float stamina;
    public float staminaDecayRate;
    public float helpingStaminaCost;

    private Rigidbody rb;
    private bool gliding = false;
    private bool canGlide = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        stamina = maxStamina;

        Animator[] animators = transform.GetComponentsInChildren<Animator>();
        characters = new GameObject[animators.Length];

        for (int i = 0; i < animators.Length; i++)
        {
            characters[i] = animators[i].gameObject;
        }

        foreach (GameObject character in characters)
        {
            if (character.gameObject.name != playerChoices.characterName)
            {
                character.SetActive(false);
            }
            else
            {
                animator = character.GetComponent<Animator>();
            }
        }
        //animator = transform.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.linearVelocity.x != 0f)
        {
            stamina -= Time.deltaTime * staminaDecayRate;
        }

        if (playerMovement.grounded)
        {
            gliding = false;
            canGlide = true;
            animator.SetBool("Gliding", false);
        }
    }

    private void FixedUpdate()
    {
        if (gliding)
        {
            playerMovement.fallGravityMultiplier = 1f;
            if (rb.linearVelocity.x < 0)
            {
                rb.AddForce(Vector3.up * Time.fixedDeltaTime * 15, ForceMode.VelocityChange);
            }
            
            if (transform.localScale.x == 1)
            {
                rb.AddForce(Vector3.right * Time.fixedDeltaTime * 30, ForceMode.VelocityChange);
            }
            else
            {
                rb.AddForce(Vector3.left * Time.fixedDeltaTime * 30, ForceMode.VelocityChange);
            }
        }
        else
        {
            playerMovement.fallGravityMultiplier = 4f;
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
                helpee.stamina = helpee.maxStamina;
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

    public void Sprint(InputAction.CallbackContext ctx)
    {
        if (!playerMovement.grounded)
        {
            if (ctx.performed && canGlide)
            {
                gliding = true;
                animator.SetBool("Gliding", true);
            }
            else if (ctx.canceled)
            {
                gliding = false;
                animator.SetBool("Gliding", false);
            }
            else if (canGlide)
            {
                stamina -= 5;
                /*rb.AddForce(Vector3.up * (playerMovement.jumpForce / 2), ForceMode.VelocityChange);
                if (transform.localScale.x == 1)
                {
                    rb.AddForce(Vector3.right * (playerMovement.jumpForce * 0.75f), ForceMode.VelocityChange);
                }
                else
                {
                    rb.AddForce(Vector3.left * (playerMovement.jumpForce * 0.75f), ForceMode.VelocityChange);
                }*/

                gliding = true;
                canGlide = false;
                animator.SetBool("Gliding", true);
            }
        }
    }
}
