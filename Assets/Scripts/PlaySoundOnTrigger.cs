using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource _audioSource;
    public bool playOnlyOnce = false;
    public AudioClip audio;
    public float volume = 0.7f;
    private bool alreadyPlayedOnce = false;
    public Transform transformObject;
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate() {
        RaycastHit2D hit = Physics2D.Raycast(transformObject.position, Vector2.right, 100, LayerMask.GetMask("Player"));
        if (hit.collider != null) {
            if (!alreadyPlayedOnce) {
                if (playOnlyOnce) {
                    alreadyPlayedOnce = true;
                }
                AudioSource.PlayClipAtPoint(audio, transform.position, volume);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
       
    }
}
