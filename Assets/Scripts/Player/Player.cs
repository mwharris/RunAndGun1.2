using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 movementInput = new Vector3(PlayerInput.Instance.Horizontal, 0, PlayerInput.Instance.Vertical);
        Vector3 movement = transform.rotation * movementInput;
        _characterController.SimpleMove(movement);
    }
}
