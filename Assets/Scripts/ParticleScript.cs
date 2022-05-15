using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        Destroy(gameObject, 2f);
    }

    // Update is called once per frame
    private void Update()
    {
        Destroy(gameObject, 2f);
    }
}