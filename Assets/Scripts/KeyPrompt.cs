using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyPrompt : MonoBehaviour
{
    private static readonly string textureResourcePath =
        "Xelu_Free_Controller&Key_Prompts/Keyboard & Mouse/Dark/{0}_Key_Dark";

    private static readonly Dictionary<string, string> keyTextureMap = new Dictionary<string, string>
    {
        {"Mouse0", "Mouse_Left"}, {"Mouse1", "Mouse_Right"}, {"Mouse2", "Mouse_Middle"}
    };

    public KeyCode keyCode;
    public string text;

    private Animator animator;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();

        var keyTextureName = Enum.GetName(typeof(KeyCode), keyCode);
        if (keyTextureMap.TryGetValue(keyTextureName, out var newTextureName)) keyTextureName = newTextureName;
        var keyTexture = (Texture2D) Resources.Load(string.Format(textureResourcePath, keyTextureName));

        var image = gameObject.GetComponentInChildren<Image>();
        var text = gameObject.GetComponentInChildren<Text>();
        image.sprite = Sprite.Create(keyTexture, new Rect(0, 0, keyTexture.width, keyTexture.height),
            new Vector2(0.5f, 0.5f));
        text.text = this.text;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            Debug.LogFormat("{0} pressed, destroying prompt", Enum.GetName(typeof(KeyCode), keyCode));
            animator.SetTrigger("Disappear");
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}