using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerObject;

    public bool UsingGamepad { get; private set; }

    private void Update()
    {
        UsingGamepad = Input.GetJoystickNames().Length > 0;
    }

    public void CreatePrompt(Vector2 position, Transform parent, string[] axes, Texture2D texture, string text)
    {
        var prefab = Resources.Load("Prefabs/KeyPrompt");
        var keyPromptObject = (GameObject) Instantiate(prefab, position, Quaternion.identity, parent);
        var keyPrompt = keyPromptObject.GetComponent<KeyPrompt>();
        keyPrompt.axes = axes;
        keyPrompt.texture = texture;
        keyPrompt.text = text;
    }
}