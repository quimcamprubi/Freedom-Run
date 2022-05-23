using System.Collections.Generic;
using System.Linq;
using CollectibleItems;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public attributes
    public float movementSpeed;
    public float ajupidSpeed;
    public bool ajupirse;
    public float jumpSpeed;
    public Transform groundCheck;
    public float groundDistanceCheck;
    public float ceilingDistanceCheck;
    public float wallDistanceCheck;
    public LayerMask platformLayer;
    public float slopeCheckDistance;
    public PhysicsMaterial2D noFriction;
    public PhysicsMaterial2D fullFriction;
    public Transform frontCheck;
    public float wallSlidingSpeed;
    public float xWallForce;
    public float yWallForce;
    public float hookSpeed = 10f;
    public bool grabbingObject;
    public float jumpTime;
    public float coyoteTime;
    public AudioSource salto;
    public AudioSource hookSound;
    public AudioSource puerta_cerrada;
    public double speedFallDamage;
    [HideInInspector] public List<CollectibleItem> keysList;
    public AudioSource shipSound;
    public bool _canStandup;

    public List<RegularItem> itemsList;
    public bool _isGrounded;
    public bool _canJump;
    public bool _isWallSliding;
    public bool _isGrappling = false;
    public LineRenderer m_lineRenderer;
    public float _coyoteTimeCounter;
    public GameObject Canvas;
    public GameObject GrapplingGunGameObject;
    public GameObject GrapplingGunScriptObject;

    public HealthMeter HealthScript;
    public bool onMovingPlatform;
    public float platformSpeed;

    private CapsuleCollider2D _capsuleCollider;
    private Vector2 _colliderSize;
    private bool _HookAnimationEnded;
    private bool _HookAnimationStarted;

    private float _input;
    private bool _isHookAvailable;
    private bool _isHooking;
    private bool _isJumping;
    private bool _isOnSlope;

    // Private attributes
    private bool _isTouchingFront;
    private bool _isWallJumping;
    private float _jumpTimeCounter;
    private float _modifiedJumpSpeed;
    private Vector2 _newForce;
    private Vector2 _newVelocity;
    private float _previousWallJumpDirection;
    private Rigidbody2D _rigidbody2D;
    private float _slopeDownAngle;
    private float _slopeDownAngleOld;
    private Vector2 _slopeNormalPerp;
    private float _slopeSideAngle;
    private bool _sprintFall;
    private bool _sprintJump;
    private float _sprintModifier = 1.0f;
    private double maxYvel=0;
    private Animator Animator;

    // Collectible items
    private CollectibleItem availableCollectibleItem;

    // Doors
    private DoorController availableDoor;
    private bool canAddCollectible;
    private bool canOpenDoor;

    // Porron
    public bool canPorron = false;
    private GameObject objectToDestroy;
    private bool Paused;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _colliderSize = _capsuleCollider.size;
        Animator = GetComponent<Animator>();
        grabbingObject = false;
        HealthScript = GameObject.Find("HealthMeter").GetComponent<HealthMeter>();
    }

    // Update is called once per frame
    private void Update()
    {
        CheckInput();
    }

    private void FixedUpdate()
    { 
        if (!_isGrounded)
        {
            if (_rigidbody2D.velocity.y < maxYvel)
            {
                maxYvel = _rigidbody2D.velocity.y;
            }
        }
        bool auxBool = !_isGrounded && !_isJumping;
        if (!_isGrappling)
        {
            CheckGround();
            CheckSlope();
            CheckFront();
            ApplyMovement();
            CheckHook();
            CheckCeiling();
        }
        if (_isGrounded && auxBool)
        {
            if (maxYvel <= speedFallDamage)
            {
                HealthScript.Hurt(3);
            }
            maxYvel = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "MainCamera") HealthScript.Hurt(2);
    }


    private void CheckInput()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (canAddCollectible)
            {
                AddCollectible();
                objectToDestroy.SetActive(false);
            }
            else if (canPorron)
            {
                canPorron = false;
                GetComponent<throwAttack>().GrabObject();
            }
            else if (canOpenDoor)
            {
                if (availableDoor.isLocked)
                {
                    // If door is locked, check if player has the necessary key
                    if (keysList.Any(key => key.collectibleItemId == availableDoor.unlockKeyId))
                        availableDoor.OpenDoor();
                    else {
                        availableDoor.LockedEvent();
                        puerta_cerrada.Play();
                    }
                } else { // If door is unlocked, open it
                    availableDoor.OpenDoor();
                }
            }
        }

        if (Input.GetButtonUp("Jump")) _isJumping = false;
        _input = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            if (_isGrounded) Jump();
            if (_isWallSliding)
            {
                _isWallSliding = false;
                WallJump();
            }

            JumpSound();
        }

        Animator.SetBool("running", _input != 0.0f);


        if (Input.GetButton("Jump"))
        {
            Jump();
        }
        else
        {
            if (grabbingObject)
                _canJump = false;
            else if (_isGrounded && !_isJumping) _canJump = true;
        }

        _sprintModifier = 1.0f + (Input.GetButton("Sprint") ? 0.5f : 0.5f * Input.GetAxis("Sprint"));
        if (Input.GetButton("Crouch"))
        {
            Animator.SetBool("ajupirse_correr", _input != 0.0f);
            ajupirse = true;
            Animator.SetBool("ajupirse", true);
        }
        else
        {
            ajupirse = false;
            Animator.SetBool("ajupirse", false);
        }

        if (_isGrounded && !_isJumping)
            Animator.SetBool("isGrounded", true);
        else
            Animator.SetBool("isGrounded", false);

        if (!_isJumping)
            Animator.SetBool("isJumping", false);
        else
            Animator.SetBool("isJumping", true);

        _isHooking = Input.GetButton("Interact");

        if (_isHooking && _isHookAvailable)
        {
            if (!_HookAnimationStarted)
            {
                Animator.Play("Hook");
                Animator.SetBool("isHooking", true);
                //Animator.SetBool("isHooking", false);
                _HookAnimationStarted = true;
                _HookAnimationEnded = false;
            }

            if (_HookAnimationEnded)
            {
                m_lineRenderer.enabled = true;
                var bodypoint = new Vector3(_rigidbody2D.position.x, _rigidbody2D.position.y, -1);
                m_lineRenderer.SetPosition(0, bodypoint);
                var hit = Physics2D.Raycast(transform.position, Vector2.up,
                    99999); //we are colliding with hook if inside if
                var hitpoint = new Vector3(hit.point.x, hit.point.y, -1);
                m_lineRenderer.SetPosition(1, hitpoint);

                _newVelocity.Set(_rigidbody2D.velocity.x, hookSpeed);
                _rigidbody2D.velocity = _newVelocity;
            }
        }

        if (!_isHooking && _HookAnimationEnded)
        {
            Animator.SetBool("isHooking", false);
            //Debug.Log("isHooking false");
            _HookAnimationStarted = false;
            m_lineRenderer.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Paused == false)
            {
                Time.timeScale = 0f;
                Paused = true;
                GrapplingGunGameObject.gameObject.SetActive(false);
                Canvas.gameObject.SetActive(true);
            }
            else
            {
                Canvas.gameObject.SetActive(false);
                GrapplingGunGameObject.gameObject.SetActive(true);
                Paused = false;
                Time.timeScale = 1f;
            }
        }
                Animator.SetBool("isGrappling", _isGrappling);
    }

    private void AddCollectible()
    {
        switch (availableCollectibleItem)
        {
            case KeyItem key:
                keysList.Add(key);
                break;
            case WeaponItem weapon:
                GetComponent<throwAttack>().GrabObject();
                break;
            case RegularItem item:
                itemsList.Add(item);
                break;
            case GrapplingGunItem gun:
                GrapplingGunScriptObject.GetComponent<GrapplingGun>().canGrapp = true;
                keysList.Add(gun);
                break;
        }
    }

    public void HookingEnded()
    {
        _HookAnimationEnded = true;
        Animator.SetBool("isHooking", false);
    }

    private void WallJump()
    {
        var canWallJump = Mathf.Sign(_rigidbody2D.transform.localScale.x) != Mathf.Sign(_previousWallJumpDirection);
        if (canWallJump || Mathf.Approximately(_previousWallJumpDirection, 0.0f))
        {
            _previousWallJumpDirection = _rigidbody2D.transform.localScale.x;
            _isWallSliding = false;
            _isWallJumping = true;
            _newForce.Set(-xWallForce * transform.localScale.x, yWallForce);
            _rigidbody2D.velocity = Vector3.zero;
            _rigidbody2D.AddForce(_newForce, ForceMode2D.Impulse);
            _rigidbody2D.transform.localScale = _rigidbody2D.transform.localScale.Equals(new Vector2(1.0f, 1.0f))
                ? new Vector2(-1.0f, 1.0f)
                : new Vector2(1.0f, 1.0f);
        }
    }

    private void CheckHook()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.up, 99999);
        //Debug.DrawRay(transform.position, Vector2.up, Color.red, 10.0f);
        //Debug.Log(hit.collider.name);

        if (hit.collider != null && hit.collider.CompareTag("Hook") /*&& _isGrounded*/) _isHookAvailable = true;
        //else {
        //_isHookAvailable = false;
        //}
        //Debug.Log(_isHookAvailable);
    }

    private void CheckGround()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundDistanceCheck);
        if (_rigidbody2D.velocity.y <= 0.0f) _isJumping = false;
        if (_isGrounded && !_isJumping)
        {
            _previousWallJumpDirection = 0.0f;
            _isWallJumping = false;
            _isWallSliding = false;
            _sprintJump = false;
            _coyoteTimeCounter = coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.fixedDeltaTime;
        }
    }

    private void CheckCeiling()
    {

        _canStandup = !(Physics2D.Raycast(transform.position, Vector2.up, ceilingDistanceCheck));
        if(_canStandup)
        {
            Animator.SetBool("canStandup", true);
        }
        else{
            Animator.SetBool("canStandup", false);
        }

    }

    private void CheckFront()
    {
        _isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, wallDistanceCheck, platformLayer);
        _isWallSliding = _isTouchingFront && !_isGrounded;
    }

    public void Jump()
    {
        if (_coyoteTimeCounter > 0f && _canJump)
        {
            _canJump = false;
            _isJumping = true;
            _sprintJump = _sprintModifier > 1.0f;
            _jumpTimeCounter = jumpTime;
            _coyoteTimeCounter = 0f;
        }

        if (_isJumping)
        {
            if (_jumpTimeCounter > 0)
            {
                _modifiedJumpSpeed = jumpSpeed;
                _newForce.Set(_rigidbody2D.velocity.x, _modifiedJumpSpeed);
                _rigidbody2D.velocity = _newForce;
                _jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                _isJumping = false;
            }
        }
    }

    private void CheckSprintModifier()
    {
        if (_isGrounded)
        {
            _sprintFall = _sprintModifier > 1.0f;
        }
        else
        {
            _sprintModifier = _sprintJump || _sprintFall ? 1.5f : 1.0f;
        }
    }

    private void CheckSlope()
    {
        Vector2 checkPosition = transform.position - (Vector3) new Vector2(0.0f, _colliderSize.y / 2);
        HorizontalCheckSlope(checkPosition);
        VerticalCheckSlope(checkPosition);
    }

    private void HorizontalCheckSlope(Vector2 checkPosition)
    {
        var slopeHitFront = Physics2D.Raycast(checkPosition, transform.right, slopeCheckDistance, platformLayer);
        var slopeHitBack = Physics2D.Raycast(checkPosition, -transform.right, slopeCheckDistance, platformLayer);
        if (slopeHitFront)
        {
            _isOnSlope = true;
            _slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            _isOnSlope = true;
            _slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            _slopeSideAngle = 0.0f;
            _isOnSlope = false;
        }
    }

    private void VerticalCheckSlope(Vector2 checkPosition)
    {
        var hit = Physics2D.Raycast(checkPosition, Vector2.down, slopeCheckDistance, platformLayer);
        if (hit)
        {
            _slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            _slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (!Mathf.Approximately(_slopeDownAngle, _slopeDownAngleOld)) _isOnSlope = true;
            _slopeDownAngleOld = _slopeDownAngle;
        }

        if (_isOnSlope && _input == 0.0f && _slopeDownAngle != 0.0f)
            _rigidbody2D.gravityScale = 0.0f;
        else
            _rigidbody2D.gravityScale = 5.0f;
    }

    private void ApplyMovement()
    {
        if (_input < 0.0f)
        {
            transform.localScale = new Vector2(-1.0f, 1.0f);
            _isHookAvailable = false;
        }
        else if (_input > 0.0f)
        {
            transform.localScale = new Vector2(1.0f, 1.0f);
            _isHookAvailable = false;
        }

        CheckSprintModifier();

        if (_isWallSliding)
        {
            _isWallJumping = false;
            _newVelocity.Set(_rigidbody2D.velocity.x,
                Mathf.Clamp(_rigidbody2D.velocity.y, -wallSlidingSpeed, float.MaxValue));
            _rigidbody2D.velocity = _newVelocity;
        }
        else if (ajupirse)
        {
            _newVelocity.Set(ajupidSpeed * _input * _sprintModifier, _rigidbody2D.velocity.y);
            _rigidbody2D.velocity = _newVelocity;
        }
        else if (_isGrounded && !_isOnSlope && !_isJumping)
        {
            if (onMovingPlatform)
                _newVelocity.Set(movementSpeed * _input * _sprintModifier + platformSpeed, _rigidbody2D.velocity.y);
            else
                _newVelocity.Set(movementSpeed * _input * _sprintModifier, _rigidbody2D.velocity.y);
            _rigidbody2D.velocity = _newVelocity;
        }
        else if (_isGrounded && _isOnSlope && !_isJumping)
        {
            if (onMovingPlatform)
                _newVelocity.Set(movementSpeed * _slopeNormalPerp.x * -_input * 1.5f + platformSpeed,
                    movementSpeed * _slopeNormalPerp.y * -_input * 1.5f);
            else
                _newVelocity.Set(movementSpeed * _slopeNormalPerp.x * -_input * 1.5f,
                    movementSpeed * _slopeNormalPerp.y * -_input * 1.5f);
            _rigidbody2D.velocity = _newVelocity;
        }
        else if (!_isGrounded && !_isWallJumping || !_isGrounded && _isWallJumping && _input != 0)
        {
            _newVelocity.Set(movementSpeed * _input * _sprintModifier, _rigidbody2D.velocity.y);
            _rigidbody2D.velocity = _newVelocity;
        }
    }

    public void AvailableCollectibleItem(CollectibleItem item, GameObject keyObject) 
    {
        availableCollectibleItem = item;
        objectToDestroy = keyObject;
        canAddCollectible = true;
    }

    public void NoAvailableCollectibleItem()
    {
        availableCollectibleItem = null;
        objectToDestroy = null;
        canAddCollectible = false;
    }

    public void AvailableDoor(DoorController doorController)
    {
        availableDoor = doorController;
        canOpenDoor = true;
    }

    public void NoAvailableDoor()
    {
        availableDoor = null;
        canOpenDoor = false;
    }

    public void AvailablePorron()
    {
        canPorron = true;
    }

    public void NoAvailablePorron()
    {
        canPorron = false;
    }

    private void JumpSound()
    {
        salto.Play();
    }

    public void HookSound()
    {
        hookSound.Play();
    }

    public void ShipSound()
    {
        shipSound.Play();
    }
}