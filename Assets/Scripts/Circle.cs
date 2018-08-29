using UnityEngine;
using System.Collections;

public class Circle : MonoBehaviour {
    public int segments;
    public float radius;
    public LineRenderer lineRenderer;

    void Start() {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = false;
        UpdatePoints();
    }


    public void UpdatePoints() {
        float x;
        float y = 0f;
        float z;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++) {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, y, z));

            angle += (360f / segments);
        }
    }
}