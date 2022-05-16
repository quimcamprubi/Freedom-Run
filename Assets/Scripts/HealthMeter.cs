using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthMeter : MonoBehaviour
{
    private const int HEALTH_STATES = 1;
    public GameObject blood;

    public int Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, maxHealth);
            UpdateIndicators();
        }
    }

    public bool Dead => _health == 0;

    // Start is called before the first frame update
    private void Start()
    {
        _health = maxHealth;
        _sprites = new GameObject[0];
        _audioSource = GetComponent<AudioSource>();
        UpdateIndicators();
    }

    private void UpdateIndicators()
    {
        var state = _health % HEALTH_STATES;
        Debug.LogFormat("UpdateIndicators: Health = {0}, State = {1}", _health, state);
        // PERF: could probably reuse stuff here
        foreach (Transform child in gameObject.transform) Destroy(child.gameObject);
        if (Dead) return;
        var spriteCount = (int) Mathf.Ceil((float) _health / HEALTH_STATES);
        _sprites = new GameObject[spriteCount];
        var nextPos = Vector3.zero;
        for (var i = 0; i < spriteCount; ++i)
        {
            var newObj = Instantiate(indicatorPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);
            newObj.transform.localPosition = nextPos;
            _sprites[i] = newObj;
            nextPos += new Vector3(0.2f, 0, 0);
        }

        _lastSprite = _sprites[spriteCount - 1];
        var renderer = _lastSprite.GetComponent<SpriteRenderer>();
        renderer.sprite = states[state];
    }

    public void Hurt()
    {
        Hurt(1);
    }

    public void Hurt(int damage)
    {
        if (Dead) return;
        if (Time.time - lastHurt < invulnerableTime) return;
        Instantiate(blood, playerObject.transform.position, Quaternion.identity);
        Debug.Log("Ouch!");
        lastHurt = Time.time;
        Health -= damage;
        if (Dead)
        {
            StartCoroutine(Death());
            return;
        }

        _audioSource.PlayOneShot(hurtSound);
    }

    private IEnumerator Death()
    {
        playerObject.SetActive(false);
        _audioSource.PlayOneShot(deathSound);
        yield return new WaitForSeconds(deathSound.length);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #region Inspector variables

    public GameObject playerObject;

    public int maxHealth;

    public AudioClip hurtSound;

    public AudioClip deathSound;

    public Sprite[] states = new Sprite[HEALTH_STATES];

    public GameObject indicatorPrefab;

    public float invulnerableTime;

    #endregion

    #region Private variables

    private int _health;

    private GameObject[] _sprites;

    private GameObject _lastSprite;

    private AudioSource _audioSource;

    private float lastHurt;

    #endregion
}