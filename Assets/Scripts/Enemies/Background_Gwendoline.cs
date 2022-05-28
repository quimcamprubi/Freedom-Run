using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_Gwendoline : MonoBehaviour
{
    public Vector2 direction;
    private float _idleFlipTimer;
    public float currentSpeed;

    // Start is called before the first frame update
    void Start() {
        direction = Vector2.right;
        _idleFlipTimer = 0.0f;
        currentSpeed = -1f;
    }

    private void FixedUpdate() {
        transform.Translate(-direction * currentSpeed * Time.deltaTime);

        if (_idleFlipTimer >= 4.0f) {
            Flip();
            _idleFlipTimer = 0.0f;
        }
        else {
            _idleFlipTimer += Time.deltaTime;
        }
    }

    // Update is called once per frame
    private void Flip() {
        currentSpeed *= -1;
        if (direction == Vector2.right) {
            transform.eulerAngles = new Vector3(0, 180, 0);
            direction = Vector2.left;
        } else {
            transform.eulerAngles = new Vector3(0, 0, 0);
            direction = Vector2.right;
        }
    }
}
