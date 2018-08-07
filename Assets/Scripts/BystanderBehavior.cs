using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BystanderBehavior : MonoBehaviour {
    // color = new Color(Random.Range(100f, 200f), Random.Range(100f, 200f), Random.Range(100f, 200f))

    public enum modes { DummyPFT, MouseClick, IdleForever, Wandering};


    public string mindSequence;
    public List<bool> mindSequenceMask; // How many are known by the player

    public Bystander personality;
    public bool target = false;
    public modes mode = modes.DummyPFT;
    private int mindLength = 5;

    private NavMeshAgent agent;
    private GameController gameController;

    // Wandering
    private float wanderClock = 0;
    private float wanderMaxEvery = 6;
    private float wanderDistance = 3;

        // Use this for initialization
    void Start () {
        gameController = Camera.main.GetComponent<GameController>();
        agent = gameObject.GetComponent<NavMeshAgent>();

        // Applying personality parameters
        agent.speed = Random.Range(personality.minSpeed, personality.maxSpeed);

        // Generate mind sequence
        mindLength = (int) Mathf.Min(gameController.maxMindLength, mindLength);
        mindSequence = gameController.RegisterBystander();

        if (mindSequence.Length <= 0) {
            Destroy(gameObject);
            return;
        }
        else {
            // By default, player doesn't know the mind sequence of said bystander
            foreach(char _ in mindSequence) {
                mindSequenceMask.Add(false);
            }
        }

        if (mode == modes.Wandering) {
            wanderClock = Random.value * wanderMaxEvery;
        }
    }
	
	// Update is called once per frame
	void Update () {
        switch (mode){
            case modes.DummyPFT:
                agent.SetDestination(GameObject.FindGameObjectWithTag("DummyPFT").transform.position);
                break;

            case modes.MouseClick:
                if (Input.GetMouseButtonDown(0)) {
                    RaycastHit hit;

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)) {
                        agent.SetDestination(hit.point);
                    }
                }
                break;

            case modes.Wandering:
                if (wanderClock > wanderMaxEvery) {
                    // Wander...
                    Vector2 position = new Vector2();
                    float angle = Random.Range(0, 359);
                    position.x = Mathf.Cos(angle) * wanderDistance * Random.value + transform.position.x;
                    position.y = Mathf.Sin(angle) * wanderDistance * Random.value + transform.position.z;

                    agent.SetDestination(new Vector3(position.x, transform.position.y, position.y));
                    wanderClock = Random.value * wanderMaxEvery;
                }
                if (agent.remainingDistance < 1) {
                    wanderClock += Time.deltaTime;
                }
                break;
        }


    }
}
