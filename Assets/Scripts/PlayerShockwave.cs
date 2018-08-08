using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShockwave : MonoBehaviour {


    public float cooldown = 1f;
    public GameObject shockwavePrefab;
    public float maxRange = 5f;
    public float speed = 1f;
    public float showTagsFor = 1f;

    private bool canShock = true;


    private void Start () {
		
	}


    private void Update() {
        if (Input.GetButtonDown("Shockwave")) {
            if (canShock) {
                StartCoroutine("Shockwave");
            }
        }
	}


    private IEnumerator Shockwave() {
        canShock = false;
        GameObject instance = GameObject.Instantiate(shockwavePrefab);
        instance.transform.position = transform.position;
        instance.transform.parent = transform;
        instance.GetComponent<ShockwaveBehavior>().maxRange = maxRange;
        instance.GetComponent<ShockwaveBehavior>().speed = speed;
        instance.GetComponent<ShockwaveBehavior>().showTagsFor = showTagsFor;
        yield return new WaitForSeconds(cooldown);
        canShock = true;
        yield return false;
    }
}
