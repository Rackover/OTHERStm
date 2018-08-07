using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPathfinderTargetMovement : MonoBehaviour {

    private float angle = 0f;
    private float distance = 20f;
    private Vector3 offset;
    private float speed = 0.1f;

	// Use this for initialization
	void Start () {
        offset = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        angle = (angle + speed * Time.deltaTime) % 360;
        transform.position = offset + new Vector3(distance * Mathf.Cos(angle), 0, distance * Mathf.Sin(angle));
    }
}
