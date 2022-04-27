using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    public int damage = 1;

    private HealthMeter healthMeter;

    // Start is called before the first frame update
    void Start()
    {
        var healthMeterObj = GameObject.Find("HealthMeter");
        healthMeter = healthMeterObj.GetComponent<HealthMeter>();
    }

    void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject == healthMeter.playerObject)
            healthMeter.Hurt(damage);
    }
}
