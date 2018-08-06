using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	
	public float walkMaximumSpeed = 10f;
    public float lerpAcceleration = 2f;
    public float turnSpeed = 50f;
    public FollowingCamera cameraScript;
    public float catchUpSpeed;
    public float cameraMovementMultiplier = 10f;

    private Vector2 currentSpeed = new Vector2();

    void Update() {

        float cameraAngle = cameraScript.horizontalRotation;
        float maxSpeed = walkMaximumSpeed;
        /*
            if running{
            maxspeed = runMaximumSpeed    
        }
        */

        // Looking in the right direction
        if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f) {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0f, cameraAngle - 180, 0f),
                catchUpSpeed *Time.deltaTime
            );
        }

        // Effective Movement
        if (currentSpeed.x < maxSpeed && currentSpeed.x > -maxSpeed) {
            currentSpeed.x = Mathf.Lerp(
                currentSpeed.x,
                Input.GetAxis("Horizontal") * maxSpeed * Time.deltaTime,
                lerpAcceleration * Time.deltaTime
            );
        }
        if (currentSpeed.y < maxSpeed && currentSpeed.y > -maxSpeed) {
            currentSpeed.y = Mathf.Lerp(
                currentSpeed.y,
                Input.GetAxis("Vertical") * maxSpeed * Time.deltaTime,
                lerpAcceleration * Time.deltaTime
            );
        }
        transform.Translate(currentSpeed.x, 0, currentSpeed.y);

        // Camera effect
        float cameraDistance = Mathf.Clamp(Mathf.Abs(currentSpeed.x) + Mathf.Abs(currentSpeed.y), -maxSpeed*Time.deltaTime, maxSpeed*Time.deltaTime);
        cameraScript.MoveCamera(cameraDistance*cameraMovementMultiplier);
    }
}
