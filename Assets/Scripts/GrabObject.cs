using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    public Transform frontCheck;
    public PlayerController player;
    [SerializeField]
    private float rayDistance;
    public LayerMask platformLayer;
    private int layerIndex;
    private GameObject grabbedObject;

    // Start is called before the first frame update
    void Start()
    {
        layerIndex=LayerMask.NameToLayer("Object");
    }
    
    private void FixedUpdate() {
        CheckSlope();
    }

    private void CheckSlope()
    {
        if (grabbedObject != null)
        {
            Vector3 pos = grabbedObject.transform.position;
            float tra = transform.localScale.x;
            float si= grabbedObject.GetComponent<BoxCollider2D>().size.x;
            Vector2 checkPosition = new Vector2(pos.x, pos.y+ tra * si/ 2);
            RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPosition, transform.localScale, 1f, platformLayer);
            if (slopeHitFront)
            {
                grabbedObject.transform.SetParent(null);
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
                player.grabbingObject = false;
                grabbedObject.transform.position += new Vector3(-tra*0.6f,0f,0f);
                grabbedObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                grabbedObject = null;
            }
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.G)) return;
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
        else
        {
            grabbedObject.transform.SetParent(null);
            grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
            player.grabbingObject = false;
            grabbedObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            grabbedObject = null;
        }
    }
}
