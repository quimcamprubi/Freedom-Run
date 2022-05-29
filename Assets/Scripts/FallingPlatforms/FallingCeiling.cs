using UnityEngine;

public class FallingCeiling : MonoBehaviour
{
    public float fallingTime;
    public AudioClip fallingPlatformSound;
    public int mass = 200;
    public float gravity = 9.8f;
    public int lastIndexWithSpikes = 6;
    private AudioSource _audioSource;
    private bool _collisioned;
    private Rigidbody2D _rb;

    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (_rb.isKinematic == false && _rb.velocity.y == 0 && !_collisioned)
        {
            Colisionar();
            _collisioned = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && !_collisioned) Invoke(nameof(DropPlatform), fallingTime);
    }

    private void DropPlatform()
    {
        _rb.isKinematic = false;
        _rb.mass = mass;
        _rb.velocity = new Vector2(0, 10);
        _rb.gravityScale = gravity;
    }

    private void Colisionar()
    {
        _audioSource.PlayOneShot(fallingPlatformSound);
        var damageSpikes = gameObject.transform.GetChild(lastIndexWithSpikes).gameObject;
        Destroy(damageSpikes);
    }
}