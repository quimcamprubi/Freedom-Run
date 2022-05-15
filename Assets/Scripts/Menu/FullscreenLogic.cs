using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullscreenLogic : MonoBehaviour
{
    public Toggle toggle;

    // Start is called before the first frame update
    private void Start()
    {
        if (Screen.fullScreen)
            toggle.isOn = true;
        else
            toggle.isOn = false;
    }

    public void ActivateFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}