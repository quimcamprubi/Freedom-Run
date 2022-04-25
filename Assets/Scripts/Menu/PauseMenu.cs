using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void OnEnable()
    {
        Time.timeScale = 0f;
    }

    public void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
