using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void ButtonStart()
    {
        SceneManager.LoadScene("Nivell_1");
    }

    public void ButtonQuit()
    {
        Debug.Log("GAME CLOSE (on unity debug doesn't work)");
        Application.Quit();
    }
}