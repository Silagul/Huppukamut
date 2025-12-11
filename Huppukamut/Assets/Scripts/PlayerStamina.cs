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
    public bool gliding = false;

    private Rigidbody rb;
    private GameObject skillCooldown;
    private GameObject dashIcon;
    private GameObject glideIcon;
    private GameObject stompIcon;
    private GameObject maskImage;
    private float dashTimer = 2f;
    private bool canGlide = true;
    private bool canRecharge = true;
    private float skillCooldownTimer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        stamina = maxStamina;

        skillCooldown = GameObject.Find("Skill Cooldown");
        dashIcon = skillCooldown.transform.Find("Dash").gameObject;
        glideIcon = skillCooldown.transform.Find("Glide").gameObject;
        stompIcon = skillCooldown.transform.Find("Stomp").gameObject;
        maskImage = GameObject.Find("Skill cooldown bar");

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

        switch (animator.GetParameter(6).name)
        {
            case "Gliding":
                dashIcon.SetActive(false);
                stompIcon.SetActive(false);
                break;
            case "Dash":
                glideIcon.SetActive(false);
                stompIcon.SetActive(false);
                break;
            case "Stomp":
                glideIcon.SetActive(false);
                dashIcon.SetActive(false);
                break;
        }
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
            canRecharge = true;
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
        
        if (skillCooldownTimer > 0 && canGlide == false && canRecharge)
        {
            skillCooldownTimer -= Time.deltaTime;
        }
        if (skillCooldownTimer <= 0)
        {
            canGlide = true;
        }
        UpdateSkillCooldownVisual(skillCooldownTimer);
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
                helpee.canvas.SetActive(false);
                helpee.moving = true;
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
                    canRecharge = false;
                    animator.SetBool("Gliding", true);
                    skillCooldownTimer = skillCooldownTime;
                }
            }
        }

        if (animator.GetParameter(6).name == "Dash")
        {
            if (ctx.performed && gliding && canGlide)
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
                    rb.AddForce(Vector3.right * (playerMovement.jumpForce * 0.8f), ForceMode.VelocityChange);
                }
                else
                {
                    rb.AddForce(Vector3.left * (playerMovement.jumpForce * 0.8f), ForceMode.VelocityChange);
                }

                gliding = true;
                canGlide = false;
                canRecharge = false;
                animator.SetBool("Dash", true);
                skillCooldownTimer = skillCooldownTime;
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
                    rb.AddForce(Vector3.down * (playerMovement.jumpForce * 1f), ForceMode.VelocityChange);
                    gliding = true;
                    canGlide = false;
                    canRecharge = false;
                    animator.SetBool("Stomp", true);
                    skillCooldownTimer = skillCooldownTime;
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

    public void UpdateSkillCooldownVisual(float timer)
    {
        float normalized = (Mathf.Clamp01(timer / skillCooldownTime));

        // Update fill amount on the parent Image (the mask)
        maskImage.GetComponent<RectTransform>().SetLocalPositionAndRotation(new Vector3(0, normalized * -100, 0), Quaternion.Euler(0, 0, 0));
    }
}
