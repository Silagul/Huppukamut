using UnityEngine;
using UnityEngine.TextCore.Text;

public class ItemDistribution : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    public GameObject[] itemPositions;
    public float budget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemPositions = GameObject.FindGameObjectsWithTag("ItemPosition");
        float maxValuePerItem = budget / itemPositions.Length;

        for (float i = budget; i > 0;)
        {
            int z = (int)Mathf.Ceil(Random.Range(0.1f, 7f));
            Item b = itemPrefabs[z].GetComponent<Item>();
            if (i >= b.budgetUsage && b.budgetUsage <= maxValuePerItem)
            {
                //Instantiate(itemPrefabs[z], itemPositions[?].transform.position, Quaternion.identity);
                i -= b.budgetUsage;
                break;
            }
            else
            {
                continue;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //
    }
}
