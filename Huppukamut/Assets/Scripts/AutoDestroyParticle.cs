using UnityEngine;

public class AutoDestroyParticle : MonoBehaviour
{
    [Tooltip("Maximum lifetime in seconds (will destroy even if particles are still playing)")]
    public float maxLifetime = 10f;

    void OnEnable()
    {
        Destroy(gameObject, maxLifetime);
    }
}