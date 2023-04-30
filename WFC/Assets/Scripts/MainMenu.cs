using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Main menu UI
/// </summary>
public class MainMenu : MonoBehaviour
{
    public void LoadWorldMap()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadDungeon()
    {
        SceneManager.LoadScene(2);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
