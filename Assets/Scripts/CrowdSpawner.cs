using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdSpawner : MonoBehaviour {

    public int bypasserAmount = 5;
    public int bypasserRange = 4;
    public Bystander[] personalities;
    public BystanderBehavior.modes[] modes;

    public GameObject bystanderPrefab;

    // Use this for initialization
    void Start () {
        for (int i = 0; i < bypasserAmount; i++) {
            Vector2 position = new Vector2();
            float angle = Random.Range(0, 359);
            position.x = Mathf.Cos(angle) * bypasserRange * Random.value + transform.position.x;
            position.y = Mathf.Sin(angle) * bypasserRange*Random.value + transform.position.z;
            GameObject bystander = GameObject.Instantiate(bystanderPrefab);
            bystander.GetComponent<Transform>().position = new Vector3(position.x, transform.position.y, position.y);   // random position within circle
            Bystander choice = personalities[(int)Mathf.Floor(Random.value * personalities.Length)];
            bystander.GetComponent<BystanderBehavior>().personality = choice; // Random personality
            Debug.Log(bystander.GetComponent<BystanderBehavior>().personality);
            bystander.GetComponent<BystanderBehavior>().mode = modes[(int)Mathf.Floor(Random.value * modes.Length)]; // Random mode
        }

        // Killing the spawner
        Destroy(gameObject);
    }
	
}
