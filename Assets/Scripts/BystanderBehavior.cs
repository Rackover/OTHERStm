using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BystanderBehavior : MonoBehaviour {
    // color = new Color(Random.Range(100f, 200f), Random.Range(100f, 200f), Random.Range(100f, 200f))

    public enum modes { DummyPFT, MouseClick, IdleForever, Wandering, Policeman};


    public string mindSequence;
    public List<bool> mindSequenceMask; // How many are known by the player
    public Bystander personality;
    public modes mode = modes.DummyPFT;
    public bool targeted = false;
    private int mindLength = 5;
    public float deathForce = 1000;

    private NavMeshAgent agent;
    private GameController gameController;

    // Wandering
    private float wanderClock = 0;
    private float wanderMaxEvery = 6;
    private float wanderDistance = 3;

    // Policeman
    public GameObject policemanDebugRangePrefab;
    private GameObject policemanDebugRangeInstance;
    private PolicemanBehavior policemanBehavior;

    // Use this for initialization
    void Start () {

        gameController = Camera.main.GetComponent<GameController>();
        agent = gameObject.GetComponent<NavMeshAgent>();

        // Applying personality parameters
        agent.speed = Random.Range(personality.minSpeed, personality.maxSpeed);

        // Generate mind sequence
        if (mode == modes.Policeman) {
            policemanDebugRangeInstance = Instantiate(policemanDebugRangePrefab, transform.position, transform.rotation, transform);
            policemanBehavior = gameObject.AddComponent<PolicemanBehavior>();
        }
        else {
            mindLength = (int)Mathf.Min(gameController.maxMindLength, mindLength);
            mindSequence = gameController.RegisterBystander();

            if (mindSequence.Length <= 0) {
                Destroy(gameObject);
                return;
            }
            else {
                // By default, player doesn't know the mind sequence of said bystander
                foreach (char _ in mindSequence) {
                    mindSequenceMask.Add(false);
                }
            }
            if (mode == modes.Wandering) {
                wanderClock = Random.value * wanderMaxEvery;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {

        // Debug
        Component halo = GetComponent("Halo");
        halo.GetType().GetProperty("enabled").SetValue(halo, targeted, null);
        // End of

        // When looking at someone, character can see the mind sequence
        if (targeted) {
            GetComponent<SequenceDisplay>().drawClock = 1f;
        }

        if (gameController.timeStopped) {
            agent.isStopped = true;
            return;
        }
        else {
            agent.isStopped = false;
        }

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

            case modes.Policeman:
                // Update range circle showing up
                policemanBehavior.UpdateVisualRadius(policemanDebugRangeInstance, gameController);
                switch (policemanBehavior.attitude) {

                    // Investigation - going to target
                    case PolicemanBehavior.attitudes.Investigating:
                        agent.SetDestination(policemanBehavior.investigationTarget);
                        if (agent.remainingDistance < 1f) {
                            policemanBehavior.attitude = PolicemanBehavior.attitudes.Idling;
                            agent.isStopped = true;
                        }
                        break;
                    
                    // Attacking the player - Game over soon
                    case PolicemanBehavior.attitudes.Attacking:

                        break;
                }
                break;
        }


    }

    public void Unmask(int amount) {
        for (int i = 0; i < (int)Mathf.Min(amount, mindSequence.Length); i++) {
            mindSequenceMask[i] = true;
        }
    }
    
    public void Die() {
        Component halo = GetComponent("Halo");
        halo.GetType().GetProperty("enabled").SetValue(halo, targeted, null);
        Rigidbody body = GetComponent<Rigidbody>();
        body.constraints = RigidbodyConstraints.None;
        body.AddForce(new Vector3(
            Random.value * deathForce - deathForce/2,
            Random.value * deathForce - deathForce / 2,
            Random.value * deathForce - deathForce / 2
        ));
        body.AddTorque(new Vector3(
            (Random.value * deathForce - deathForce / 2)/100,
            (Random.value * deathForce - deathForce / 2)/100,
            (Random.value * deathForce - deathForce / 2)/100
        ));

        Destroy(GetComponent<NavMeshAgent>());
        Destroy(GetComponent<BystanderBehavior>());
    }

    public void UpdateKnowledge(List<char> knowledge) {
        string targetSequence = Camera.main.GetComponent<GameController>().target.GetComponent<BystanderBehavior>().mindSequence;
        for (int i = 0; i < mindSequence.Length; i++) {
            char element = mindSequence[i];
            char targetElement = targetSequence[i];
            if (element.Equals(targetElement)) {
                knowledge[i] = element;
            }
        }
    }
}
