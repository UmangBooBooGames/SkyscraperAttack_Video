using UnityEngine;

public class SimpleOrbitar : MonoBehaviour
{
    public Camera mainCam; // assign your main camera here

    void Update()
    {
        RotateTowardsMouse();
    }

    void RotateTowardsMouse()
    {
        // Ray from mouse position into world
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

        // A plane at y = 0 (ground level)
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        float rayDistance;
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);

            // Direction from player to mouse point
            Vector3 direction = point - transform.position;
            direction.y = 0f; // keep only horizontal rotation

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
    }
}
