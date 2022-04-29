using UnityEngine;

public class FallingPlatforms : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float fallingTime;
    public float destroyTime;
    
    // Start is called before the first frame update
    void Start() {
        _rb = GetComponent<Rigidbody2D> ();
    }

    void OnCollisionEnter2D (Collision2D col) { 
        if (col.gameObject.CompareTag("Player")) {
            Invoke(nameof(DropPlatform), fallingTime);
            Destroy(gameObject, destroyTime);
        }
    }

    void DropPlatform() {
        _rb.isKinematic = false;
    }
}
