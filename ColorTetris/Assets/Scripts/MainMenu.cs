using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public StartGame startGame;
    public void Setup()
    {
        gameObject.SetActive(true);
    }

    public void StartGame()
    {
        gameObject.SetActive(false);
        startGame.Setup();
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game!");
        Application.Quit();
    }
}
