using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform playerBody;   // The parent object (for horizontal rotation)
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;  // vertical rotation (pitch)

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // hide cursor
    }

    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Vertical rotation (pitch)
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -45f, 45f); // clamp up/down if desired

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal rotation (yaw)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
