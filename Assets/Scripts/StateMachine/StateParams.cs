using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateParams : IStateParams
{
    public Vector3 Velocity { get; set; } = Vector3.zero;
}
