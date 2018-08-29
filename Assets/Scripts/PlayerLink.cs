using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLink : MonoBehaviour {

    public Image crosshair;
    public int rayAmount = 6;
    public float linkMaxDistance = 4f;
    public Material linkMaterial;
    public float linkWidth = 0.1f;
    public float linkZoomBackDistance = 4f;
    public float suspicionIncrement = 3f;

    private bool linking = false;
    private GameObject currentTarget;

    private LineRenderer line;                           // Line Renderer

    // Use this for initialization
    void Start() {
        crosshair.enabled = false;
        line = this.gameObject.AddComponent<LineRenderer>();
        // Set the width of the Line Renderer
        line.startWidth = linkWidth;
        line.endWidth = linkWidth;
        line.positionCount = 2;
        line.material = linkMaterial;
    }

	// Update is called once per frame
	void Update () {

        if (linking) {
            line.enabled = true;
            line.SetPosition(0, currentTarget.transform.position);
            line.SetPosition(1, transform.position);
            return;
        }

        line.enabled = false;
        ResetTarget(currentTarget);
        crosshair.enabled = false;

        if (GetComponent<PlayerMovement>().currentSpeed.magnitude <= 0.01f) {

            // DEBUG
            //crosshair.enabled = true;

            // Finding a target
            List<GameObject> targets = new List<GameObject>();

            for (int i = 0; i < rayAmount; i++) {

                Vector2 cursor = new Vector2(Camera.main.pixelWidth / 2, i*(Camera.main.pixelHeight/rayAmount));

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(cursor);

                if (Physics.Raycast(ray, out hit)) {
                    GameObject target = hit.transform.gameObject;
                    // Checking if it's a bystander
                    if (target.GetComponent<BystanderBehavior>() != null) {
                        targets.Add(target);
                    }
                }
            }

            if (targets.Count > 0) {
                // Taking the closest for target
                float nearest = 0f;
                GameObject referenceTarget = null;
                foreach (GameObject target in targets) {
                    float distance = Vector3.Distance(target.transform.position, transform.position);
                    if (target.GetComponent<BystanderBehavior>().mode != BystanderBehavior.modes.Policeman && 
                        (referenceTarget == null || distance < nearest) && 
                        linkMaxDistance > distance) {

                        referenceTarget = target;
                        nearest = distance;
                    }
                }

                if (referenceTarget != null) {
                    referenceTarget.GetComponent<BystanderBehavior>().targeted = true;
                    if (currentTarget != referenceTarget) {
                        ResetTarget(currentTarget);
                    }
                    currentTarget = referenceTarget;
                }
            }

            /// If there's a target, allow for linking
            if (currentTarget != null && Input.GetButton("Link")) {
                linking = true;
                StartCoroutine("LinkTarget", currentTarget);
            }
        }
    }
    

    void ResetTarget(GameObject lastTarget) {
        if (lastTarget != null) {
            lastTarget.GetComponent<BystanderBehavior>().targeted = false;
        }
    }

    private IEnumerator LinkTarget (GameObject target) {
        GameController controller = Camera.main.GetComponent<GameController>();
        target.GetComponent<BystanderBehavior>().UpdateKnowledge(Camera.main.GetComponent<GameController>().discoveredSequence);
        while (Input.GetButton("Link")) {
            controller.timeStopped = true;
            Camera.main.GetComponent<FollowingCamera>().MoveCameraBetween(gameObject.transform, target.transform, linkZoomBackDistance);
            target.GetComponent<BystanderBehavior>().Unmask(Camera.main.GetComponent<GameController>().maxMindLength);
            target.GetComponent<SequenceDisplay>().drawClock = 2f; 
            yield return null;
        }
        controller.timeStopped = false;
        controller.IncreaseSuspicion(suspicionIncrement);
        controller.Investigate(target.transform.position);
        target.GetComponent<BystanderBehavior>().Die();
        currentTarget = null;
        linking = false;
        yield return true;
    }
    
}
