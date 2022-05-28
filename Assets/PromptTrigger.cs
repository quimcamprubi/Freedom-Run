using UnityEngine;

public class PromptTrigger : MonoBehaviour
{
    public string[] axes;

    public Texture2D textureKeyboard;

    public Texture2D textureGamepad;

    public string text;

    public bool keepParent;

    private GameManager gameManager;

    void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Prompt trigger activated");
        if (other.gameObject == gameManager.playerObject) {
            var texture = gameManager.UsingGamepad ? textureGamepad : textureKeyboard;
            gameManager.CreatePrompt(transform.position, keepParent ? transform.parent : null, axes, texture, text);
            Destroy(gameObject);
        }
    }
}
