using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSpikes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D other) {
        var healthMeterObj = GameObject.Find("HealthMeter");
        var healthMeter = healthMeterObj.GetComponent<HealthMeter>();
        healthMeter.Hurt();
    }
}
