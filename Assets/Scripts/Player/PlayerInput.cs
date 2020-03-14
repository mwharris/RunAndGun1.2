﻿using UnityEngine;

public class PlayerInput : MonoBehaviour, IPlayerInput
{
    public static IPlayerInput Instance { get; set; }
    
    public float Vertical => Input.GetAxis("Vertical");
    public float Horizontal => Input.GetAxis("Horizontal");
    public float MouseX => Input.GetAxis("Mouse X");

    private void Awake()
    {
        Instance = this;
    }

    public void Tick()
    {
        // TODO: ...
    }
}