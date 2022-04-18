using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    // Public attributes
    public float movementSpeed;
    public float jumpSpeed;
    public Transform groundCheck;
    public float groundDistanceCheck;
    public float wallDistanceCheck;
    public LayerMask platformLayer;
    public float slopeCheckDistance;
    public PhysicsMaterial2D noFriction;
    public PhysicsMaterial2D fullFriction;
    public Transform frontCheck;
    public float wallSlidingSpeed;
    public float xWallForce;
    public float yWallForce;
    public float hookSpeed;
    
    public float jumpTime;
    public float coyoteTime;

    // Private attributes
    private bool _isSprinting = false;
    private bool _isGrounded;
    private bool _isJumping;
    private bool _canJump;
    private bool _sprintJump;
    private bool _isOnSlope;
    private bool _sprintFall;
    private bool _isTouchingFront;
    private bool _isWallSliding;
    private bool _isWallJumping;
    private float _previousWallJumpDirection = 0.0f;
    private bool _isHookAvailable;
    private bool _isHooking;
    
    private Animator Animator;

    private CapsuleCollider2D _capsuleCollider;
    private Rigidbody2D _rigidbody2D;
    private Vector2 _colliderSize;
    private Vector2 _slopeNormalPerp;
    private Vector2 _newVelocity;
    private Vector2 _newForce;
    
    private float _input;
    private float _sprintModifier = 1.0f;
    private float _slopeDownAngle;
    private float _slopeSideAngle;
    private float _slopeDownAngleOld;
    private float _jumpTimeCounter;
    private float _modifiedJumpSpeed;
    private float _coyoteTimeCounter;
    private bool _HookAnimationStarted = false;
    private bool _HookAnimationEnded = false;
    public LineRenderer m_lineRenderer;
    void Start() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _colliderSize = _capsuleCollider.size;
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        CheckInput();
        
    }

    private void FixedUpdate() {
        CheckGround();
        CheckSlope();
        CheckFront();
        ApplyMovement();
        CheckHook();
    }

    private void CheckInput() {
        if (Input.GetButtonUp("Jump"))
        {
            _isJumping = false;
        }
        _input = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump")) {
            if (_isGrounded) Jump();
            if (_isWallSliding) {
                _isWallSliding = false;
                WallJump();
            }
        }

        Animator.SetBool("running", _input != 0.0f);

        if (Input.GetButton("Jump")) {Jump();}
        else
        {
            if (_isGrounded && !_isJumping)
            {
                _canJump = true;
            }
        }
        
        _isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (_isGrounded && !_isJumping) { Animator.SetBool("isGrounded", true); }
        else { Animator.SetBool("isGrounded", false); }

        if (_isJumping == false) { Animator.SetBool("isJumping", false); }
        if (_isJumping == true) { Animator.SetBool("isJumping", true); }

        _isHooking = Input.GetKey(KeyCode.W);
        
        if (_isHooking && _isHookAvailable)
        {
            if (!_HookAnimationStarted)
            {
                Animator.Play("Hook");
                Animator.SetBool("isHooking", true);
                Animator.SetBool("isHooking", false);
                _HookAnimationStarted = true;
                _HookAnimationEnded = false;
                
            }

            if (_HookAnimationEnded)
            {
                m_lineRenderer.enabled = true;
                Vector3 bodypoint = new Vector3(_rigidbody2D.position.x, _rigidbody2D.position.y, -1);
                m_lineRenderer.SetPosition(0, bodypoint);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 99999); //we are colliding with hook if inside if
                Vector3 hitpoint = new Vector3(hit.point.x,hit.point.y,-1);
                m_lineRenderer.SetPosition(1, hitpoint);

                _newVelocity.Set(_rigidbody2D.velocity.x, hookSpeed);
                _rigidbody2D.velocity = _newVelocity;
                
            }

        }
        if (!_isHooking && _HookAnimationEnded)
        {
            Animator.SetBool("isHooking", false);
            Debug.Log("isHooking false");
            _HookAnimationStarted = false;
            m_lineRenderer.enabled = false;
        }
        

    }

    public void HookingEnded()
    {
        _HookAnimationEnded = true;
        Animator.SetBool("isHooking", false);
    }
    private void WallJump() {
        bool canWallJump = Mathf.Sign(_rigidbody2D.transform.localScale.x) != Mathf.Sign(_previousWallJumpDirection);
        if (canWallJump || Mathf.Approximately(_previousWallJumpDirection,0.0f)) {
            _previousWallJumpDirection = _rigidbody2D.transform.localScale.x;
            _isWallSliding = false;
            _isWallJumping = true;
            _newForce.Set(-xWallForce * transform.localScale.x, yWallForce);
            _rigidbody2D.AddForce(_newForce, ForceMode2D.Impulse);
            _rigidbody2D.transform.localScale = _rigidbody2D.transform.localScale.Equals(new Vector2(1.0f, 1.0f))
                    ? new Vector2(-1.0f, 1.0f)
                    : new Vector2(1.0f, 1.0f);
        }
    }

    private void CheckHook()
    {
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 99999);
        //Debug.DrawRay(transform.position, Vector2.up, Color.red, 10.0f);
        //Debug.Log(hit.collider.name);

        if (hit.collider.CompareTag("Hook")){
            
            
            _isHookAvailable = true;
            
        }
        else
        {
            _isHookAvailable = false;
        }
        //Debug.Log(_isHookAvailable);

    } 

    private void CheckGround() {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundDistanceCheck);
        if (_rigidbody2D.velocity.y <= 0.0f) {
            _isJumping = false;
        }
        if (_isGrounded && !_isJumping) {
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

    private void CheckFront() {
        _isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, wallDistanceCheck, platformLayer);
        _isWallSliding = _isTouchingFront && !_isGrounded;
    }

    private void Jump() { 
        if (_coyoteTimeCounter>0f && _canJump)
        {
            _canJump = false;
            _isJumping = true;
            _sprintJump = _isSprinting;
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

    private void CheckSprintModifier() {
        if (_isGrounded) {
            _sprintModifier = _isSprinting ? 1.5f : 1.0f;
            _sprintFall = _isSprinting;
        } else {
            _sprintModifier = _sprintJump || _sprintFall ? 1.5f : 1.0f;
        }
    }
    
    private void CheckSlope() {
        Vector2 checkPosition = transform.position - (Vector3)(new Vector2(0.0f, _colliderSize.y / 2));
        HorizontalCheckSlope(checkPosition);
        VerticalCheckSlope(checkPosition);
    }

    private void HorizontalCheckSlope(Vector2 checkPosition) {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPosition, transform.right, slopeCheckDistance, platformLayer);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPosition, -transform.right, slopeCheckDistance, platformLayer);
        if (slopeHitFront) {
            _isOnSlope = true;
            _slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        } else if (slopeHitBack) {
            _isOnSlope = true;
            _slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        } else {
            _slopeSideAngle = 0.0f;
            _isOnSlope = false;
        }
    }

    private void VerticalCheckSlope(Vector2 checkPosition) {
        RaycastHit2D hit = Physics2D.Raycast(checkPosition, Vector2.down, slopeCheckDistance, platformLayer);
        if (hit) {
            _slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            _slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(!Mathf.Approximately(_slopeDownAngle, _slopeDownAngleOld)) {
                _isOnSlope = true;
            }
            _slopeDownAngleOld = _slopeDownAngle;
        }
        if (_isOnSlope && _input == 0.0f && _slopeDownAngle != 0.0f) {
            _rigidbody2D.gravityScale = 0.0f;
        } else {
            _rigidbody2D.gravityScale = 5.0f;
        }
    }
    
    private void ApplyMovement() {
        if (_input < 0.0f) {
            transform.localScale = new Vector2(-1.0f, 1.0f);
            
        } else if (_input > 0.0f) {
            transform.localScale = new Vector2(1.0f, 1.0f);
        }
        CheckSprintModifier();

        if (_isWallSliding) {
            _isWallJumping = false;
            _newVelocity.Set(_rigidbody2D.velocity.x, Mathf.Clamp(_rigidbody2D.velocity.y, -wallSlidingSpeed, float.MaxValue));
            _rigidbody2D.velocity = _newVelocity;
        } else if (_isGrounded && !_isOnSlope && !_isJumping) {
            _newVelocity.Set(movementSpeed * _input * _sprintModifier, _rigidbody2D.velocity.y);
            _rigidbody2D.velocity = _newVelocity;
        } else if (_isGrounded && _isOnSlope && !_isJumping) {
            _newVelocity.Set(movementSpeed * _slopeNormalPerp.x * -_input, movementSpeed * _slopeNormalPerp.y * -_input);
            _rigidbody2D.velocity = _newVelocity;
        } else if (!_isGrounded && !_isWallJumping || (!_isGrounded && _isWallJumping && _input != 0)) {
            _newVelocity.Set(movementSpeed * _input * _sprintModifier, _rigidbody2D.velocity.y);
            _rigidbody2D.velocity = _newVelocity;
        }
        
    }
}
