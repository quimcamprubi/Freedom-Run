using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Go_to_level_collider : MonoBehaviour
{
    public string levelscene = null;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (levelscene != null){
            SceneManager.LoadScene(levelscene);
        }
        
    }

    
}
