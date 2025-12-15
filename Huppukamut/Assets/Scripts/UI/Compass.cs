//using UnityEditor;
//using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public GameObject goal;
    public GameObject target;
    public GameObject player;
    private GameObject needle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        goal = GameObject.FindGameObjectWithTag("Finish");
        needle = GameObject.Find("Needle");
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //GameObject target = goal;
        target = FindClosestTagged("Helpee");

        Vector3 targetDirection = target.transform.position - player.transform.position;

        float angle = Vector3.Angle(new Vector3(targetDirection.x, targetDirection.y, 0), Vector3.up);

        if (target.transform.position.x > player.transform.position.x)
        {
            angle *= -1;
        }

        needle.GetComponent<RectTransform>().SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, angle));
    }

    public GameObject FindClosestTagged(string tag)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = player.transform.position;

        foreach (GameObject go in gos)
        {
            if (go.TryGetComponent<HelpeeAi>(out HelpeeAi h))
            {
                Vector3 diff = go.transform.position - position;    // Vector from this to target
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance && h.stamina <= 0)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
        }
        if (closest == null)
        {
            return goal;
        }
        else
        {
            return closest;
        }
    }
}
