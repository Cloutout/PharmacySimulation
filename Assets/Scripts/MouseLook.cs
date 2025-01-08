using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 100f; 
    [SerializeField] private Transform playerBody;

    private float _xRotation;

    
    public bool isUIActive;

    private void Start()
    {
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        
        if (isUIActive)
            return;

        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Clamp vertical rotation to avoid flipping
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        
        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up, mouseX);
    }

    /// <summary>
    /// Adjusts the mouse sensitivity dynamically, e.g., from a slider in the UI.
    /// </summary>
    /// <param name="sensitivity">New sensitivity value</param>
    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
    }
}