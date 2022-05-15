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
    private void Start()
    {
        transform.position = Pose[0].position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (transform.position == Pose[ID].position) ID += suma;

        if (ID == Pose.Length - 1) suma = -1;

        if (ID == 0) suma = 1;

        transform.position = Vector3.MoveTowards(transform.position, Pose[ID].position, speed * Time.deltaTime);
    }
}