using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public MainMenu mainMenu;
    public GameObject DifficultyToggles;

    [SerializeField] private Slider slider;
    [SerializeField] private Text sliderValue;
    public void Setup()
    {
        gameObject.SetActive(true);
        //DifficultyToggles.transform.GetChild((int)LoadGameValues.Dificulty).GetComponent<Toggle>().isOn = true;
    }

    public void MainMenu()
    {
        gameObject.SetActive(false);
        mainMenu.Setup();
    }

    public void PlaytGame()
    {
        SceneManager.LoadScene("TestLevel");
    }

    public void SetVirusLevel()
    {
        sliderValue.text = slider.value.ToString();
        LoadGameValues.virusLevel = (int)slider.value;
    }

    public void SetSlowGameSpeed()
    {
        LoadGameValues.Dificulty = LoadGameValues.Dificulties.Slow;
    }

    public void SetMediumGameSpeed()
    {
        LoadGameValues.Dificulty = LoadGameValues.Dificulties.Medium;
    }

    public void SetFastGameSpeed()
    {
        LoadGameValues.Dificulty = LoadGameValues.Dificulties.Fast;
    }
}
