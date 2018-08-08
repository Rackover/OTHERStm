using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Bystander", menuName = "Bystander character", order = 1)]
public class Bystander : ScriptableObject {
    public string model = "BystanderModel";
    public float minSpeed = 1f;
    public float maxSpeed = 2f;
}