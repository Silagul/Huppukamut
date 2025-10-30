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

    [Header("References")]
    public GameObject player;
    public GameObject goal;
    private NavMeshAgent agent;

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
        agent = GetComponent<NavMeshAgent>();

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
        Vector3 dist = player.transform.position - transform.position;
        if(dist.magnitude <= 2)
        {
            agent.destination = goal.transform.position;
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
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
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
