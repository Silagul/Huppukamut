using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public enum OffMeshLinkMoveMethod
{
    Teleport,
    NormalSpeed,
    Parabola
}

public class HelpeeAi : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Jump")] // This adds a header to the editor view.
    public OffMeshLinkMoveMethod method = OffMeshLinkMoveMethod.Parabola;
    public float jumpDuration = 1.5f;
    public float jumpHeight = 5.0f;

    [Header("Stamina")]
    public float maxStamina;
    public float stamina;
    public float staminaDecayRate;
    public bool moving = false;

    [Header("References")]
    public GameObject player;
    public GameObject goal;

    public NavMeshAgent agent;
    public GameObject canvas;
    private CapsuleCollider capsuleCollider;
    private Rigidbody rb;
    private float timer = 2f;
    private bool ticking = false;

    //private GameObject[] goals;
    //private float moveTimer = 1f;
    //private float movingSpeed = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        //goals = GameObject.FindGameObjectsWithTag("HelpeeGoal");
        //int rand = UnityEngine.Random.Range(0, nodes.Length);
        //agent.destination = FindClosestTagged("HelpeeGoal");
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
        goal = GameObject.Find("Helpee Goal");
        capsuleCollider = GetComponent<CapsuleCollider>();
        canvas = GetComponentInChildren<Canvas>().gameObject;
        agent = GetComponent<NavMeshAgent>();

        capsuleCollider.excludeLayers = LayerMask.GetMask("Default");

        agent.autoTraverseOffMeshLink = true;
        while (true)
        {
            if (agent.isOnOffMeshLink)
            {
                if (method == OffMeshLinkMoveMethod.NormalSpeed)
                    yield return StartCoroutine(NormalSpeed(agent));
                else if (method == OffMeshLinkMoveMethod.Parabola)
                    yield return StartCoroutine(Parabola(agent, jumpHeight, jumpDuration));
                agent.CompleteOffMeshLink();
            }
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetDirection = goal.transform.position - transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        if (!moving)
        {
            agent.enabled = false;
        }

        if (agent.enabled)
        {
            if (!agent.isOnOffMeshLink && stamina > 0 && agent.remainingDistance > 1)
            {
                stamina -= Time.deltaTime * staminaDecayRate;
            }

            if (stamina <= 0)
            {
                SetDestination(gameObject);
                moving = false;
                canvas.SetActive(true);
            }

            if (moving == true)
            {
                if (targetDirection.sqrMagnitude <= 4)
                {
                    agent.isStopped = true;
                    rb.constraints = RigidbodyConstraints.FreezePositionX;
                    if (!ticking)
                    {
                        transform.GetComponentInChildren<Animator>().SetTrigger("Pose");
                        HelpeeTracker helpeeTracker = GameObject.Find("Autettavat").GetComponent<HelpeeTracker>();
                        helpeeTracker.HelpeeRescued(gameObject);
                        ScoreManager.instance.AddPoint(2000);
                        ticking = true;
                    }
                }
            }
        }

        if (ticking)
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 0)
        {
            Destroy(gameObject);
        }

        /*if (rb.linearVelocity.x != 0f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(rb.linearVelocity.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }*/
    }

    public void SetDestination(GameObject destination)
    {
        agent.destination = destination.transform.position;
    }

    IEnumerator NormalSpeed(NavMeshAgent agent)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        while (agent.transform.position != endPos)
        {
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator Parabola(NavMeshAgent agent, float height, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }
}
