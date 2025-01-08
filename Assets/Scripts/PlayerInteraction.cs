using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 3f; 
    [SerializeField] private Transform holdPosition; 
    [SerializeField] private LayerMask interactableMask; 
    [SerializeField] private float throwForce = 5f; 

    private GameObject _heldObject; 
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Ensure your scene has a camera tagged as 'MainCamera'.");
        }
    }

    private void Update()
    {
        HandleInteractionInput();
        HandleObjectPlacement();
        UpdateHeldObjectPosition();
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            if (_heldObject == null)
            {
                TryPickupObject();
            }
            else
            {
                DropObject();
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && _heldObject != null) 
        {
            ThrowObject();
        }
    }

    private void HandleObjectPlacement()
    {
        if (Input.GetMouseButtonDown(0) && _heldObject != null)
        {
            PlaceObject();
        }
    }

    private void UpdateHeldObjectPosition()
    {
        if (_heldObject != null)
        {
            _heldObject.transform.position = holdPosition.position; 
        }
    }

    private void TryPickupObject()
    {
        if (_mainCamera == null) return;

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableMask))
        {
            var hitObject = hit.collider.gameObject;
            if (hitObject.TryGetComponent(out Rigidbody objectRb))
            {
                
                _heldObject = hitObject;
                objectRb.isKinematic = true; 
                _heldObject.transform.SetParent(holdPosition); 
                _heldObject.transform.localPosition = Vector3.zero;
            }
        }
    }

    private void DropObject()
    {
        if (_heldObject == null) return;

        if (_heldObject.TryGetComponent(out Rigidbody objectRb))
        {
            objectRb.isKinematic = false; 
        }

        _heldObject.transform.SetParent(null); 
        _heldObject = null;
    }

    private void PlaceObject()
    {
        if (_mainCamera == null || _heldObject == null) return;

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            _heldObject.transform.position = hit.point; 
            DropObject();
        }
    }

    private void ThrowObject()
    {
        if (_heldObject == null) return;

        if (_heldObject.TryGetComponent(out Rigidbody objectRb))
        {
            objectRb.isKinematic = false; 
            objectRb.AddForce(_mainCamera.transform.forward * throwForce, ForceMode.Impulse); 
        }

        _heldObject.transform.SetParent(null); 
        _heldObject = null;
    }
}
