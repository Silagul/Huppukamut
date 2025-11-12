using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public GameObject goal;
    public GameObject needle;
    public GameObject player;

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
        Vector3 targetDirection = goal.transform.position - player.transform.position;
        float angle = Vector3.Angle(new Vector3(targetDirection.x, targetDirection.y, 0), Vector3.up);
        if (goal.transform.position.x > player.transform.position.x)
        {
            angle *= -1;
        }

        needle.GetComponent<RectTransform>().SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, angle));
    }
}
