using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    public Transform frontCheck;

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
        RaycastHit2D hitInfo = Physics2D.Raycast(frontCheck.position, transform.localScale, rayDistance);
        if (hitInfo.collider != null && hitInfo.collider.gameObject.layer==layerIndex)
        {
            if (Input.GetKey(KeyCode.G))
            {
                if (grabbedObject == null)
                {
                    grabbedObject = hitInfo.collider.gameObject;
                    grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
                    grabbedObject.transform.SetParent(transform);
                }
                else
                {
                    grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
                    grabbedObject.transform.SetParent(null);
                    grabbedObject = null;
                }
            }
        }

        Debug.DrawRay(frontCheck.position, transform.right * rayDistance);
    }
}
