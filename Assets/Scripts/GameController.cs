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

    [Header("Player")]
    public bool paralyzed = false; // Freezes the player and takes away the camera control

    [Header("Bystanders")]
    public int maxBystanders = 200;
    private List<string> availableSequences;
    public GameObject target;
    
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

    string RandomSequence(List<string> sequenceElements, int maxMindLength) {
        string result = "";
        for (int i = 0; i < maxMindLength; i++) {
            result += sequenceElements[(int)Mathf.Floor(Random.value * sequenceElements.Count)];
        }
        return result;
    }
}
