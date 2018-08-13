using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour {

    public Transform characterBody; // Will follow this transform POSITION without inheriting the rotation
    public float catchUpSpeed = 8f;
    public float heightAboveHead = 2f;
    public bool freeze = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (freeze) {
            freeze = false;
            return;
        }
		if (!Vector3.Equals(characterBody.position, transform.position)) {
            transform.position = Vector3.Lerp(
                new Vector3(
                    characterBody.position.x,
                    characterBody.position.y+heightAboveHead,
                    characterBody.position.z
                ),
                transform.position, 
                catchUpSpeed*Time.deltaTime
            );
        }
	}
}
