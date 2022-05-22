using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    [Header("Scripts Ref:")] public GrapplingRope grappleRope;
    public PlayerController playerController;

    [Header("Layers Settings:")] [SerializeField]
    private bool grappleToAll;

    public bool canGrapp;
    [SerializeField] private int grappableLayerNumber = 9;

    [Header("Main Camera:")] public Camera m_camera;

    [Header("Transform Ref:")] public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;

    [Header("Physics Ref:")] public SpringJoint2D m_springJoint2D;
    public Rigidbody2D m_rigidbody;

    [Header("Rotation:")] [SerializeField] private bool rotateOverTime = true;
    [Range(0, 60)] [SerializeField] private float rotationSpeed = 4;

    [Header("Distance:")] [SerializeField] private bool hasMaxDistance;
    [SerializeField] private float maxDistance = 20;

    [Header("No Launch To Point")] [SerializeField]
    private bool autoConfigureDistance;

    [SerializeField] private float targetDistance = 3;
    [SerializeField] private float targetFrequncy = 1;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    private bool isHooked;

    private void Start()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;
    }

    private void Update()
    {
        if(canGrapp){
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                SetGrapplePoint();
            }
            else if (Input.GetKey(KeyCode.Mouse0))
            {
                if (Input.GetButtonDown("Jump") && isHooked)
                {
                    playerController._isGrappling = false;
                    playerController._isWallSliding = false;
                    playerController._canJump = true;
                    isHooked = false;
                    playerController._coyoteTimeCounter = 2.0f;
                    onMouse0Release();
                    if (playerController._isGrounded) playerController.Jump();
                }

                if (grappleRope.enabled)
                {
                    RotateGun(grapplePoint, false);
                }
                else
                {
                    Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
                    RotateGun(mousePos, true);
                }
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                isHooked = false;
                onMouse0Release();
            }
            else
            {
                Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
                RotateGun(mousePos, true);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null && hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistance);
        }
    }

    private void onMouse0Release()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;
        playerController._isGrappling = false;
        m_rigidbody.gravityScale = 1;
    }

    private void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        var distanceVector = lookPoint - gunPivot.position;

        var angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime)
            gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, Quaternion.AngleAxis(angle, Vector3.forward),
                Time.deltaTime * rotationSpeed);
        else
            gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void SetGrapplePoint()
    {
        playerController.HookSound();
        Vector2 distanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
        if (Physics2D.Raycast(firePoint.position, distanceVector.normalized))
        {
            var _hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized);
            if (_hit.transform.gameObject.layer == grappableLayerNumber || grappleToAll)
                if (Vector2.Distance(_hit.point, firePoint.position) <= maxDistance || !hasMaxDistance)
                {
                    playerController._isGrappling = true;
                    grapplePoint = _hit.point;
                    grappleDistanceVector = grapplePoint - (Vector2) gunPivot.position;
                    grappleRope.enabled = true;
                }
        }
    }

    public void Grapple()
    {
        m_springJoint2D.autoConfigureDistance = false;
        if (!autoConfigureDistance)
        {
            m_springJoint2D.distance = targetDistance;
            m_springJoint2D.frequency = targetFrequncy;
        }

        if (autoConfigureDistance)
        {
            m_springJoint2D.autoConfigureDistance = true;
            m_springJoint2D.frequency = 0;
        }

        isHooked = true;
        m_springJoint2D.connectedAnchor = grapplePoint;
        m_springJoint2D.enabled = true;
    }
}