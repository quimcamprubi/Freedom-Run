using UnityEngine;

public class GrabObject : MonoBehaviour
{
    public Transform frontCheck;
    public PlayerController player;
    [SerializeField] private float rayDistance;
    public LayerMask platformLayer;
    private GameObject grabbedObject;
    private int layerIndex;

    // Start is called before the first frame update
    private void Start()
    {
        layerIndex = LayerMask.NameToLayer("Object");
    }


    // Update is called once per frame
    private void Update()
    {
        if (!Input.GetButtonDown("Interact")) return;
        if (grabbedObject == null)
        {
            var hitInfo = Physics2D.Raycast(frontCheck.position, transform.localScale, rayDistance);
            if (hitInfo.collider != null && hitInfo.collider.gameObject.layer == layerIndex)
            {
                grabbedObject = hitInfo.collider.gameObject;
                player.grabbingObject = true;
                grabbedObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
                grabbedObject.transform.position = new Vector3(
                    grabbedObject.transform.position.x + 0.2f * transform.localScale.x,
                    frontCheck.position.y, grabbedObject.transform.position.z);
                grabbedObject.transform.SetParent(transform);
            }
        }
        else
        {
            grabbedObject.transform.SetParent(null);
            grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
            player.grabbingObject = false;
            grabbedObject.GetComponent<Rigidbody2D>().constraints =
                RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            grabbedObject = null;
        }
    }

    private void FixedUpdate()
    {
        CheckSlope();
    }

    private void CheckSlope()
    {
        if (grabbedObject != null)
        {
            var pos = grabbedObject.transform.position;
            var tra = transform.localScale.x;
            var si = grabbedObject.GetComponent<BoxCollider2D>().size.x;
            var checkPosition = new Vector2(pos.x, pos.y + tra * si / 2);
            var slopeHitFront = Physics2D.Raycast(checkPosition, transform.localScale, 1f, platformLayer);
            if (slopeHitFront)
            {
                grabbedObject.transform.SetParent(null);
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
                player.grabbingObject = false;
                grabbedObject.transform.position += new Vector3(-tra * 0.6f, 0f, 0f);
                grabbedObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX |
                                                                        RigidbodyConstraints2D.FreezeRotation;
                grabbedObject = null;
            }
        }
    }
}