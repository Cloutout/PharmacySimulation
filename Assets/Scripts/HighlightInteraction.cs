using UnityEngine;

public class HighlightInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [Tooltip("Maximum distance for raycasting.")]
    public float maxRayDistance = 3f;

    [Tooltip("Player camera used for raycasting.")]
    public Camera playerCamera;

    [Header("Highlight Settings")]
    [Tooltip("Material used to highlight objects.")]
    public Material highlightMaterial;

    private GameObject _highlightedObject; // Currently highlighted object
    private Material _originalMaterial;   // Original material of the highlighted object

    private void Update()
    {
        HandleRaycast();
    }

    private void HandleRaycast()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
        {
            // Check if the object is interactable
            if (hit.transform.CompareTag("Pickable"))
            {
                HighlightObject(hit.transform.gameObject);
            }
            else
            {
                ResetHighlight();
            }
        }
        else
        {
            ResetHighlight();
        }
    }

    private void HighlightObject(GameObject obj)
    {
        // If the object is already highlighted, do nothing
        if (_highlightedObject == obj) return;

        // Reset previously highlighted object
        ResetHighlight();

        // Highlight the new object
        Renderer objRenderer = obj.GetComponent<Renderer>();
        if (objRenderer != null)
        {
            _originalMaterial = objRenderer.material; // Store the original material
            objRenderer.material = highlightMaterial; // Apply the highlight material
            _highlightedObject = obj; // Set as the current highlighted object
        }
    }

    private void ResetHighlight()
    {
        if (_highlightedObject == null) return;

        // Restore the original material
        Renderer objRenderer = _highlightedObject.GetComponent<Renderer>();
        if (objRenderer != null)
        {
            objRenderer.material = _originalMaterial;
        }

        _highlightedObject = null; // Clear the highlighted object reference
    }
}
