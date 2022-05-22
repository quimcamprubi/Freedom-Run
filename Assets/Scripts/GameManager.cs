using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerObject;

    public bool UsingGamepad { get; private set; }

    void Update() {
        UsingGamepad = Input.GetJoystickNames().Length > 0;
    }

    public void CreatePrompt(Vector2 position, Transform parent, KeyCode keyCode, string text)
    {
        var prefab = Resources.Load("Prefabs/KeyPrompt");
        var keyPromptObject = (GameObject) Instantiate(prefab, position, Quaternion.identity, parent);
        var keyPrompt = keyPromptObject.GetComponent<KeyPrompt>();
        keyPrompt.keyCode = keyCode;
        keyPrompt.text = text;
    }
}