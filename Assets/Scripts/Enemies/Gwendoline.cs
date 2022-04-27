using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Gwendoline : MonoBehaviour {
    public float speed;
    private Rigidbody2D _rigidbody2D;
    // Start is called before the first frame update
    void Start() 
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate() 
    {
        Vector2 position = _rigidbody2D.position;
        position.x = position.x + Time.deltaTime * speed;
        
        _rigidbody2D.MovePosition(position);
    }
}
