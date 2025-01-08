using TMPro;
using UnityEngine;

public class UIUpdater : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Displays the player's current balance.")]
    public TextMeshProUGUI balanceText;

    [Tooltip("Displays the current day.")]
    public TextMeshProUGUI dayText;

    private SaveManager _saveManager;

    private void Start()
    {
        _saveManager = SaveManager.Instance;

        
        if (_saveManager == null)
        {
            Debug.LogError("SaveManager instance is null!");
            return;
        }

        if (balanceText == null)
        {
            Debug.LogError("Balance text component is not assigned!");
            return;
        }

        if (dayText == null)
        {
            Debug.LogError("Day text component is not assigned!");
            return;
        }

       
        _saveManager.OnBalanceUpdated += UpdateBalanceUI;
        _saveManager.OnDayUpdated += UpdateDayUI;

        
        UpdateBalanceUI(_saveManager.playerBalance);
        UpdateDayUI(_saveManager.currentDay);
    }

    private void UpdateBalanceUI(float newBalance)
    {
        balanceText.text = $"Money: {newBalance:F2}$"; // Format with two decimal places for clarity
    }

    private void UpdateDayUI(int currentDay)
    {
        dayText.text = $"Day: {currentDay}";
    }

    private void OnDestroy()
    {
         
        if (_saveManager != null)
        {
            _saveManager.OnBalanceUpdated -= UpdateBalanceUI;
            _saveManager.OnDayUpdated -= UpdateDayUI;
        }
    }
}