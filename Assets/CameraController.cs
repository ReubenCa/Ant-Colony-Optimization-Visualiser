using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 2f;
    public float zoomSpeed = 2f;
    public float minY = 2f;
    public float maxY = 10f;
    public float minX = -5f;
    public float maxX = 5f;
    public float MaxZoom = 10f;
    public float MinZoom = 2f;

    [SerializeField]
    private bool useMouseControls = true; // Toggle for mouse controls

    void Update()
    {
        if (useMouseControls)
        {
            // Camera movement using mouse position
            PanCamera();

            // Camera zoom using scroll wheel
            ZoomCamera();
        }
        else
        {
            // Camera movement using WASD keys
            MoveWithKeys();

            // Camera zoom using scroll wheel
            ZoomCamera();
        }

        // Toggle mouse controls
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            useMouseControls = !useMouseControls;
        }
    }

    void PanCamera()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 moveDirection = Vector3.zero;

        // Horizontal movement
        if (mousePosition.x < Screen.width * 0.05f)
        {
            moveDirection.x = -1;
        }
        else if (mousePosition.x > Screen.width * 0.95f)
        {
            moveDirection.x = 1;
        }

        // Vertical movement
        if (mousePosition.y < Screen.height * 0.05f)
        {
            moveDirection.y = -1;
        }
        else if (mousePosition.y > Screen.height * 0.95f)
        {
            moveDirection.y = 1;
        }

        // Normalize the direction to prevent faster diagonal movement
        moveDirection.Normalize();

        // Move the camera
        transform.Translate(moveDirection * panSpeed * Time.deltaTime);

        // Clamp the position to stay within the specified range
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minX, maxX),
            Mathf.Clamp(transform.position.y, minY, maxY),
            transform.position.z
        );
    }

    void MoveWithKeys()
    {
        Vector3 moveDirection = Vector3.zero;

        // Horizontal movement
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection.x = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveDirection.x = -1;
        }

        // Vertical movement
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection.y = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveDirection.y = -1;
        }

        // Normalize the direction to prevent faster diagonal movement
        moveDirection.Normalize();

        // Move the camera
        transform.Translate(moveDirection * panSpeed * Time.deltaTime);

        // Clamp the position to stay within the specified range
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minX, maxX),
            Mathf.Clamp(transform.position.y, minY, maxY),
            transform.position.z
        );
    }

    void ZoomCamera()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float newSize = Camera.main.orthographicSize - scroll * zoomSpeed;

        // Clamp the zoom level
        newSize = Mathf.Clamp(newSize, MinZoom, MaxZoom);

        // Apply the new size
        Camera.main.orthographicSize = newSize;
    }
}
