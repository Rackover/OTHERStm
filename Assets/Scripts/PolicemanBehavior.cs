using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolicemanBehavior : MonoBehaviour {

    public enum attitudes { Idling, Investigating, Attacking };
    public attitudes attitude = attitudes.Idling;
    public Vector3 investigationTarget;
    public float FOV = 50f;

    public void Start() {
        Camera.main.GetComponent<GameController>().RegisterPoliceman(gameObject);
    }

    public void UpdateVisualRadius(GameObject instance, GameController controller) {
        Circle circle = instance.GetComponent<Circle>();
        circle.radius = controller.suspicion;
        circle.UpdatePoints();
    }

    public void StartInvestigation(Vector3 position) {
        if (attitude != attitudes.Attacking) {
            investigationTarget = position;
            attitude = attitudes.Investigating;
        }
    }

    public void StartAttacking() {
        attitude = attitudes.Attacking;
    }
}
