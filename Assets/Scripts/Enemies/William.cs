using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class William : MonoBehaviour
{
    // Start is called before the first frame update
	public GameObject williams;
	public GameObject porron;
    public string sceneDestination;


    // Update is called once per frame
    void Update()
    {
        //if (!porron.activeSelf){ williams.SetActive(true); }
        bool mort = false;
        if (porron.activeSelf)
        {
            williams.SetActive(false);
        } 
        else {
            williams.SetActive(true);
            mort = true;
        }
        if(mort == true)
        {
            if (williams.activeSelf == null)
            {
                PlayerPrefs.SetString("LevelProgress", sceneDestination);
                SceneManager.LoadScene(sceneDestination);
            }
        }
    }
	
}
