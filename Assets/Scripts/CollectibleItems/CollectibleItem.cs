using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectibleItem : MonoBehaviour {
    public string collectibleItemId;

    public abstract void OnTriggerEnter2D(Collider2D other);
    public abstract void OnTriggerExit2D(Collider2D other);
}
