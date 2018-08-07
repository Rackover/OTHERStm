using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    [Header("Codes")]
    public List<string> sequenceElements;
    public int maxMindLength = 5;

    [Header("Player")]
    public bool paralyzed = false; // Freezes the player and takes away the camera control

    [Header("Bystanders")]
    public int maxBystanders = 200;
    private List<string> availableSequences = new List<string>();
    
    private void Awake() {
        // Generate all the keys that will ever be used
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
        // Debug
        if (Input.GetKey(KeyCode.O)) {
            Cursor.visible = true;
            paralyzed = true;
        }
        if (Input.GetKey(KeyCode.P)) {
            Cursor.visible = false;
            paralyzed = false;
        }
        if (Input.GetKey(KeyCode.I)) {
            Debug.Log(availableSequences.ToString());
        }
    }

    public string RegisterBystander() {
        if (availableSequences.Count > 0) {
            string seq = availableSequences[0];
            availableSequences.RemoveAt(0);
            Debug.Log("Handed over a sequence");
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
