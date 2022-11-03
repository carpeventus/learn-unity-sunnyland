using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Parallax : MonoBehaviour {

    public Transform _camera;

    private Vector2 startPoint;

    public float rate;
    // Start is called before the first frame update
    void Start() {
        startPoint = transform.position;
    }

    // Update is called once per frame
    void Update() {
        transform.position = new Vector2(startPoint.x + _camera.position.x * rate, transform.position.y);
    }
}
