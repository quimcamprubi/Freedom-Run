using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class FallingCeiling : MonoBehaviour
{
    public float fallingTime;
    public AudioClip fallingPlatformSound;
    private AudioSource _audioSource;
    private Rigidbody2D _rb;
    private bool _collisioned;
    public int mass = 200;
    public float gravity = 9.8f;
    public int lastIndexWithSpikes = 6;

    // Start is called before the first frame update
    private void Start() {
        _rb = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.CompareTag("Player") && !_collisioned) {
            Invoke(nameof(DropPlatform), fallingTime);
        }
    }

    private void DropPlatform() {
        _rb.isKinematic = false;
        _rb.mass = mass;
        _rb.velocity = new Vector2(0, 10);
        _rb.gravityScale = gravity;
    }

    private void FixedUpdate() {
        if (_rb.isKinematic == false && _rb.velocity.y == 0 && !_collisioned) {
            Colisionar();
            _collisioned = true;
        }
    }

    private void Colisionar() {
        _audioSource.PlayOneShot(fallingPlatformSound);
        GameObject damageSpikes = gameObject.transform.GetChild(lastIndexWithSpikes).gameObject;
        Destroy(damageSpikes);
    }
}
