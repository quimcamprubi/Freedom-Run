using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void ButtonStart()
    {
        SceneManager.LoadScene("Start_Scene");
    }

    public void ButtonQuit()
    {
        Debug.Log("GAME CLOSE (on unity debug doesn't work)");
        Application.Quit();
    }
}