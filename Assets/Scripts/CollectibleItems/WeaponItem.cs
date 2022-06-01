using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CollectibleItems
{
    public class WeaponItem : RegularItem
    {
        public bool canTeleport;
        public String sceneDestination;

        public void Teleport() {
            PlayerPrefs.SetString("LevelProgress", sceneDestination);
            SceneManager.LoadScene(sceneDestination);
        }
    }
}