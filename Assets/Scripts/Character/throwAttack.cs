using UnityEngine;

public class throwAttack : MonoBehaviour
{
    public Transform attackPos;
    public GameObject projectilePrefab;
    public float power;
    public bool canThrow = true;
    public float wallDistanceCheck;
    public LayerMask platformLayer;
    private Vector2 direction;
    private GameObject porron;
    private Animator Animator;

    // Start is called before the first frame update
    private void Start()
    {
        Animator = GetComponent<Animator>();
    }

    public bool IsArmed()
    {
        return canThrow;
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (canThrow)
            {
                bool wallOverlap = Physics2D.OverlapCircle(attackPos.position, wallDistanceCheck, platformLayer);
                if (!wallOverlap) Throw();
            }
        }
        Animator.SetBool("isArmed", canThrow);
    }

    private void Throw()
    {
        canThrow = false;
        porron = Instantiate(projectilePrefab, attackPos.position, Quaternion.identity);
        direction = new Vector2(transform.localScale.x, 0.2f);
        porron.GetComponent<Rigidbody2D>().isKinematic = false;
        porron.GetComponent<Rigidbody2D>().AddForce(direction * power);
        porron.GetComponent<porronScript>().canRotate = true;
    }

    public void GrabObject()
    {
        Destroy(porron);
        canThrow = true;
    }
}