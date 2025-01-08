using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Maximum distance for ray casting.")]
    public float maxRayDistance = 3f;

    [Tooltip("Camera used for ray casting and item holding.")]
    public Camera playerCamera;

    [Header("Item Hold Settings")]
    [Tooltip("Position offset for held items.")]
    public Vector3 holdOffset = new Vector3(0, -0.5f, 1.5f);

    [Tooltip("Rotation offset for held items.")]
    public Vector3 holdRotation = Vector3.zero;

    private GameObject _pickedObject;

    private void Update()
    {
        HandleInteractionInput();
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Check for "E" key press
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
            {
                Debug.DrawRay(ray.origin, ray.direction * maxRayDistance, Color.green, 1f);

                if (hit.transform.CompareTag("Pickable"))
                {
                    PickUpOrDropObject(hit.transform.gameObject);
                }
            }
        }
    }

    private void PickUpOrDropObject(GameObject obj)
    {
        if (_pickedObject == null)
        {
            PickUpObject(obj);
        }
        else
        {
            DropObject();
        }
    }

    private void PickUpObject(GameObject obj)
    {
        _pickedObject = obj;

        // Disable physics interactions
        Rigidbody objRigidbody = _pickedObject.GetComponent<Rigidbody>();
        if (objRigidbody != null)
        {
            objRigidbody.isKinematic = true;
        }

        // Parent to camera and set position/rotation
        _pickedObject.transform.SetParent(playerCamera.transform);
        _pickedObject.transform.localPosition = holdOffset;
        _pickedObject.transform.localRotation = Quaternion.Euler(holdRotation);

        Debug.Log($"Picked up: {_pickedObject.name}");
    }

    private void DropObject()
    {
        if (_pickedObject != null)
        {
            // Re-enable physics interactions
            Rigidbody objRigidbody = _pickedObject.GetComponent<Rigidbody>();
            if (objRigidbody != null)
            {
                objRigidbody.isKinematic = false;
            }

            // Detach from camera
            _pickedObject.transform.SetParent(null);
            _pickedObject = null;

            Debug.Log("Dropped the object.");
        }
    }
}
