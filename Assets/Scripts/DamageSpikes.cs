using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSpikes : MonoBehaviour
{
    public bool has_critical_damage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D other) {
        // Necessari per a que quan una caixa (o qualsevol altre objecte que no sigui la griselda) no li tregui vida
        // Nomes funciona assignant-li el tag player a la griselda
        if (other.gameObject.CompareTag("Player")) {
            var healthMeterObj = GameObject.Find("HealthMeter");
            var healthMeter = healthMeterObj.GetComponent<HealthMeter>();
            if (has_critical_damage) {
                healthMeter.Hurt(2);
            } else {
                healthMeter.Hurt();
            }

        }
    }
}
