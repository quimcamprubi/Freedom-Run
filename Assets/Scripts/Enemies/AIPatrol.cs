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
    private Vector2 enemyPosition;
    private Vector2 playerPosition;
    public Vector3 defaultPosition;
    private Vector2 _colliderSize;
    private Vector2 _slopeNormalPerp;

    public float guardSpeed = 2.0f;
    public float currentSpeed = 0.0f;
    public float groundCheckDistance = 2.0f;
    private float _idleFlipTimer;
    public float jumpForceY = 100.0f;
    public float flipDelay = 0.2f;
    private float _dazedTime;
    public float startDazedTime = 0.6f;
    public float playerDetectionRange = 10f;
    private float _slopeDownAngle;
    private float _slopeSideAngle;
    private float _slopeDownAngleOld;

    public int health = 3;
    public int distanceBoundary;

    public bool isDetectedPlayer;
    public bool isTouchingPlayer;
    public bool isTouchingFront;
    public bool isGrounded;
    public bool isOverBoundary;
    public bool isPlayerOverBoundary;
    public bool isChasing;
    public bool playerOnRange;
    private bool _canJump;
    public bool isReturning;
    public bool _isOnSlope;
    public bool _isJumping;
    public bool mustFlip;
    private bool _is_alive = true;

    public Transform groundDetectorFront;
    public Transform groundDetector;

    public BoxCollider2D followRange;
    public BoxCollider2D attackerCollider2D;
    public GameObject targetPlayer;
    private CapsuleCollider2D _capsuleCollider;

    private void Start()
    {
        var healthMeterObj = GameObject.Find("HealthMeter");
        healthMeter = healthMeterObj.GetComponent<HealthMeter>();
        defaultPosition = transform.position;
        direction = Vector2.right;
        currentSpeed = 0;
        animator = GetComponent<Animator>();
        _idleFlipTimer = 0.0f;
        animator.SetBool("running", false);
        _canJump = true;
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _colliderSize = _capsuleCollider.size;
    }

    private void FixedUpdate()
    {
        CheckFrontGround();
        CheckSlope();
        CheckJumping();
        CheckFront();
        CheckBoundary();
        DetectPlayer();
        CheckHealth();

        if (isReturning)
        {
            if (Math.Abs(defaultPosition.x - transform.position.x) <= 2)
            {
                isReturning = false;
                currentSpeed = 0.0f;
            }

            if (defaultPosition.x > transform.position.x && direction == Vector2.left
                || defaultPosition.x < transform.position.x && direction == Vector2.right)
            {
                CancelInvoke(nameof(Flip));
                mustFlip = true;
                Flip();
            }
            else
            {
                transform.Translate(direction * 3 * Math.Sign(currentSpeed) * Time.deltaTime);
            }
        }
        else
        {
            if (_dazedTime <= 0)
            {
                currentSpeed = currentSpeed = direction == Vector2.right ? Math.Abs(guardSpeed) : -Math.Abs(guardSpeed);
            }
            else
            {
                currentSpeed = 0;
                _dazedTime -= Time.deltaTime;
            }
        }
    }

    private void CheckHealth()
    {
        if (health <= 0)
        {
            transform.RotateAround(groundDetectorFront.localPosition, new Vector3(0, 1, 0), Time.deltaTime * 10);
            if (_is_alive)
            {
                _is_alive = false;
                Invoke(nameof(Die), 0.5f);
            }
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    /* Check if the player is in the Field Of View of the Enemy */
    private void DetectPlayer()
    {
        isDetectedPlayer = Physics2D.Raycast(transform.position, direction, playerDetectionRange,
                               LayerMask.GetMask("Player")) && !Physics2D.Raycast(transform.position, direction,
                               playerDetectionRange, LayerMask.GetMask("Platforms"))
                           || Physics2D.Raycast(transform.position, -direction, 2,
                               LayerMask.GetMask("Player"))
                           && !Physics2D.Raycast(transform.position, -direction, 2,
                               LayerMask.GetMask("Platforms"));
        Debug.DrawRay(transform.position, direction * playerDetectionRange, Color.red);
        Debug.DrawRay(transform.position, -direction * 2, Color.red);

        if (isChasing)
        {
            // isChasing is false by default, only set if player is detected.

            playerOnRange = followRange.IsTouchingLayers(LayerMask.GetMask("Player"));

            /* Check if player is in the chasing range (doesn't matter the playerDetector range, cause we already detected him)
               As long as the player does not leave the chasing range, or does not surpass a enemy boundary, or does not
               hide between platforms then we will need to chase the player.*/
            if (playerOnRange && targetPlayer.CompareTag("Player") && !isPlayerOverBoundary && !isTouchingFront)
            {
                /* We check it's the player, so that he doesn't start following us when other enemies are around. */
                Move();
                CheckAttack();
            }
            else
            {
                isReturning = true;
                animator.SetBool("running", false);
                isChasing = false;
            }
        }
        else if (isDetectedPlayer && !isPlayerOverBoundary)
        {
            /* If the player is detected, The enemy is no longer patrolling, we have to reset the flipTimer, so that when
               he starts patrolling again he still has to wait 2 seconds for flipping */
            _idleFlipTimer = 0.0f;

            /* If the player is detected and the enemy is not facing the player, we flip the enemy to face the player */
            if (targetPlayer.transform.position.x > transform.position.x && direction == Vector2.left
                || targetPlayer.transform.position.x < transform.position.x && direction == Vector2.right)
                Flip();
            /* Then the enemy performs a surprise jump, an anticipation to let some time to the player to start running. */
            Jump(Vector2.up * jumpForceY);

            /* The Invoke is needed to let some time between the enemy's surprise-jump and the chase. */
            Invoke(nameof(StartChasing), 0.5f);
        }
        else if (!_isJumping && !isReturning)
        {
            // Keeps looking for the player while idle, flips FOV every 2 seconds
            if (_idleFlipTimer >= 2.0f)
            {
                Flip();
                _idleFlipTimer = 0.0f;
            }
            else
            {
                _idleFlipTimer += Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// Performs a jump, intensity specified by jumpVector
    /// </summary>
    private void Jump(Vector2 jumpVector)
    {
        if (_canJump)
        {
            _canJump = false;
            rb.AddForce(jumpVector);
        }
    }

    /// <summary>
    /// Prepares to chase the player within the chasing range.
    /// </summary>
    private void StartChasing()
    {
        currentSpeed = direction == Vector2.right ? Math.Abs(guardSpeed) : -Math.Abs(guardSpeed);
        isChasing = true;
        _canJump = true;
        isReturning = false;
        animator.SetBool("running", true);
    }

    /// <summary>
    /// Move towards the player 
    /// </summary>
    private void Move()
    {
        /* We need to face the player to follow him, if we are not facing him then we need to flip the enemy.
           we also need to do so with a delay, to let the player time to run into the other direction.
           This is only performed if we aren't already flipping (mustFlip = false) */
        if (!mustFlip && (targetPlayer.transform.position.x > transform.position.x && direction == Vector2.left ||
                          targetPlayer.transform.position.x < transform.position.x && direction == Vector2.right))
        {
            /* Flip is not performed until a time delay passes, so to avoid entering this conditional more than once 
               and flipping more than once, we set mustFlip to true, which will be set to false by Flip() once 
               the flip is done, letting us enter this conditional again if necessary*/
            mustFlip = true;
            Invoke(nameof(Flip), flipDelay);
        }

        if (_isOnSlope)
            transform.Translate(-direction * currentSpeed * _slopeNormalPerp * Time.deltaTime);
        else
            transform.Translate(direction * currentSpeed * Time.deltaTime);

        /*  - If groundCheck is not touching the ground on the front (not the same as jumping!)
            we need to flip the enemy, so he doesn't fall into the void.
            - If the enemy is touching a frontal platform, we need to flip him, so he does not run into the platform
            forever.
            - If the enemy is surpassing his boundaries (defined by a circumference of center=defaultPosition 
            and radius=distanceBoundary) we need to flip him so he does not escape the boundaries.
        */
        if (!_isOnSlope && (!isGrounded && !_isJumping || isTouchingFront || isOverBoundary))
        {
            CancelInvoke(nameof(Flip));
            mustFlip = true;
            Flip();
        }
    }

    /// <summary>
    /// Checks whether the player is jumping.
    /// </summary>
    private void CheckJumping()
    {
        _isJumping = !Physics2D.Raycast(groundDetector.position,
            Vector2.down, 0.5f, LayerMask.GetMask("Platforms"));
        Debug.DrawRay(groundDetector.position, Vector2.down * 0.5f, Color.blue);
    }

    /// <summary>
    /// Check if attack on the player can be performed, if so, hurt the player.
    /// </summary>
    private void CheckAttack()
    {
        isTouchingPlayer = attackerCollider2D.IsTouchingLayers(playerLayer);
        if (isTouchingPlayer) healthMeter.Hurt();
    }

    private void CheckFrontGround()
    {
        isGrounded = Physics2D.Raycast(groundDetectorFront.position,
            Vector2.down, groundCheckDistance, LayerMask.GetMask("Platforms"));
        Debug.DrawRay(groundDetectorFront.position, Vector2.down * groundCheckDistance, Color.green);
    }

    private void CheckFront()
    {
        isTouchingFront = attackerCollider2D.IsTouchingLayers(platformLayer);
    }

    private void CheckBoundary()
    {
        enemyPosition = transform.position;
        playerPosition = targetPlayer.transform.position;
        isOverBoundary = distanceBoundary > 0 && (enemyPosition.x > defaultPosition.x + distanceBoundary
                                                  || enemyPosition.x < defaultPosition.x - distanceBoundary);
        isPlayerOverBoundary = distanceBoundary > 0 && (playerPosition.x > defaultPosition.x + distanceBoundary
                                                        || playerPosition.x < defaultPosition.x - distanceBoundary);
    }

    private void Flip()
    {
        currentSpeed *= -1;
        if (direction == Vector2.right)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            direction = Vector2.left;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            direction = Vector2.right;
        }

        if (mustFlip) mustFlip = false;
    }

    public void takeDamage(int damage)
    {
        _dazedTime = startDazedTime;
        health -= damage;
        rb.velocity = Vector3.zero;
        Jump(new Vector2(300 * targetPlayer.transform.localScale.x, 500));
        _idleFlipTimer = 0.0f;
        StartChasing();
    }

    private void CheckSlope()
    {
        Vector2 checkPosition = transform.position - (Vector3) new Vector2(0.0f, _colliderSize.y / 2);
        HorizontalCheckSlope(checkPosition);
        VerticalCheckSlope(checkPosition);
    }

    private void HorizontalCheckSlope(Vector2 checkPosition)
    {
        var slopeHitFront = Physics2D.Raycast(checkPosition, transform.right, 0.8f, platformLayer);
        var slopeHitBack = Physics2D.Raycast(checkPosition, -transform.right, 0.8f, platformLayer);
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
        var hit = Physics2D.Raycast(checkPosition, Vector2.down, 0.8f, platformLayer);
        if (hit)
        {
            _slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            _slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (!Mathf.Approximately(_slopeDownAngle, _slopeDownAngleOld)) _isOnSlope = true;
            _slopeDownAngleOld = _slopeDownAngle;
        }

        if (_isOnSlope && _slopeDownAngle != 0.0f)
            rb.gravityScale = 0.0f;
        else
            rb.gravityScale = 5.0f;
    }
}