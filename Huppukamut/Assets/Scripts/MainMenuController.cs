using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    public Transform cam;
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject creditsMenu;
    public GameObject current;
    public Transform target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main.transform;
        mainMenu = GameObject.Find("MainMenu");
        settingsMenu = GameObject.Find("SettingsMenu");
        creditsMenu = GameObject.Find("CreditsMenu");
        target = mainMenu.transform.Find("CameraTarget");
        current = mainMenu;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 diff = cam.position - target.position;

        // Move our position a step closer to the target.
        float step = 40f * Time.deltaTime; // calculate distance to move
        cam.position = Vector3.MoveTowards(cam.position, target.position, step);
        //cam.LookAt(current.transform.position);
        /*
        Vector3 targetDirection = current.transform.position - cam.position;
        float singleStep = 1f * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(cam.forward, targetDirection, singleStep, 0.0f);
        cam.rotation = Quaternion.LookRotation(newDirection);*/
    }

    public void GhangeActiveMenu(GameObject menu)
    {
        target = menu.transform.Find("CameraTarget");
        cam.rotation = menu.transform.rotation;
        current = menu;
    }
}
