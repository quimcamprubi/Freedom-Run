using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class William : MonoBehaviour
{
    // Start is called before the first frame update
	public GameObject williams;
	public GameObject porron;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!porron.activeSelf){ williams.SetActive(true); }
    }
	
}
