using UnityEngine;

public class PlaySoundOnTrigger : MonoBehaviour
{
    public bool playOnlyOnce;
    public AudioClip audio;
    public float volume = 0.7f;

    public Transform transformObject;

    // Start is called before the first frame update
    private AudioSource _audioSource;
    private bool alreadyPlayedOnce;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        var hit = Physics2D.Raycast(transformObject.position, Vector2.right, 100, LayerMask.GetMask("Player"));
        if (hit.collider != null)
            if (!alreadyPlayedOnce)
            {
                if (playOnlyOnce) alreadyPlayedOnce = true;
                AudioSource.PlayClipAtPoint(audio, transform.position, volume);
            }
    }

    private void OnTriggerEnter(Collider other)
    {
    }
}