using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float movementSpeed = 10f;

    public float zoomSpeed = 10f;

    public float maxZoomDistance = 20f;
    public float minZoomDistance = 5f;

    private float currentZoomDistance = 10f;

    public float rotationSpeed = 1f;

    private bool isMiddleMouseButtonHeld = false;

    void Update()
    {
        Vector3 moveDirection = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= transform.right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= transform.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += transform.right;
        }
        transform.position += moveDirection.normalized * movementSpeed * Time.deltaTime;

        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        currentZoomDistance -= zoomInput * zoomSpeed;
        currentZoomDistance = Mathf.Clamp(currentZoomDistance, minZoomDistance, maxZoomDistance);
        transform.position = new Vector3(transform.position.x, currentZoomDistance, transform.position.z);

        if (Input.GetMouseButtonDown(2))
        {
            isMiddleMouseButtonHeld = true;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isMiddleMouseButtonHeld = false;
        }

        if (isMiddleMouseButtonHeld)
        {
            float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed;
            float verticalRotation = Input.GetAxis("Mouse Y") * rotationSpeed;

            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x = Mathf.Clamp(eulerAngles.x - verticalRotation, 10f, 80f);
            eulerAngles.y += horizontalRotation;
            transform.eulerAngles = eulerAngles;
        }
    }
}
