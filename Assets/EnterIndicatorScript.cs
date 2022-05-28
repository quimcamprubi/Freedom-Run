using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnterIndicatorScript : MonoBehaviour
{
     [SerializeField]
     private TextMeshProUGUI textMeshPro;

    // Start is called before the first frame update
    void Start()
    {
        var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        textMeshPro.text = gameManager.UsingGamepad ? "B" : "Enter";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
