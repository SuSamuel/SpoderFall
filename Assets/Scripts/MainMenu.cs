using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // call when start dungeon button is clicked
    public void StartPrototype()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // call when start building button is clicked
    public void StartDemo()
    {
        SceneManager.LoadScene("SecondMap");
    }

    // call when quit button is clicked
    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
