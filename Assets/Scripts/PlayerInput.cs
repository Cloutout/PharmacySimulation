using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Input Settings")]
    [Tooltip("Amount to add when the 'P' key is pressed.")]
    public float addAmount = 100f;

    [Tooltip("Amount to subtract when the 'O' key is pressed.")]
    public float subtractAmount = 50f;

    private SaveManager _saveManager;

    private void Start()
    {
        // Cache the SaveManager instance
        _saveManager = SaveManager.Instance;

        if (_saveManager == null)
        {
            Debug.LogError("SaveManager instance is null! Please ensure SaveManager exists in the scene.");
        }
    }

    private void Update()
    {
        HandleBalanceInput();
    }

    private void HandleBalanceInput()
    {
        if (_saveManager == null) return;

        // Add balance when "P" is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            UpdatePlayerBalance(addAmount, "Added");
        }

        // Subtract balance when "O" is pressed
        if (Input.GetKeyDown(KeyCode.O))
        {
            UpdatePlayerBalance(-subtractAmount, "Subtracted");
        }
    }

    private void UpdatePlayerBalance(float amount, string action)
    {
        _saveManager.UpdatePlayerBalance(amount);
        Debug.Log($"{action} {Mathf.Abs(amount)} balance. New balance: {_saveManager.playerBalance}");
    }
}