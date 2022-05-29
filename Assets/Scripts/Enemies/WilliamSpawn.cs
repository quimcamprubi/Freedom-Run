using UnityEngine;

public class WilliamSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject williams;
    public GameObject porron;
    public string sceneDestination;

    // Update is called once per frame
    private void Update()
    {
        if (!porron.activeSelf)
        {
            williams.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}