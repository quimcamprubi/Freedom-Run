using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public class Parallax : MonoBehaviour
{
    private float _length, _startPosition;
    public GameObject cam;
    public float parallaxEffect;
    private void Start() {
        _startPosition = transform.position.x;
        _length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void FixedUpdate() {
        float temp = cam.transform.position.x * (1 - parallaxEffect);
        var dist = cam.transform.position.x * parallaxEffect;
        transform.position = new Vector3(_startPosition + dist, transform.position.y, transform.position.z);
        if (temp > _startPosition + _length) {
            _startPosition += _length;
        } else if (temp < _startPosition - _length) {
            _startPosition -= _length;
        }
    }
}
