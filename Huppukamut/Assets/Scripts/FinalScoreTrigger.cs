using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalScoreTrigger : MonoBehaviour
{
    public string endSceneName = "EndScene";  // Vaihda oman scenen nimeen

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ? Tallennetaan score ScoreManagerilta ?
            if (ScoreManager.instance != null)
            {
                PlayerPrefs.SetInt("FinalScore", ScoreManager.instance.CurrentScore);
                PlayerPrefs.Save();
                Debug.Log("Tallennettu score: " + ScoreManager.instance.CurrentScore);
            }
            else
            {
                Debug.LogError("ScoreManager.instance on NULL - ei löytynyt sceneltä!");
            }

            // ? Vaihdetaan scena ?
            SceneManager.LoadScene(endSceneName);
        }
    }
}
