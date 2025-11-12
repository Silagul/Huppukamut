using UnityEngine;

public class CharacterMoveAnimations : MonoBehaviour
{
    public HelpeeAi helpee = null;
    public PlayerMovement playerMovement = null;
    public PlayerStamina playerStamina = null;
    public Animator animator;
    public Rigidbody rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = transform.GetComponentInChildren<Animator>();

        if (TryGetComponent<HelpeeAi>(out HelpeeAi h))
        {
            helpee = h;
        }

        if (TryGetComponent<PlayerMovement>(out PlayerMovement pm))
        {
            playerMovement = pm;
            playerStamina = GetComponent<PlayerStamina>();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (playerMovement != null)
        {
            animator.SetFloat("VertVel", Mathf.InverseLerp(-12f, 12f, rb.linearVelocity.y * -1));
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x) / playerMovement.moveSpeed);
            animator.SetBool("InAir", !playerMovement.grounded);
            animator.SetFloat("Stamina", playerStamina.stamina / playerStamina.maxStamina);
        }
        else if (helpee != null)
        {
            animator.SetFloat("VertVel", Mathf.InverseLerp(-9f, 9f, helpee.agent.velocity.y * -1));
            animator.SetFloat("Speed", Mathf.Abs(helpee.agent.velocity.x) / helpee.agent.speed);
            animator.SetBool("InAir", helpee.agent.isOnOffMeshLink);
            animator.SetFloat("Stamina", helpee.stamina / helpee.maxStamina);

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
