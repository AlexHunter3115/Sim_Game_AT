using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInteraction : MonoBehaviour
{

    [SerializeField] MapCreation map;


    void Update()
    {
        // Check if the mouse button was clicked this frame
        if (Input.GetMouseButtonDown(0))
        {
            // Create a ray from the camera through the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray intersects with any colliders
            if (Physics.Raycast(ray, out hit))
            {
                // Log the position of the intersection point
                Debug.Log("Intersection point: " + hit.point);
            }
        }
    }
}
