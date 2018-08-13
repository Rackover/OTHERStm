using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequenceDisplay : MonoBehaviour {

    public GameObject displayer;
    /*
        No longer needed :(
    public float maxDrawDistance = 3f;
    public float minDrawDistance = 1f;
    */
    public float drawClock = 0f;

    private BystanderBehavior behavior;
    private Transform player;
    private TextMesh textComponent;

    // Use this for initialization
    void Start () {
        behavior = GetComponent<BystanderBehavior>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        textComponent = displayer.GetComponent<TextMesh>();
    }
	
	// Update is called once per frame
	void Update () {

        if (drawClock > 0f) {
            drawClock -= Time.deltaTime;
        }

        // Updating alpha
        float alpha = 1;
        /*
        No longer needed :(
        alpha = Mathf.Clamp(minDrawDistance + maxDrawDistance - Vector3.Distance(transform.position, player.position), 0f, 1f);

        */
        if (alpha < 0) {
            return;
        }

        textComponent.color = new Color(
            textComponent.color.r,
            textComponent.color.g,
            textComponent.color.b,
            alpha*Mathf.Min(drawClock, 1)
        );
        
        // Updating text
        string tag = "";
        List<char> playerKnowledge = Camera.main.GetComponent<GameController>().discoveredSequence;

        for (int i = 0; i < behavior.mindSequence.Length; i++) {
            if (i > 0) {
                tag += "\n";
            }

            string letter;

            if (behavior.mindSequenceMask[i]) {
                letter = System.Convert.ToString(behavior.mindSequence[i]);

                // Letter has been discovered to be part of target
                if (playerKnowledge[i].Equals(behavior.mindSequence[i])) {
                    string alphaHex = ((int)Mathf.Max(0, Mathf.Floor(alpha * Mathf.Min(drawClock, 1) * 255))).ToString("x2");
                    string color = "#ffff00" + alphaHex;
                    letter = "<color=\"" + color + "\">" + letter + "</color>";

                }
            }
            else {
                letter = "?";
            }

            tag += letter;
        }
        textComponent.text = tag;

        // Updating direction
        displayer.transform.LookAt(
            2f * transform.position - new Vector3(
                Camera.main.transform.position.x,
                displayer.transform.position.y,
                Camera.main.transform.position.z
            )
        );
    }
}
