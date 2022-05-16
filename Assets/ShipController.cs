using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private int departDistance = 10;
    [SerializeField] private float moveSpeed;
    [SerializeField] private PlayerController playerController;
    
    private bool depart = false;
    private Rigidbody2D rb2d;
    private bool soundPlayed = false;

    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
    }
    
    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        //print("Distance to player: +" + distanceToPlayer);
        if (distanceToPlayer < departDistance) {
            if (!depart) PlaySound();
            depart = true;
        } 
    }

    void FixedUpdate() {
        if (depart) {
            PlaySound();
            rb2d.velocity = new Vector2(moveSpeed, 0);
        }
    }

    private void PlaySound() {
        if (!soundPlayed) playerController.ShipSound();
        soundPlayed = true;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        playerController.onMovingPlatform = true;
        playerController.platformSpeed = moveSpeed;
    }

    private void OnCollisionExit2D(Collision2D other) {
        playerController.onMovingPlatform = false;
        playerController.platformSpeed = 0;
    }

    
}
