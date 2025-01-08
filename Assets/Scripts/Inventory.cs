using UnityEngine;

public class Inventory : MonoBehaviour
{
    private SaveManager _saveManager;

    void Start()
    {
        _saveManager = SaveManager.Instance;
        if (_saveManager == null)
        {
            Debug.LogError("SaveManager instance not found!");
            return;
        }

        
        var inventory = _saveManager.GetInventory();
        
        foreach (var item in inventory)
        {
            Debug.Log($"Product: {item.Key}, Quantity: {item.Value}");
        }
    }

    public void AddToInventory(string itemName, int quantity)
    {
        _saveManager.UpdateInventory(itemName, quantity);
    }

    private void RemoveFromInventory(string itemName, int quantity)
    {
        _saveManager.UpdateInventory(itemName, -quantity);
    }

    private int GetItemQuantity(string itemName)
    {
        return _saveManager.GetInventoryQuantity(itemName);
    }

    
    public void SellItem(string itemName, int quantity, int salePricePerUnit)
    {
        int currentQuantity = GetItemQuantity(itemName);
        if (currentQuantity >= quantity)
        {
            RemoveFromInventory(itemName, quantity);
            int totalSalePrice = quantity * salePricePerUnit;
            _saveManager.UpdatePlayerBalance(totalSalePrice);
            Debug.Log($"{quantity} units of {itemName} sold. Earnings: {totalSalePrice}");
        }
        else
        {
            Debug.LogWarning($"Not enough stock of {itemName}.");
        }
    }
}