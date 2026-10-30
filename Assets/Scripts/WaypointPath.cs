using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    public static WaypointPath Instance { get; private set; }

    [Tooltip("Order matters: Waypoint 1 to 6, then BaseEnd last")]
    public Waypoint[] waypoints;

    void Awake()
    {
        Instance = this;
    }
}