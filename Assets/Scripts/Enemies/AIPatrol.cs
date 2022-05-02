using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = System.Random;

public class AIPatrol : MonoBehaviour
{
    private Animator animator;
    private HealthMeter healthMeter;
    public Rigidbody2D rb;

    public LayerMask platformLayer;
    public LayerMask playerLayer;
    
    public Vector2 direction;
    public Vector3 defaultPosition;

    public float guardSpeed = 2.0f;
    private float _currentSpeed;
    public float groundCheckDistance = 2.0f;
    private float _idleFlipTimer;
    public float jumpForceY = 100.0f;
    public int distanceBoundary;

    public bool isDetectedPlayer;
    public bool isTouchingPlayer;
    public bool isTouchingFront;
    public bool isGrounded;
    public bool isOverBoundary;
    public bool isChasing;
    public bool playerOnRange;
    private bool _canJump;
    private bool _isJumping;
    
    public Transform groundDetectorFront;

    public BoxCollider2D playerDetector;
    public BoxCollider2D followRange;
    public BoxCollider2D attackerCollider2D;

    void Start() {
        var healthMeterObj = GameObject.Find("HealthMeter");
        healthMeter = healthMeterObj.GetComponent<HealthMeter>();
        defaultPosition = transform.position;
        direction = Vector2.right;
        _currentSpeed = 0;
        animator = GetComponent<Animator>();
        _idleFlipTimer = 0.0f;
        animator.SetBool("running", false);
        _canJump = true;
    }

    private void FixedUpdate() {
        DetectPlayer();

    }

    /* Check if the player is in the Field Of View of the Enemy */
    private void DetectPlayer() {
        isDetectedPlayer = playerDetector.IsTouchingLayers(LayerMask.GetMask("Player"));
   
        if (isChasing) {
            // Chases the player as long as he does not leave the Chasing Range, doesn't matter the FOV of the enemy
            playerOnRange = followRange.IsTouchingLayers(LayerMask.GetMask("Player"));
            if (playerOnRange) {
                // TODO: Follow the player
                Move();
                CheckAttack();
            } else {
                animator.SetBool("running", false);
                isChasing = false;
                _currentSpeed = 0.0f;
            }
        } else if (isDetectedPlayer) { 
            // Detects the player in the FOV, prepares to change the player in the Chasing Range 
            _idleFlipTimer = 0.0f;
            SurpiseJump();
            Invoke(nameof(StartChasing), 0.5f);
        } else if (!_isJumping) {
            // Keeps looking for the player while idle, flips FOV every 2 seconds
            if (_idleFlipTimer >= 2.0f) {
                Flip();
                _idleFlipTimer = 0.0f;
            } else {
                _idleFlipTimer += Time.deltaTime;
            }
        }
    }

    private void SurpiseJump() {
        if (_canJump) {
            _canJump = false;
            _isJumping = true;
            rb.AddForce(Vector2.up * jumpForceY);
        }
    }

    private void StartChasing() {
        _currentSpeed = (direction == Vector2.right) ? Math.Abs(guardSpeed) : -Math.Abs(guardSpeed);
        isChasing = true;
        _canJump = true;
        _isJumping = false;
        animator.SetBool("running", true); 
        
    }

    private void Move() {
        transform.Translate(direction * _currentSpeed * Time.deltaTime);
        
        CheckGround(); 
        CheckFront();
        CheckBoundary();

        /*  - If groundCheck is not touching the ground we need to flip the enemy, so he does not fall from the
            platforms.
            - If the enemy is touching a frontal platform, we need to flip him, so he does not run into the platform
            forever.
            - If the enemy is surpassing his boundaries (defined by a circumference of center=defaultPosition 
            and radius=distanceBoundary) we need to flip him so he does not escape the boundaries.
        */
        if (!isGrounded || isTouchingFront || isOverBoundary) {
            Flip();
        }
    }
    
    private void CheckAttack() {
        isTouchingPlayer = attackerCollider2D.IsTouchingLayers(playerLayer);
        if (isTouchingPlayer) {
            healthMeter.Hurt();
        }
    }
    
    private void CheckGround() {
        isGrounded = Physics2D.Raycast(groundDetectorFront.position,
            Vector2.down, groundCheckDistance);
    }
    
    private void CheckFront() {
        isTouchingFront = attackerCollider2D.IsTouchingLayers(platformLayer);
    }
    
    private void CheckBoundary() {
        isOverBoundary = distanceBoundary > 0 && transform.position.x > defaultPosition.x + distanceBoundary;
    }
    
    private void Flip() {
        _currentSpeed *= -1;
        if (direction == Vector2.right) {
            transform.eulerAngles = new Vector3(0, 180, 0);
            direction = Vector2.left;
        } else {
            transform.eulerAngles = new Vector3(0, 0, 0);
            direction = Vector2.right;
        }    
    }
}
