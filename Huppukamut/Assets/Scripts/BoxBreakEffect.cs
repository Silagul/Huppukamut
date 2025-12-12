using UnityEngine;

public class BoxBreakEffect : MonoBehaviour
{
    public GameObject particleEffect;
    public GameObject spawnObject;
    public float particleLifetime = 2f;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerStamina>(out PlayerStamina ps))
        {
            if (ps.gliding)
            {
                Debug.Log("BREAK BOX");

                if (particleEffect)
                {
                    GameObject fx = Instantiate(particleEffect, transform.position, transform.rotation);
                    Destroy(fx, particleLifetime);
                }

                if (spawnObject)
                {
                    Instantiate(spawnObject, transform.position, transform.rotation);
                }

                Destroy(gameObject);
            }
        }
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        //if (!other.CompareTag("Player")) return;
        if (collision.collider.gameObject.TryGetComponent<PlayerStamina>(out PlayerStamina ps))
        {
            if (ps.gliding)
            {
                Debug.Log("BREAK BOX");

                if (particleEffect)
                {
                    GameObject fx = Instantiate(particleEffect, transform.position, transform.rotation);
                    Destroy(fx, particleLifetime);
                }

                if (spawnObject)
                {
                    Instantiate(spawnObject, transform.position, transform.rotation);
                }

                Destroy(gameObject);
            }
        }
    }*/
}
