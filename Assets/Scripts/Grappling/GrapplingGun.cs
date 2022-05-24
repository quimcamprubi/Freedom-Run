using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    [Header("Scripts Ref:")] public GrapplingRope grappleRope;
    public PlayerController playerController;

    [Header("Layers Settings:")] [SerializeField]
    private bool grappleToAll;

    public bool grappleEnabled;
    [SerializeField] private int grappableLayerNumber = 9;

    [Header("Main Camera:")] public Camera m_camera;

    [Header("Transform Ref:")] public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;

    public float grappleThreshold = 0.6f;

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

    private bool canHook;

    private Vector2 lastChange;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;
    }

    private void Update()
    {
        if (!grappleEnabled) return;

        Vector2 distanceVector;
        bool shouldHook, shouldRelease;
        if (gameManager.UsingGamepad)
        {
            distanceVector = new Vector2(Input.GetAxisRaw("HorizontalSecondary"),
                -Input.GetAxisRaw("VerticalSecondary"));
            shouldHook = shouldRelease = (lastChange - distanceVector).magnitude > grappleThreshold;
        }
        else
        {
            distanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
            shouldRelease = !Input.GetKey(KeyCode.Mouse0);
            shouldHook = Input.GetKeyDown(KeyCode.Mouse0);
        }

        canHook = !isHooked && (canHook || playerController._isGrounded);
        shouldHook = shouldHook && canHook;
        if (shouldHook && !isHooked)
        {
            SetGrapplePoint(distanceVector);
        }
        else if (shouldRelease && isHooked)
        {
            ReleaseGrapple();
        }
        else if (isHooked)
        {
            if (Input.GetButtonDown("Jump"))
            {
                playerController._isWallSliding = false;
                playerController._canJump = true;
                playerController._coyoteTimeCounter = 2.0f;
                ReleaseGrapple();
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
        else
        {
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            RotateGun(mousePos, true);
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

    private void ReleaseGrapple()
    {
        lastChange = Vector2.zero;
        isHooked = false;
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

    private void SetGrapplePoint(Vector2 distanceVector)
    {
        if (distanceVector.magnitude < grappleThreshold) return;
        lastChange = distanceVector;
        playerController.HookSound();
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