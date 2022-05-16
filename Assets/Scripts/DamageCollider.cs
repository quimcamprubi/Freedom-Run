using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    public int damage = 1;
    public bool has_critical_damage;
    private HealthMeter healthMeter;

    // Start is called before the first frame update
    private void Start()
    {
        var healthMeterObj = GameObject.Find("HealthMeter");
        healthMeter = healthMeterObj.GetComponent<HealthMeter>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject == healthMeter.playerObject)
        {
            if (has_critical_damage)
                healthMeter.Hurt(2);
            else
                healthMeter.Hurt();
        }
    }
}