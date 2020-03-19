using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateParams : IStateParams
{
    public Vector3 Velocity { get; set; }
    public bool Sprinting { get; set; } = false;
}
