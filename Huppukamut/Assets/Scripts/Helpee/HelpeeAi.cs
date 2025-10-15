using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.AI.Navigation;

public enum OffMeshLinkMoveMethod
{
    Teleport,
    NormalSpeed,
    Parabola
}

public class HelpeeAi : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public OffMeshLinkMoveMethod method = OffMeshLinkMoveMethod.Parabola;
    public float jumpDuration = 1.5f;
    public GameObject goal;
    private NavMeshAgent agent;

    //private GameObject[] nodes;
    //private float moveTimer = 1f;
    //private float movingSpeed = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        //nodes = GameObject.FindGameObjectsWithTag("PathNode");
        //int rand = UnityEngine.Random.Range(0, nodes.Length);
        //agent.destination = nodes[rand].transform.position;

        agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.transform.position;

        agent.autoTraverseOffMeshLink = true;
        while (true)
        {
            if (agent.isOnOffMeshLink)
            {
                if (method == OffMeshLinkMoveMethod.NormalSpeed)
                    yield return StartCoroutine(NormalSpeed(agent));
                else if (method == OffMeshLinkMoveMethod.Parabola)
                    yield return StartCoroutine(Parabola(agent, 5.0f, jumpDuration));
                agent.CompleteOffMeshLink();
            }
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //agent.SetDestination(target.transform.position);
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
