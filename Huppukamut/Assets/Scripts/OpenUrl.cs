using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenUrl : MonoBehaviour
{
    public void OpenWebsite(string url = "https://www.sekasingaming.fi/")
    {
        Application.OpenURL(url);
    }
}
