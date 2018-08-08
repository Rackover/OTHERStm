using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveBehavior : MonoBehaviour {

    public float maxRange = 5f;
    public float speed = 1f;
    public float showTagsFor = 1f;

    private List<GameObject> alreadyDone = new List<GameObject>();
    private float range = 0.4f;
    private SphereCollider waveCollider;
    private float lerpSafetyMultiplier = 1.2f;

    // Use this for initialization
    void Start() {
        waveCollider = GetComponent<SphereCollider>();
        Camera.main.GetComponent<GameController>().timeStopped = true;
    }

    // Update is called once per frame
    void Update() {

        Camera.main.GetComponent<FollowingCamera>().MoveCamera(5f);

        waveCollider.radius = range;
        range = Mathf.Lerp(range, maxRange, Mathf.Sin(((range)/maxRange)*Mathf.PI)* speed * Time.deltaTime);
        if (range > maxRange/lerpSafetyMultiplier && !Input.GetButton("Shockwave")) {
            Camera.main.GetComponent<GameController>().timeStopped = false;
            foreach(GameObject bystander in alreadyDone) {
                bystander.GetComponent<SequenceDisplay>().drawClock = showTagsFor;
            }
            Destroy(gameObject);
        }

        ///
        /// Debug
        ///

        GetComponent<Light>().range = range;
        GetComponent<Light>().intensity = 1000*(maxRange - range/maxRange);
    }
    private void OnTriggerEnter(Collider collision) {
        // Check if it is a bystander
        if (collision.gameObject.GetComponents<BystanderBehavior>().Length > 0 && !alreadyDone.Contains(collision.gameObject)) {
            // If so, let's unmask part of his code
            alreadyDone.Add(collision.gameObject);
            BystanderBehavior behavior = collision.gameObject.GetComponent<BystanderBehavior>();
            int unmaskAmount = (int)Mathf.Clamp(behavior.mindSequence.Length - behavior.mindSequence.Length * (range / maxRange)+1, 0, behavior.mindSequence.Length);
            behavior.Unmask(unmaskAmount);
            collision.gameObject.GetComponent<SequenceDisplay>().drawClock = 3600f; // very high value (forever)
        }
    }
}