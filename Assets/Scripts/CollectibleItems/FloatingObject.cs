using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{

    public float Speed;
    private float Delay;


    private Rigidbody2D Rigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        
       Rigidbody2D = GetComponent<Rigidbody2D>();
       Delay = Time.time + 0.7f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time < Delay) {
            Rigidbody2D.velocity = Vector2.up * Speed;
        }
        else{
            //if(Delay < Time.time + 1) {
            //    Delay = Time.time + 1;
            //}
            Rigidbody2D.velocity = Vector2.down * Speed;
            if(Time.time > Delay + 0.7f){
                Delay = Time.time + 0.7f;
            }
            
        }
        
        
    }
}
