using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerInput
{
    float Vertical { get; }
    float Horizontal { get; }
    float MouseX { get; }
}
