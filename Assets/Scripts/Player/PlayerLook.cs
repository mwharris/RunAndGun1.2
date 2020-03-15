using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    public Camera PlayerCamera => playerCamera;

    private float _mouseSensitvity = 100f;
    private float _cameraXRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = PlayerInput.Instance.MouseX * _mouseSensitvity * Time.deltaTime;
        float mouseY = PlayerInput.Instance.MouseY * _mouseSensitvity * Time.deltaTime;

        _cameraXRotation -= mouseY;
        _cameraXRotation = Mathf.Clamp(_cameraXRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(_cameraXRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}