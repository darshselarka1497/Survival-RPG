using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject tutorial;

    public void quitGame()
    {
        Application.Quit();
    }
    public void displayTutorial()
    {
        tutorial.SetActive(true);
    }
    public void exitTutorial()
    {
        tutorial.SetActive(false);
    }
}
