using Unity.Cinemachine;
using UnityEngine;

public class CameraMouseContoller : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float maxSensitivity = 100f;
    public float minSensitivity = 20f; // sensitivity when zoomed in fully

    [Header("Zoom Settings")]
    public CinemachineCamera vcam;
    public float zoomSpeed = 5f;
    public float minFOV = 10f;
    public float maxFOV = 40f;
    public float zoomSmoothTime = 10f; // higher = smoother

    private float rotationX;
    private float rotationY;

    private bool cursorLocked = false;
    private float targetFOV;
    private float currentFOV;

    void Start()
    {
        // Initialize rotation from editor
        Vector3 startRotation = transform.rotation.eulerAngles;
        rotationX = startRotation.x;
        rotationY = startRotation.y;

        vcam = GetComponent<CinemachineCamera>();
        // Initialize FOV
        if (vcam != null)
        {
            currentFOV = vcam.Lens.FieldOfView;
            targetFOV = currentFOV;
        }

        UnlockCursor();
    }

    void Update()
    {
        HandleCursorLock();

        if (cursorLocked)
        {
            HandleMouseLook();
        }

        HandleZoom();
    }

    void HandleCursorLock()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LockCursor();
        }

        if (Input.GetMouseButtonUp(0))
        {
            UnlockCursor();
            this.enabled = false;
            HeadshotHook.Instance.ShowBulletCinematic();
        }
    }

    void HandleMouseLook()
    {
        if (vcam == null) return;

        // Sensitivity scaled based on zoom
        float t = Mathf.InverseLerp(minFOV, maxFOV, currentFOV);
        float sensitivity = Mathf.Lerp(minSensitivity, maxSensitivity, t);

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -80f, 80f);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

    void HandleZoom()
    {
        if (vcam == null) return;

        // Scroll input
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetFOV -= scroll * zoomSpeed * 10f;
            targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
        }

        // Smooth transition
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * zoomSmoothTime);
        vcam.Lens.FieldOfView = currentFOV;
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorLocked = true;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorLocked = false;
    }
}

