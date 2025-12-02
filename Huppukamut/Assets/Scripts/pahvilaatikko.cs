using UnityEngine;

public class pahvilaatikko : MonoBehaviour
{
    private void OnMouseDown()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Laatikko")
        {
            //Destroy(gameObject );
            Destroy(collision.gameObject);
        }
    }

}