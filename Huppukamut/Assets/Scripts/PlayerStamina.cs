using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Animator animator;
    public GameObject[] characters;
    public PlayerChoices playerChoices;
    public Image icon;
    public float maxStamina;
    public float stamina;
    public float staminaDecayRate;
    public float helpingStaminaCost;
    public float skillCooldownTime;

    private Rigidbody rb;
    private float dashTimer = 2f;
    private bool gliding = false;
    private bool canGlide = true;
    //private float skillCooldownTimer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        stamina = maxStamina;

        Animator[] animators = transform.GetComponentsInChildren<Animator>();
        characters = new GameObject[animators.Length];
        icon = GameObject.Find("PlayerIcon").GetComponent<Image>();

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
                icon.sprite = character.GetComponent<HelpeeUI>().icon;
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
            canGlide = true;
            if (animator.GetParameter(6).name == "Gliding")
            {
                gliding = false;
                animator.SetBool("Gliding", false);
            }
            if (animator.GetParameter(6).name == "Stomp")
            {
                gliding = false;
                animator.SetBool("Stomp", false);
                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            }
        }

        if (IsDashing())
        {
            dashTimer -= Time.deltaTime;
        }
        if (dashTimer < 0 && gliding)
        {
            gliding = false;
            animator.SetBool("Dash", false);
            dashTimer = 2f;
        }
    }

    private void FixedUpdate()
    {
        if (animator.GetParameter(6).name == "Gliding" && gliding)
        {
            playerMovement.fallGravityMultiplier = 1f;
            if (rb.linearVelocity.y < 0)
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
        else if (IsDashing())
        {
            //playerMovement.deceleration = 0;
        }
        else
        {
            playerMovement.fallGravityMultiplier = playerMovement.originalFallGravityMultiplier;
            //playerMovement.deceleration = playerMovement.originalDeceleration;
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
        if (animator.GetParameter(6).name == "Gliding")
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

                    gliding = true;
                    canGlide = false;
                    animator.SetBool("Gliding", true);
                }
            }
        }

        if (animator.GetParameter(6).name == "Dash")
        {
            if (ctx.performed && canGlide && gliding)
            {
                gliding = true;
                animator.SetBool("Dash", true);
            }
            else if (ctx.canceled)
            {
                gliding = false;
                animator.SetBool("Dash", false);
            }
            else if (canGlide)
            {
                stamina -= 5;
                rb.AddForce(Vector3.up * (playerMovement.jumpForce * 0.3f), ForceMode.VelocityChange);
                if (transform.localScale.x == 1)
                {
                    rb.AddForce(Vector3.right * (playerMovement.jumpForce * 1f), ForceMode.VelocityChange);
                }
                else
                {
                    rb.AddForce(Vector3.left * (playerMovement.jumpForce * 1f), ForceMode.VelocityChange);
                }

                gliding = true;
                canGlide = false;
                animator.SetBool("Dash", true);
            }
        }

        if (animator.GetParameter(6).name == "Stomp")
        {
            if (!playerMovement.grounded)
            {
                if (ctx.performed && canGlide)
                {
                    gliding = true;
                    animator.SetBool("Stomp", true);
                }
                /*else if (ctx.canceled)
                {
                    gliding = false;
                    animator.SetBool("Stomp", false);
                }*/
                else if (canGlide)
                {
                    stamina -= 5;
                    rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX;
                    rb.AddForce(Vector3.down * (playerMovement.jumpForce * 0.8f), ForceMode.VelocityChange);
                    gliding = true;
                    canGlide = false;
                    animator.SetBool("Stomp", true);
                }
            }
        }
    }

    public bool IsDashing()
    {
        if (animator.GetParameter(6).name == "Dash" && gliding)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
