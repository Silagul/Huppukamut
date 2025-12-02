using UnityEngine;

public class BoxBreakEffect : MonoBehaviour
{
    public GameObject particleEffect;
    public GameObject spawnObject;
    public float particleLifetime = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

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
