using UnityEngine;
using UnityEngine.TextCore.Text;

public class ItemDistribution : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    public GameObject[] itemPositions;
    public float budget;

    private GameObject[] items = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemPositions = GameObject.FindGameObjectsWithTag("ItemPosition");
        float maxValuePerItem = budget / itemPositions.Length;
        float totalValue = 0;
        
        int a = 0;
        foreach (GameObject item in itemPrefabs)
        {
            float b = item.GetComponent<Item>().budgetUsage;
            if (b < maxValuePerItem)
            {
                a++;
            }
        }

        items = new GameObject[a];
        int c = 0;
        foreach (GameObject item in itemPrefabs)
        {
            float b = item.GetComponent<Item>().budgetUsage;
            if (b < maxValuePerItem)
            {
                items[c] = item;
                c++;
            }
        }

        /*
        GameObject holder = null;
        float largest = 0;

        foreach (GameObject item in items)
        {
            float b = item.GetComponent<Item>().budgetUsage;
            if (b > largest)
            {
                holder = item;
                largest = b;
            }
        }
        float x = budget / largest;
        */

        for (int p = 0; p < itemPositions.Length; p++)
        {
            for (float i = budget; i > 0;)
            {
                int z = (int)Mathf.Ceil(Random.Range(0.1f, items.Length - 1));
                Item b = items[z].GetComponent<Item>();
                if (i >= b.budgetUsage)
                {
                    Instantiate(items[z], itemPositions[p].transform.position, Quaternion.identity);
                    i -= b.budgetUsage;
                    totalValue += b.budgetUsage;
                    break;
                }
                else
                {
                    continue;
                }
            }
        }
        print("Leftover budget: " + (budget - totalValue));
    }

    // Update is called once per frame
    void Update()
    {
        //
    }
}
