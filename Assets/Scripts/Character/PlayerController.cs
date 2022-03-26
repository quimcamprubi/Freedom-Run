using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    // Public attributes
    public float movementSpeed;
    public float jumpSpeed = 8.0f;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask platformLayer;
    public float slopeCheckDistance;

    // Private attributes
    private bool _isSprinting = false;
    private bool _isGrounded;
    private bool _isJumping;
    private bool _canJump;
    private bool _sprintJump;
    private bool _isOnSlope;
    private bool _sprintFall;

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

    void Start() 
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _colliderSize = _capsuleCollider.size;
    }

    // Update is called once per frame
    void Update() {
        CheckInput();
    }

    private void FixedUpdate() {
        CheckGround();
        CheckSlope();
        ApplyMovement();
    }

    private void CheckInput() {
        _input = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump")) Jump();
        _isSprinting = Input.GetKey(KeyCode.LeftShift);
    }

    private void CheckGround() {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayer);
        if (_rigidbody2D.velocity.y <= 0.0f) {
            _isJumping = false;
        }
        if (_isGrounded && !_isJumping) {
            _canJump = true;
            _sprintJump = false;
        }
    }

    private void Jump() {
        if (_canJump && Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayer)) {
            _sprintJump = _isSprinting;
            _canJump = false;
            _isJumping = true;
            _newForce.Set(0.0f, jumpSpeed);
            _rigidbody2D.AddForce(_newForce, ForceMode2D.Impulse);
        }
    }

    private void CheckSprintModifier() {
        if (_isGrounded) {
            _sprintModifier = _isSprinting ? 2.0f : 1.0f;
            _sprintFall = _isSprinting;
        } else {
            _sprintModifier = _sprintJump || _sprintFall ? 2.0f : 1.0f;
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
            Debug.DrawRay(hit.point, _slopeNormalPerp, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
    }
    
    private void ApplyMovement() {
        CheckSprintModifier();
        if (_isGrounded && !_isOnSlope && !_isJumping) {
            _newVelocity.Set(movementSpeed * _input * _sprintModifier, _rigidbody2D.velocity.y);
            _rigidbody2D.velocity = _newVelocity;
        } else if (_isGrounded && _isOnSlope) {
            _newVelocity.Set(movementSpeed * _slopeNormalPerp.x * -_input, movementSpeed * _slopeNormalPerp.y * -_input);
            _rigidbody2D.velocity = _newVelocity;
        } else if (!_isGrounded) {
            _newVelocity.Set(movementSpeed * _input * _sprintModifier, _rigidbody2D.velocity.y);
            _rigidbody2D.velocity = _newVelocity;
        }
    }
}
