using UnityEngine;

public class FallingPlatforms : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float fallingTime;
    public float destroyTime;
    public AudioClip fallingPlatformSound;
    private AudioSource _audioSource;


    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Invoke(nameof(DropPlatform), fallingTime);
            Destroy(gameObject, destroyTime);
        }
    }

    private void DropPlatform()
    {
        _rb.isKinematic = false;
        _audioSource.PlayOneShot(fallingPlatformSound);
    }
}