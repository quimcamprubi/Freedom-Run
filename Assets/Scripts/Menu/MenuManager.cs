using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void ButtonStart()
    {
        string levelToLoad = PlayerPrefs.GetString("LevelProgress");
        if (levelToLoad == "") {
            SceneManager.LoadScene("Nivell_1");
        }
        else {
            SceneManager.LoadScene(levelToLoad);
        }
    }

    public void ButtonQuit()
    {
        Debug.Log("GAME CLOSE (on unity debug doesn't work)");
        Application.Quit();
    }
}