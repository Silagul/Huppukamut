using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject helpMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenu = GameObject.Find("PauseMenu");
        helpMenu = GameObject.Find("InGame Help Window");
        pauseMenu.SetActive(false);
        helpMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //
    }

    public void Pause(InputAction.CallbackContext ctx)
    {
        pauseMenu.SetActive(true);
        helpMenu.SetActive(false);
        Time.timeScale = 0f;
        timer.Instance.Pause();
    }

    public void PauseFunction()
    {
        pauseMenu.SetActive(true);
        helpMenu.SetActive(false);
        Time.timeScale = 0f;
        timer.Instance.Pause();
    }

        public void HelpFunction()
    {
        helpMenu.SetActive(true);
        Time.timeScale = 0f;
        timer.Instance.Pause();
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        timer.Instance.Resume();
    }
}
