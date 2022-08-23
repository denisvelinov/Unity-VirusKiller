using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseGame : MonoBehaviour
{
    public void Setup()
    {
        gameObject.SetActive(true);
    }

    public void PlayAgainButton()
    {
        SceneManager.LoadScene("TestLevel");
    }

    public void BackButton()
    {
        LoadGameValues.virusLevel = 0;
        SceneManager.LoadScene("MainMenu");
    }
}
