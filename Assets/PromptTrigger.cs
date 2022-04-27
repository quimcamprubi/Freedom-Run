using UnityEngine;

public class PromptTrigger : MonoBehaviour
{
    public KeyCode key;

    public string text;

    private GameManager gameManager;

    void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Prompt trigger activated");
        if (other.gameObject == gameManager.playerObject) {
            gameManager.CreatePrompt(transform.position, transform.parent, key, text);
            Destroy(gameObject);
        }
    }
}
