using UnityEngine;

public class CharacterMoveAnimations : MonoBehaviour
{
    public HelpeeAi helpee = null;
    public PlayerMovement playerMovement = null;
    public PlayerStamina playerStamina = null;
    public Animator animator = null;
    public Rigidbody rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (TryGetComponent<HelpeeAi>(out HelpeeAi h))
        {
            helpee = h;
            animator = transform.GetComponentInChildren<Animator>();
        }

        if (TryGetComponent<PlayerMovement>(out PlayerMovement pm))
        {
            playerMovement = pm;
            playerStamina = GetComponent<PlayerStamina>();
            animator = playerStamina.animator;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement != null)
        {
            if (animator == null)
            {
                animator = playerStamina.animator;
            }
            else
            {
                animator.SetFloat("VertVel", Mathf.InverseLerp(-12f, 12f, rb.linearVelocity.y * -1));
                animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x) / playerMovement.moveSpeed);
                animator.SetBool("InAir", !playerMovement.grounded);
                animator.SetFloat("Stamina", playerStamina.stamina / playerStamina.maxStamina);
            }
        }
        else if (helpee != null)
        {
            if (animator == null)
            {
                animator = transform.GetComponentInChildren<Animator>();
            }
            else
            {
                animator.SetFloat("VertVel", Mathf.InverseLerp(-9f, 9f, helpee.agent.velocity.y * -1));
                animator.SetFloat("Speed", Mathf.Abs(helpee.agent.velocity.x) / helpee.agent.speed);
                animator.SetBool("InAir", helpee.agent.isOnOffMeshLink);
                animator.SetFloat("Stamina", helpee.stamina / helpee.maxStamina);
            }

            if (helpee.stamina == 0)
            {
                animator.SetBool("Sad", true);
            }
            else
            {
                animator.SetBool("Sad", false);
            }
        }
    }
}
