using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    [Header("Game")]
    public bool timeStopped = false;

    [Header("Investigation")]
    public List<string> sequenceElements;
    public int maxMindLength = 5;
    public List<char> discoveredSequence;
    public GameObject target;

    [Header("Player")]
    public bool paralyzed = false; // Freezes the player and takes away the camera control

    [Header("Bystanders")]
    public int maxBystanders = 200;
    private List<string> availableSequences;
    private List<GameObject> policemen = new List<GameObject>();

    public float suspicion = 3f;
    public float suspicionSpeed = 0.1f;
    public float suspicionCatchUpSpeed = 6f;
    private float suspicionObjective = 0f;


    private void Awake() {

        discoveredSequence = new List<char>(maxMindLength);
        for (int i = 0; i < maxMindLength; i++) {
            discoveredSequence.Add('?');
        }
        
        // Generate all the keys that will ever be used
        availableSequences = new List<string>();
        Debug.Log(Mathf.Pow(sequenceElements.Count, maxMindLength).ToString() + " possibilities of sequences");
        int toGenerate = (int)Mathf.Min(maxBystanders,  Mathf.Pow(sequenceElements.Count, maxMindLength));
        Debug.Log("Will generate "+toGenerate.ToString());
        bool firstLoop = false;
        for (int i = 0; i < toGenerate; i++) {
            string generated = RandomSequence(sequenceElements, maxMindLength);
            while (firstLoop || availableSequences.Contains(generated)) {
                firstLoop = false;
                generated = RandomSequence(sequenceElements, maxMindLength);
            }
            availableSequences.Add(generated);
        }
    }

    void Update () {
        // Picking a target
        if (target == null) {
            List<GameObject> bystanders = new List<GameObject>();
            GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject obj in objects){
                if (obj.GetComponents<BystanderBehavior>().Length > 0) {
                    bystanders.Add(obj);
                }
            }
            target = bystanders[(int)Mathf.Floor(Random.value * bystanders.Count)];
        }

        // Increasing suspicion overtime
        if (!timeStopped) {
            suspicion += Time.deltaTime* suspicionSpeed;
        }
    }

    public string RegisterBystander() {
        if (availableSequences.Count > 0) {
            string seq = availableSequences[0];
            availableSequences.RemoveAt(0);
            return seq;
        }
        else {
            Debug.Log("Fell short of sequences");
            return "";
        }
    }

    public void RegisterPoliceman(GameObject policeman) {
        policemen.Add(policeman);
    }

    string RandomSequence(List<string> sequenceElements, int maxMindLength) {
        string result = "";
        for (int i = 0; i < maxMindLength; i++) {
            result += sequenceElements[(int)Mathf.Floor(Random.value * sequenceElements.Count)];
        }
        return result;
    }

    public void IncreaseSuspicion(float amount) {
        suspicionObjective = suspicion + amount;
        StartCoroutine("UpdateSuspicion");
    }

    public void Investigate(Vector3 position) {
        foreach (GameObject policeman in policemen) {
            if (Vector3.Distance(policeman.transform.position, position) <= suspicion) {
                PolicemanBehavior policemanBehavior = policeman.GetComponent<PolicemanBehavior>();
                if (policemanBehavior.attitude == PolicemanBehavior.attitudes.Idling) {
                    policemanBehavior.StartInvestigation(position);
                }
                else {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    Vector2 policemanPosition = new Vector2(policeman.transform.position.x, policeman.transform.position.z);
                    Vector2 playerPosition = new Vector2(player.transform.position.x, player.transform.position.z);
                    Vector2 direction = (playerPosition - policemanPosition).normalized;
                    bool visible = Vector2.Angle(new Vector2(policeman.transform.forward.x, policeman.transform.forward.z), direction) < policemanBehavior.FOV / 2;
                    
                    // A raycast should be done aswell, ignoring people
                    // Also a max vision radius should be implemented
                    if (visible) {
                        policemanBehavior.StartAttacking();
                    }
                }
            }
        }
    }

    IEnumerator UpdateSuspicion() {
        while (suspicion < suspicionObjective) {
            if (!timeStopped) {
                suspicion = Mathf.Lerp(suspicion, suspicionObjective, Time.deltaTime);
            }
            yield return null;
        }
    }
}
