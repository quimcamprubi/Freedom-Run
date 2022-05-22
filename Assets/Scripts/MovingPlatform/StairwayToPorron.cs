// Keep in mind that this solution is for custom physics.
// The easy way to add gravity is to just slap a RigidBody component 
// on your objects.

using System;
using UnityEngine;

public class StairwayToPorron : MonoBehaviour
{
    public Vector3 speed;
    public float gravity = 1.62f;
    private AudioSource _audioSource;
    public AudioClip boxingSound;
    public float bellDelay = 3f;
    private BoxCollider2D _boxCollider2D;
    // Start() is called before the first frame
    void Start()
    {
        speed = new Vector3(0f, 0f, 0f);
        _audioSource = GetComponent<AudioSource>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        Invoke(nameof(BellSound), bellDelay);
    }

    void BellSound() {
        AudioSource.PlayClipAtPoint(boxingSound, transform.position, 0.7f);

        // _audioSource.PlayOneShot(boxingSound);
    }

    // FixedUpdate() is called at regular time intervals independent of frames
    void FixedUpdate()
    {
        speed.Set(speed.x, speed.y - gravity * Time.fixedDeltaTime, speed.z);
        transform.position += speed;
        
        
    }
}
