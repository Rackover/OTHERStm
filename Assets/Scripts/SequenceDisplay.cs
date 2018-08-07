using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequenceDisplay : MonoBehaviour {

    public TextMesh textComponent;
    public float maxDrawDistance = 3f;
    public float minDrawDistance = 1f;

    private BystanderBehavior behavior;
    private Transform player;

    // Use this for initialization
    void Start () {
        behavior = GetComponent<BystanderBehavior>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
	
	// Update is called once per frame
	void Update () {

        // Updating alpha
        float alpha = 1;

        alpha = Mathf.Clamp(minDrawDistance + maxDrawDistance - Vector3.Distance(transform.position, player.position), 0f, 1f);

        if (alpha < 0) {
            return;
        }

        textComponent.color = new Color(
            textComponent.color.r,
            textComponent.color.g,
            textComponent.color.b,
            alpha
        );
        
        // Updating text
        string tag = "";
        for (int i = 0; i < behavior.mindSequence.Length; i++) {
            if (i > 0) {
                tag += "\n";
            }

            string letter;

            if (behavior.mindSequenceMask[i]) {
                letter = System.Convert.ToString(behavior.mindSequence[i]);
            }
            else {
                letter = "?";
            }
            tag += letter;
        }
        textComponent.text = tag;
    }
}
