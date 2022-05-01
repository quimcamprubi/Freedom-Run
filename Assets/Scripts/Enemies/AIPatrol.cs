using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class AIPatrol : MonoBehaviour
{
    public float speed;
    public float groundCheckDistance = 2.0f;
    public bool isTouchingFront;
    public bool isGrounded;
    public bool isOverBoundary;
    public bool isTouchingPlayer;
    public Vector2 direction = Vector2.right;
    public Transform groundDetectorFront;
    public Transform groundDetectorBack;
    public LayerMask platformLayer;
    public LayerMask playerLayer;
    public BoxCollider2D boxCollider2D;
    public int distanceBoundary;
    private HealthMeter healthMeter;
    public Vector3 defaultPosition;
    public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start() {
        var healthMeterObj = GameObject.Find("HealthMeter");
        healthMeter = healthMeterObj.GetComponent<HealthMeter>();
        defaultPosition = transform.position;
    }

    // Update is called once per frame
    private void Update() {
    }

    private void FixedUpdate() { 
        transform.Translate(direction * speed * Time.deltaTime);
        isGrounded = Physics2D.Raycast(groundDetectorFront.position, Vector2.down, groundCheckDistance);
        isTouchingFront = boxCollider2D.IsTouchingLayers(platformLayer);
        isTouchingPlayer = boxCollider2D.IsTouchingLayers(playerLayer);
        isOverBoundary = distanceBoundary != 0 && transform.position.x > defaultPosition.x + distanceBoundary;
        if (isTouchingPlayer) {
            healthMeter.Hurt();
        }
        else if (!isGrounded || isTouchingFront || isOverBoundary) {
            speed *= -1;
            if (direction == Vector2.right) {
                transform.eulerAngles = new Vector3(0, 180, 0);
                direction = Vector2.left;
            } else {
                transform.eulerAngles = new Vector3(0, 0, 0);
                direction = Vector2.right;
            }
        }
    }


}
