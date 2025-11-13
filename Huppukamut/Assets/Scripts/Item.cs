using UnityEngine;

public class Item : MonoBehaviour
{
    public float movement;
    public float speed;
    public float recoveryAmount;
    public float size;
    public float budgetUsage;
    public GameObject particleEffect;

    private float current;
    private bool upward = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        current = 0;
        transform.localScale = new Vector3(size, size, size);
    }

    // Update is called once per frame
    void Update()
    {
        if (upward)
        {
            current += Time.deltaTime * speed;
        }
        else
        {
            current -= Time.deltaTime * speed;
        }
        
        if (Mathf.Abs(current) > movement / 2)
        {
            if (current >= 0 && upward)
            {
                upward = false;
            }

            if (current <= 0 && !upward)
            {
                upward = true;
            }
        }

        transform.Translate(Vector3.up * current * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerStamina>(out PlayerStamina playerStamina))
        {
            playerStamina.stamina += recoveryAmount;
            GameObject effect = Instantiate(particleEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
