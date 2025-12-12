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
    private ItemDistribution itemDistribution;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        current = 0;
        transform.localScale = new Vector3(size, size, size);
        itemDistribution = GameObject.Find("Positions").GetComponent<ItemDistribution>();
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
            /*if (playerStamina.stamina <= playerStamina.maxStamina - recoveryAmount)
            {
                //
            }*/
            playerStamina.stamina += recoveryAmount;
            if (playerStamina.stamina > playerStamina.maxStamina)
            {
                playerStamina.stamina = playerStamina.maxStamina;
            }
            ScoreManager.instance.AddPoint(250);
            
            // === SOUND ===
            if (SoundManager.instance != null)
                SoundManager.instance.PlayCollect();
            
            GameObject effect = Instantiate(particleEffect, transform.position, Quaternion.identity);
            itemDistribution.IncrementScore();
            Destroy(gameObject);
        }
    }
}