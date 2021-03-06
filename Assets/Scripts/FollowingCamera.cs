﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour {

    [Header("Camera")]
    public Transform POI;
    public float mouseSensivity = 10f;
    public float objectiveCatchUpSpeed = 8f; // At what speed will the camera try to keep up with the objective
    public float optimalOffset;    // Distance between camera and POI at start
    public float startingRotation = 180f;
    public float cameraAdditionalHeight = 4f;
    public float cameraPitchMultiplier = 3f;

    private float offsetObjective;
    private float offset;
    private Vector3 unlimitedRotation;
    public float horizontalRotation; // Public for the only reason that the player rotation will need it
    private float horizontalRotationObjective; // The camera will will be lerped to this angle
    private GameController gameController;

    // Use this for initialization
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        offset = optimalOffset;
        offsetObjective = offset;
        transform.position = POI.position;
        horizontalRotation = startingRotation;
        horizontalRotationObjective = horizontalRotation;
        gameController = GetComponent<GameController>();
    }


    // Update is called once per frame
    void Update() {
    }

    void LateUpdate() {
        if (!gameController.paralyzed) horizontalRotationObjective += Input.GetAxis("HorizontalView") * mouseSensivity * Time.deltaTime;

        if (horizontalRotation != horizontalRotationObjective) {
            horizontalRotation = Mathf.Lerp(horizontalRotation, horizontalRotationObjective, objectiveCatchUpSpeed * Time.deltaTime);
        }
        if (offset != offsetObjective) {
            offset = Mathf.Lerp(offset, offsetObjective, objectiveCatchUpSpeed * Time.deltaTime);
        }

        transform.position = POI.position
                                + Quaternion.Euler(0f, horizontalRotation, 0f) * (offset * -Vector3.back)
                                + new Vector3(0, cameraAdditionalHeight + (offset - optimalOffset) * cameraPitchMultiplier);

        // Custom Lookat
        Vector3 direction = POI.position - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, objectiveCatchUpSpeed * Time.time);
    }

    public void MoveCamera(float moveOffset) {
        offsetObjective = optimalOffset + moveOffset;
    }

    public void ReplaceCamera() {
        offsetObjective = optimalOffset;
    }

    public void MoveCameraBetween(Transform target1, Transform target2, float moveOffset) {

        POI.GetComponent<CameraTarget>().freeze = true;

        Vector3 middlePoint = new Vector3(
            (target1.position.x + target2.position.x) / 2,
            (target1.position.y + target2.position.y) / 2,
            (target1.position.z + target2.position.z) / 2
        );
        
        if (!POI.position.Equals(middlePoint)) {
            POI.position = Vector3.Lerp(POI.position, middlePoint, objectiveCatchUpSpeed * Time.deltaTime);
        }
        MoveCamera(moveOffset);
    }
}