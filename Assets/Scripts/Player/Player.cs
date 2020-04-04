using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform _playerBody;
    public Transform PlayerBody => _playerBody;
}