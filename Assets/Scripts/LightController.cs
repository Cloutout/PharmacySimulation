using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the BillsManager for tracking light usage.")]
    public BillsManager billsManager;

    private Light _lightSource;
    private bool _isLightOn;

    private void Awake()
    {
        
        _lightSource = GetComponent<Light>();
        if (_lightSource == null)
        {
            Debug.LogError("Light component not found on the GameObject!");
        }

        
        if (billsManager == null)
        {
            billsManager = FindObjectOfType<BillsManager>();
            if (billsManager == null)
            {
                Debug.LogError("BillsManager is not assigned and could not be found in the scene!");
            }
        }
    }

    /// <summary>
    /// Işığı açar veya kapatır ve BillsManager'ı günceller.
    /// </summary>
    public void ToggleLight()
    {
        if (billsManager == null)
        {
            Debug.LogError("BillsManager is not assigned!");
            return;
        }

        _isLightOn = !_isLightOn;

        
        billsManager.RegisterLightSource(_isLightOn);

        
        if (_lightSource != null)
        {
            _lightSource.enabled = _isLightOn;
        }

        Debug.Log($"Light toggled. Current state: {(_isLightOn ? "On" : "Off")}");
    }
}