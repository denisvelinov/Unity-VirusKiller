using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    public Piece piece;
    public void Setup()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        gameObject.SetActive(false);
        piece.isPaused = false;
        Time.timeScale = 1;
    }

    public void Quit()
    {
        LoadGameValues.virusLevel = 0;
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
