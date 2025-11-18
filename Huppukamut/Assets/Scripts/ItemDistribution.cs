using UnityEngine;
using UnityEngine.TextCore.Text;

public class ItemDistribution : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    public GameObject[] itemPositions;
    public float budget;

    private float totalValue = 0;
    private GameObject temp = null;
    private GameObject[] filled;
    private GameObject[] items = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemPositions = GameObject.FindGameObjectsWithTag("ItemPosition");
        filled = new GameObject[itemPositions.Length];
        for (int o = 0; o < filled.Length; o++)
        {
            filled[o] = null;
        }

        //FirstVersion();
        SecondVersion();

        print("Leftover budget: " + (budget - totalValue));
    }

    // Update is called once per frame
    void Update()
    {
        //
    }

    public void FirstVersion()
    {
        float maxValuePerItem = budget / itemPositions.Length;

        int a = 0;
        foreach (GameObject item in itemPrefabs)
        {
            float b = item.GetComponent<Item>().budgetUsage;
            if (b <= maxValuePerItem)
            {
                a++;
            }
        }

        items = new GameObject[a];
        int c = 0;
        foreach (GameObject item in itemPrefabs)
        {
            float b = item.GetComponent<Item>().budgetUsage;
            if (b <= maxValuePerItem)
            {
                items[c] = item;
                c++;
            }
        }

        for (int p = 0; p < itemPositions.Length; p++)
        {
            for (float i = budget; i > 0;)
            {
                int z = (int)Mathf.Ceil(Random.Range(0f, items.Length - 1));
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
    }

    public void SecondVersion()
    {
        // Step 1: Find the item with the largest budgetUsage.
        temp = FindLargestValueBelow(itemPrefabs, budget);

        for (float i = budget; i > 0;)
        {
            // Step 2: Divide budget by budgetUsage to determine the maximum number of times that item can appear in the level. If that number is smaller than itemPositions.Length, skip to step 5.
            if (temp != null)
            {
                float m = temp.GetComponent<Item>().budgetUsage;
                float x = budget / m;
                if (x < itemPositions.Length)
                {
                    temp = FindLargestValueBelow(itemPrefabs, m);
                }

                // Step 3: Choose a random number between 1 and that number.
                int z = (int)Mathf.Floor(Random.Range(1.1f, x));

                // Step 4: Place that many of that item in the level. Reduce from budget the total budgetUsage of the placed items.
                for (int n = 0; n < z;)
                {
                    bool isFilled = false;
                    int p = (int)Mathf.Ceil(Random.Range(0f, itemPositions.Length - 1));
                    print(p);
                    for (int o = 0; o < filled.Length; o++)
                    {
                        if (filled[o] == itemPositions[p])
                        {
                            isFilled = true;
                        }
                    }
                    if (isFilled == false)
                    {
                        for (int o = 0; o < filled.Length; o++)
                        {
                            if (filled[o] == null)
                            {
                                filled[o] = itemPositions[p];
                                Instantiate(temp, itemPositions[p].transform.position, Quaternion.identity);
                                budget -= m;
                                totalValue += m;
                                n++;
                                break;
                            }
                        }
                    }
                }
                // Step 5: Find the item with the next largest budgetUsage.
                temp = FindLargestValueBelow(itemPrefabs, m);
            }
            else
            {
                break;
            }
        }

        // Step 6: Loop steps 2-5 until budget is zero.
    }

    public GameObject FindLargestValueBelow(GameObject[] items, float previousMax)
    {
        GameObject holder = null;
        float largest = 0;

        foreach (GameObject item in items)
        {
            float b = item.GetComponent<Item>().budgetUsage;
            if (b > largest && b < previousMax)
            {
                holder = item;
                largest = b;
            }
        }
        return holder;
    }

    public int CountFreeSlots()
    {
        int amount = 0;
        for (int o = 0; o < filled.Length; o++)
        {
            if (filled[o] == null)
            {
                amount++;
            }
        }
        return amount;
    }
}
