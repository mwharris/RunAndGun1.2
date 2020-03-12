using UnityEngine;

public class StateParams
{
    public Vector3 Velocity { get; set; }
    
    public StateParams(Vector3 v)
    {
        Velocity = v;
    }
}