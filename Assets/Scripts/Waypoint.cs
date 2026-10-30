using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Tooltip("Rotate the enemy when it reaches this waypoint?")]
    public bool changeRotation = true;

    [Tooltip("Target Z rotation applied when an enemy reaches this point")]
    public float rotationZ;
}