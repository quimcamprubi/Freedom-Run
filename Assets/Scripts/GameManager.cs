using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerObject;

    public void CreatePrompt(Vector2 position, Transform parent, KeyCode keyCode, string text) {
        var prefab = Resources.Load("Prefabs/KeyPrompt");
        var keyPromptObject = (GameObject)Instantiate(prefab, position, Quaternion.identity, parent);
        var keyPrompt = keyPromptObject.GetComponent<KeyPrompt>();
        keyPrompt.keyCode = keyCode;
        keyPrompt.text = text;
    }
}
