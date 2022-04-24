using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    public Transform frontCheck;
    public PlayerController player;
    [SerializeField]
    private float rayDistance;

    private int layerIndex;
    private GameObject grabbedObject;
    // Start is called before the first frame update
    void Start()
    {
        layerIndex=LayerMask.NameToLayer("Object");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)){
            if (grabbedObject == null){
                RaycastHit2D hitInfo = Physics2D.Raycast(frontCheck.position, transform.localScale, rayDistance);
                if (hitInfo.collider != null && hitInfo.collider.gameObject.layer == layerIndex){
                    grabbedObject = hitInfo.collider.gameObject;
                    player.grabbingObject = true;
                    grabbedObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                    grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
                    grabbedObject.transform.position = new Vector3(grabbedObject.transform.position.x+ 0.2f * transform.localScale.x,
                        frontCheck.position.y, grabbedObject.transform.position.z);
                    grabbedObject.transform.SetParent(transform);
                }
            }
            else{
                grabbedObject.transform.SetParent(null);
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
                grabbedObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                grabbedObject = null;
                player.grabbingObject = false;

            }
        }
    }
}
