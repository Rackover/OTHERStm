using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	
	public float walkMaximumSpeed = 10f;
    public float lerpAcceleration = 2f;
    public float turnSpeed = 50f;
    public float catchUpSpeed;
    public float cameraMovementMultiplier = 10f;

    private Vector2 currentSpeed = new Vector2();
    private FollowingCamera cameraScript;
    private GameController gameController;

    private void Start() {
        cameraScript = Camera.main.GetComponent<FollowingCamera>();
        gameController = Camera.main.GetComponent<GameController>();
    }

    void Update() {

        float cameraAngle = cameraScript.horizontalRotation;
        float maxSpeed = walkMaximumSpeed;
        /*
            if running{
            maxspeed = runMaximumSpeed    
        }
        */

        transform.Translate(currentSpeed.x, 0, currentSpeed.y);

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (gameController.paralyzed) {
            input = new Vector2();
        }

        // Looking in the right direction
        if (input.x != 0f || input.y != 0f) {
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
                input.x * maxSpeed * Time.deltaTime,
                lerpAcceleration * Time.deltaTime
            );
        }
        if (currentSpeed.y < maxSpeed && currentSpeed.y > -maxSpeed) {
            currentSpeed.y = Mathf.Lerp(
                currentSpeed.y,
                input.y * maxSpeed * Time.deltaTime,
                lerpAcceleration * Time.deltaTime
            );
        }

        // Camera effect
        float cameraDistance = Mathf.Clamp(Mathf.Abs(currentSpeed.x) + Mathf.Abs(currentSpeed.y), -maxSpeed*Time.deltaTime, maxSpeed*Time.deltaTime);
        cameraScript.MoveCamera(cameraDistance*cameraMovementMultiplier);
    }
}
