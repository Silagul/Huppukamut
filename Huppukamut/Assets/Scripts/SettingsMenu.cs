

using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject SettingsMenuUI;

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ResumeGame()
    {
        SettingsMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }




    public void QuitGame()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "WinScreen")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SettingsMenuUI.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }
}

