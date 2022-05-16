using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] Pose;
    public float speed;
    public int ID;
    public int suma;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Pose[0].position;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position == Pose[ID].position){
            ID += suma;
        }

        if(ID == Pose.Length-1){
            suma = -1;
        }

        if(ID == 0){
            suma = 1;
        }

        transform.position = Vector3.MoveTowards(transform.position, Pose[ID].position, speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        other.transform.SetParent(transform, true);
    }

    private void OnCollisionExit2D(Collision2D other) {
        other.transform.SetParent(null);
        other.transform.localScale = new Vector3(1, 1, 1);
    }
}
