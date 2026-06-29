using UnityEngine;
using UnityEngine.InputSystem;

public class MouseDragObject : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private Plane dragPlane;
    private Vector3 offset;

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("No Main Camera found. Make sure your camera tag is set to MainCamera.");
        }
    }

    void Update()
    {
        if (mainCamera == null || Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryStartDrag();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        if (isDragging && Mouse.current.leftButton.isPressed)
        {
            DragObject();
        }
    }

    private void TryStartDrag()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                dragPlane = new Plane(Vector3.up, transform.position);

                if (dragPlane.Raycast(ray, out float enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    offset = transform.position - hitPoint;
                    isDragging = true;
                }
            }
        }
    }

    private void DragObject()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            transform.position = hitPoint + offset;
        }
    }
}