using TMPro;
using UnityEditor;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    public Transform cam;
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject creditsMenu;
    public GameObject linksMenu;
    public GameObject current;
    public Transform target;
    public float moveStep;
    public float rotateStep;

    //private Quaternion targetRotation = new Quaternion(0, 0, 0, 1);
    //private float ang = 0f;
    private float distance = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main.transform;
        mainMenu = GameObject.Find("MainMenu");
        settingsMenu = GameObject.Find("SettingsMenu");
        creditsMenu = GameObject.Find("CreditsMenu");
        linksMenu = GameObject.Find("LinksMenu");

        GameObject[] menus = { mainMenu, settingsMenu, creditsMenu, linksMenu };
        foreach (GameObject menu in menus)
        {
            menu.transform.Find("Canvas").gameObject.SetActive(false);
        }
        current = mainMenu;
        target = current.transform.Find("CameraTarget");
        ChangeActiveMenu(current);
    }

    // Update is called once per frame
    void Update()
    {
        // Move our position a step closer to the target.
        Vector3 targetDirection = current.transform.position - cam.position;
        Vector3 targetDirection2 = target.transform.position - cam.position;
        //float step = moveStep * Time.deltaTime; // calculate distance to move
        float step = (distance + 10) * Time.deltaTime; // calculate distance to move
        cam.position = Vector3.MoveTowards(cam.position, target.position, step);

        //float ang2 = Quaternion.Angle(cam.rotation, current.transform.rotation);
        float singleStep = rotateStep * Time.deltaTime;
        if (current == mainMenu)
        {
            if (targetDirection2.sqrMagnitude <= 121)
            {
                Vector3 newDirection = Vector3.RotateTowards(cam.forward, targetDirection, singleStep * 1.5f, 0.0f);
                cam.rotation = Quaternion.LookRotation(newDirection);
            }
        }
        else
        {
            Vector3 newDirection = Vector3.RotateTowards(cam.forward, targetDirection, singleStep, 0.0f);
            cam.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    public void ChangeActiveMenu(GameObject menu)
    {
        GameObject[] menus = { mainMenu, settingsMenu, creditsMenu, linksMenu };
        foreach (GameObject m in menus)
        {
            if (m != menu)
            {
                m.transform.Find("Canvas").gameObject.SetActive(false);
            }
            else
            {
                m.transform.Find("Canvas").gameObject.SetActive(true);
            }
        }
        //ang = Quaternion.Angle(cam.rotation, menu.transform.rotation);
        current = menu;
        target = menu.transform.Find("CameraTarget");
        Vector3 targetDirection = target.transform.position - cam.position;
        distance = targetDirection.magnitude;

        //print(ang + " degrees");
        //print(distance + " units length");
    }
}
