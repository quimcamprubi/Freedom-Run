using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    public bool isLocked;
    public string unlockKeyId;
    public string sceneDestination;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var controller = other.GetComponent<PlayerController>();
        if (controller != null) controller.AvailableDoor(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var controller = other.GetComponent<PlayerController>();
        if (controller != null) controller.NoAvailableDoor();
    }

    public void OpenDoor()
    {
        SceneManager.LoadScene(sceneDestination);
    }
}