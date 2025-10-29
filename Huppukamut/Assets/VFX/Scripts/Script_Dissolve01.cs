using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Dissolve01 : MonoBehaviour
{
    public float DissolveDuration = 2;
    public float DissolveStrength;

    public void StartDissolver()
    {
        StartCoroutine(Dissolver());
    }

    public IEnumerator Dissolver()
    {
        float ElapsedTime = 0;

        Material M_Smoke01 = GetComponent<Renderer>().material;

        while (ElapsedTime < DissolveDuration)
        {
            ElapsedTime += Time.deltaTime;

            DissolveStrength = Mathf.Lerp(0, 1, ElapsedTime / DissolveDuration);
            M_Smoke01.SetFloat("_VoronoiScale", DissolveStrength);

            yield return null;
        }
    }

    private void Start()
    {
        StartDissolver();
    }
}