using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyPrompt : MonoBehaviour
{
    public String[] axes;
    public Texture2D texture;
    public string text;

    private Animator animator;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
        var image = gameObject.GetComponentInChildren<Image>();
        var text = gameObject.GetComponentInChildren<Text>();
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));
        text.text = this.text;
    }

    // Update is called once per frame
    private void Update()
    {
        foreach (var axis in axes)
        {
            if (Math.Abs(Input.GetAxisRaw(axis)) > 0.5)
            {
                Debug.LogFormat("{0} pressed, destroying prompt", axis);
                animator.SetTrigger("Disappear");
                break;
            }
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}