using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class porronScript : MonoBehaviour
{
    public bool canRotate = false;
    public int damage = 2;
    public float RotSpeed;

    private bool hit = false;
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (canRotate) transform.Rotate(0, 0, RotSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        canRotate = false;
        if(!hit){
            var controllerEnemy = col.gameObject.GetComponent<AIPatrol>();
            if (controllerEnemy != null)
            {
                hit = true;
                this.GetComponent<Rigidbody2D>().velocity=Vector2.zero;
                controllerEnemy.takeDamage(damage);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var controller = other.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.AvailablePorron();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var controller = other.GetComponent<PlayerController>();
        if (controller != null) controller.NoAvailablePorron();
    }
}