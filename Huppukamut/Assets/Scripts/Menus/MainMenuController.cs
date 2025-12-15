/*using TMPro;
using UnityEditor;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine.UIElements;*/
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Transform cam;
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject creditsMenu;
    public GameObject linksMenu;
    public GameObject current;
    public Transform target;

    // === NEW: Smooth camera settings (copied from first script) ===
    [Header("Smooth Camera Animation")]
    public float moveDuration = 0.8f;
    public float rotationDuration = 0.75f;
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // Internal smooth transition state
    private Vector3 camStartPos;
    private Quaternion camStartRot;
    private Vector3 camTargetPos;
    private Quaternion camTargetRot;
    private float transitionTimer = 0f;
    private bool isTransitioning = false;

    void Start()
    {
        cam = Camera.main.transform;
        mainMenu = GameObject.Find("MainMenu");
        settingsMenu = GameObject.Find("SettingsMenu");
        creditsMenu = GameObject.Find("CreditsMenu");
        linksMenu = GameObject.Find("LinksMenu");

        // Hide all canvases
        GameObject[] menus = { mainMenu, settingsMenu, creditsMenu, linksMenu };
        foreach (GameObject menu in menus)
        {
            var canvas = menu.transform.Find("Canvas")?.gameObject;
            if (canvas != null) canvas.SetActive(false);
        }

        // Start on main menu with instant positioning
        current = mainMenu;
        target = current.transform.Find("CameraTarget");
        ChangeActiveMenu(current, instant: true);
    }

    void Update()
    {
        if (!isTransitioning) return;

        transitionTimer += Time.deltaTime;

        float moveT = Mathf.Clamp01(transitionTimer / moveDuration);
        float rotT = Mathf.Clamp01(transitionTimer / rotationDuration);

        // Smooth position with curve
        cam.position = Vector3.Lerp(camStartPos, camTargetPos, moveCurve.Evaluate(moveT));
        // Smooth rotation with curve
        cam.rotation = Quaternion.Slerp(camStartRot, camTargetRot, rotationCurve.Evaluate(rotT));

        // Finish when both are done
        if (moveT >= 1f && rotT >= 1f)
        {
            isTransitioning = false;
        }
    }

    public void ChangeActiveMenu(GameObject menu)
    {
        ChangeActiveMenu(menu, instant: false);
    }

    // Overloaded version so Start() can snap instantly
    private void ChangeActiveMenu(GameObject menu, bool instant)
    {
        GameObject[] menus = { mainMenu, settingsMenu, creditsMenu, linksMenu };
        foreach (GameObject m in menus)
        {
            var canvas = m.transform.Find("Canvas")?.gameObject;
            if (canvas != null)
                canvas.SetActive(m == menu);
        }

        current = menu;
        target = menu.transform.Find("CameraTarget");

        if (instant)
        {
            // Snap camera immediately (used on Start)
            cam.position = target.position;
            cam.rotation = target.rotation;
            isTransitioning = false;
        }
        else
        {
            // Start smooth transition
            camStartPos = cam.position;
            camStartRot = cam.rotation;
            camTargetPos = target.position;
            camTargetRot = target.rotation;

            transitionTimer = 0f;
            isTransitioning = true;
        }
    }

    public void LoadLevel(string name)
    {
        SceneManager.LoadScene(name);
    }
}