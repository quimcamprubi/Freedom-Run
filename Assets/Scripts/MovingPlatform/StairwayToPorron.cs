// Keep in mind that this solution is for custom physics.
// The easy way to add gravity is to just slap a RigidBody component 
// on your objects.

using UnityEngine;

public class StairwayToPorron : MonoBehaviour
{
    public Vector3 speed;
    public float gravity = 1.62f;

    private BoxCollider2D _boxCollider2D;

    // Start() is called before the first frame
    private void Start()
    {
        speed = new Vector3(0f, 0f, 0f);
    }

    private void FixedUpdate()
    {
        speed.Set(speed.x, speed.y - gravity * Time.fixedDeltaTime, speed.z);
        transform.position += speed;
    }
}