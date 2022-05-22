using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    public bool isLocked;
    public bool isClosing;
    public string unlockKeyId;
    public string sceneDestination;

    public AudioClip openSound;

    public AudioClip closeSound;

    private AudioSource source;

    private Animator animator;

    void Start() {
        source = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        if (isClosing) {
            animator.SetTrigger("Close");
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        PlayerController controller = other.GetComponent<PlayerController>();
        if (controller != null) {
            controller.AvailableDoor(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var controller = other.GetComponent<PlayerController>();
        if (controller != null) controller.NoAvailableDoor();
    }

    public void OpenDoor() {
        animator.SetTrigger("Open");
    }

    public void PlayOpenSound() {
        source.PlayOneShot(openSound);
    }

    public void PlayCloseSound() {
        source.PlayOneShot(closeSound);
    }

    public void LoadDestination() {
        PlayerPrefs.SetString("LevelProgress", sceneDestination);
        SceneManager.LoadScene(sceneDestination);
    }

    public void LockedEvent() {
        gameObject.GetComponent<OnTextCalled>().enabled = true;
    }
}