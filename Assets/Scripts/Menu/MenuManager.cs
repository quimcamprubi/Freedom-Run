using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void ButtonStart()
    {
        var levelToLoad = PlayerPrefs.GetString("LevelProgress");
        if (levelToLoad == "")
            SceneManager.LoadScene("Start_Scene");
        else
            SceneManager.LoadScene(levelToLoad);
    }

    public void ButtonQuit()
    {
        Debug.Log("GAME CLOSE (on unity debug doesn't work)");
        Application.Quit();
    }

    public void DeleteProgress()
    {
        PlayerPrefs.SetString("LevelProgress", "Start_Scene");
    }
}