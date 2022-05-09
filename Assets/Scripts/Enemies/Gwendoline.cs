using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Cinemachine;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Gwendoline : MonoBehaviour {
    public float rollingSpeed;
    public float changeTime = 3.0f;
    public int damage = 1;
    public bool isBoss = false;
    
    private Rigidbody2D rigidbody2D;
    private float timer;
    private int direction = 1;
    private bool isRolling = false;
    private bool slowDown = false;
    private Animator animator;
    private HealthMeter healthMeter;
    private int prevDirection = 0;
    private int accelCounter = 0;
    private bool finishingRotation = false;
    private bool firstDrop = true;
    private bool isCoroutineExecuting = false;
    private float distanceToPlayer;

    [SerializeField] private float giveUpDistance;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float agroRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int accelerationRate = 8;

    void Start() 
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
        animator.SetFloat("Look X", direction);
        var healthMeterObj = GameObject.Find("HealthMeter");
        healthMeter = healthMeterObj.GetComponent<HealthMeter>();
    }

    void Update() {
        // Check distance to player
        distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        //print("Distance to player: +" + distanceToPlayer);
        if (distanceToPlayer < agroRange) {
            animator.SetBool("Is Rolling", true);
        } else if (distanceToPlayer > giveUpDistance || !isRolling) {
            if (isRolling) StopRolling();
            timer -= Time.deltaTime;
            if (timer < 0) {
                direction = -direction;
                timer = changeTime;
                animator.SetFloat("Look X", direction);
            }
        }
    }

    private void StartRolling() {
        isRolling = true;
        slowDown = false;
    }
    
    private void StopRolling() {
        isRolling = false;
        slowDown = true;
        finishingRotation = true;
    }

    void FixedUpdate() {
        if (isRolling) {
            print(Mathf.Abs(transform.position.y - playerTransform.position.y));
            if (isBoss && transform.position.y > playerTransform.position.y && Mathf.Abs(transform.position.y - playerTransform.position.y) > 9) { // Go right for the drop in level 5.
                if (firstDrop) {
                    if (prevDirection != 1) {
                        rigidbody2D.AddForce(new Vector2(moveSpeed * accelerationRate,0), ForceMode2D.Force);
                        accelCounter++;
                        if (accelCounter > 30) {
                            prevDirection = 1;
                            accelCounter = 0;
                        }
                    } else {
                        rigidbody2D.AddForce(new Vector2(moveSpeed,0), ForceMode2D.Force);
                        prevDirection = 1;
                    }
                    transform.Rotate(new Vector3(0, 0, -360 * Time.deltaTime));
                    StartCoroutine(ChangeDropDirection());
                } else {
                    if (prevDirection != -1) {
                        rigidbody2D.AddForce(new Vector2(-moveSpeed * accelerationRate,0), ForceMode2D.Force);
                        accelCounter++;
                        if (accelCounter > 30) {
                            prevDirection = -1;
                            accelCounter = 0;
                        }
                    } else {
                        rigidbody2D.AddForce(new Vector2(-moveSpeed,0), ForceMode2D.Force);
                        prevDirection = -1;
                    }
                    transform.Rotate(new Vector3(0, 0, 360 * Time.deltaTime));
                    StartCoroutine(ChangeDropDirection());
                }
            }
            else if (transform.position.x < playerTransform.position.x) { // Move right
                if (prevDirection != 1) {
                    rigidbody2D.AddForce(new Vector2(moveSpeed * accelerationRate,0), ForceMode2D.Force);
                    accelCounter++;
                    if (accelCounter > 30) {
                        prevDirection = 1;
                        accelCounter = 0;
                    }
                } else {
                    rigidbody2D.AddForce(new Vector2(moveSpeed,0), ForceMode2D.Force);
                    prevDirection = 1;
                }
                transform.Rotate(new Vector3(0, 0, -360 * Time.deltaTime));
                
            } else if (transform.position.x > playerTransform.position.x) { // Move left
                if (prevDirection != -1) {
                    rigidbody2D.AddForce(new Vector2(-moveSpeed * accelerationRate,0), ForceMode2D.Force);
                    accelCounter++;
                    if (accelCounter > 30) {
                        prevDirection = -1;
                        accelCounter = 0;
                    }
                } else {
                    rigidbody2D.AddForce(new Vector2(-moveSpeed,0), ForceMode2D.Force);
                    prevDirection = -1;
                }
                transform.Rotate(new Vector3(0, 0, 360 * Time.deltaTime));
            } 
        } else if (!isRolling && slowDown) {
            rigidbody2D.velocity = rigidbody2D.velocity * (0.99f * Time.deltaTime);
            if (finishingRotation) {
                Vector3 to = new Vector3(0f, 0f, 0f);
                if (Vector3.Distance(transform.eulerAngles, to) > 0.02f) {
                    transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, to, Time.deltaTime*3);
                } else {
                    transform.eulerAngles = to;
                    finishingRotation = false;
                    rigidbody2D.velocity = Vector2.zero;
                }
            }
            if (rigidbody2D.velocity.x == 0 && !finishingRotation) {
                slowDown = false;
                animator.SetBool("Is Rolling", false);
            }
        }
    }

    IEnumerator ChangeDropDirection() {
        if (isCoroutineExecuting)
            yield break;
        isCoroutineExecuting = true;
        yield return new WaitForSeconds(2.5f);
        firstDrop = false;
        print("Direction changed.");
        isCoroutineExecuting = false;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (isRolling && other.gameObject == healthMeter.playerObject) {
            healthMeter.Hurt(damage);
        }
    }
}
