using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Player Data")]
    public float playerBalance;
    public int currentDay;

    [Header("Inventory")]
    private readonly Dictionary<string, int> _inventory = new Dictionary<string, int>();

    [Header("Upgrades")]
    public List<string> unlockedUpgrades = new List<string>();

    [Header("Daily Stats")]
    public int dailyEarnings;
    public int dailyExpenses;

    [Header("Settings")]
    public float volume = 1.0f;
    public int resolutionIndex;
    public int qualityLevel = 2;

    public delegate void BalanceUpdatedHandler(float newBalance);
    public event BalanceUpdatedHandler OnBalanceUpdated;

    public delegate void DayUpdatedHandler(int newDay);
    public event DayUpdatedHandler OnDayUpdated;

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, "game_save.json");

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (!LoadGame())
            {
                InitializeDefaultValues();
                Debug.LogWarning("Save dosyası bulunamadı. Varsayılan değerler yüklendi.");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDefaultValues()
    {
        playerBalance = 1000; 
        currentDay = 1;
        _inventory.Clear();
        unlockedUpgrades.Clear();
        dailyEarnings = 0;
        dailyExpenses = 0;
        volume = 1.0f;
        resolutionIndex = 0;
        qualityLevel = 2;
        SaveGame();
    }

    public void NewGame()
    {
        InitializeDefaultValues();
        Debug.Log("Yeni oyun başlatıldı.");
    }

    public bool LoadGame()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.LogWarning("Save dosyası bulunamadı.");
            return false;
        }

        try
        {
            string json = File.ReadAllText(SaveFilePath);
            var saveData = JsonUtility.FromJson<SaveData>(json);

            if (saveData == null)
            {
                Debug.LogError("Save verisi çözümlenemedi.");
                return false;
            }

            playerBalance = saveData.playerBalance;
            currentDay = saveData.currentDay;

            _inventory.Clear();
            if (saveData.inventoryItems != null)
            {
                foreach (var item in saveData.inventoryItems)
                {
                    _inventory[item.itemName] = item.quantity;
                }
            }

            unlockedUpgrades = saveData.unlockedUpgrades ?? new List<string>();
            dailyEarnings = saveData.dailyEarnings;
            dailyExpenses = saveData.dailyExpenses;
            volume = saveData.volume;
            resolutionIndex = saveData.resolutionIndex;
            qualityLevel = saveData.qualityLevel;

            Debug.Log("Oyun başarıyla yüklendi.");

            OnBalanceUpdated?.Invoke(playerBalance);
            OnDayUpdated?.Invoke(currentDay);

            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Save dosyası yüklenirken hata oluştu: {ex.Message}");
            return false;
        }
    }

    public void SaveGame()
    {
        var saveData = new SaveData
        {
            playerBalance = playerBalance,
            currentDay = currentDay,
            inventoryItems = new List<InventoryItem>(),
            unlockedUpgrades = new List<string>(unlockedUpgrades),
            dailyEarnings = dailyEarnings,
            dailyExpenses = dailyExpenses,
            volume = volume,
            resolutionIndex = resolutionIndex,
            qualityLevel = qualityLevel
        };

        foreach (var item in _inventory)
        {
            saveData.inventoryItems.Add(new InventoryItem
            {
                itemName = item.Key,
                quantity = item.Value
            });
        }

        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(SaveFilePath, json);
            Debug.Log($"Oyun {SaveFilePath} konumuna kaydedildi.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Oyun kaydedilirken hata oluştu: {ex.Message}");
        }
    }

    public bool HasSave()
    {
        return File.Exists(SaveFilePath);
    }

    public void ResetDailyValues()
    {
        dailyEarnings = 0;
        dailyExpenses = 0;
        SaveGame();
    }

    public void UpdatePlayerBalance(float amount)
    {
        playerBalance += amount;
        SaveGame();
        OnBalanceUpdated?.Invoke(playerBalance);
    }

    public void UpdateInventory(string itemName, int quantityChange)
    {
        if (_inventory.ContainsKey(itemName))
        {
            int newQuantity = _inventory[itemName] + quantityChange;
            _inventory[itemName] = Mathf.Max(0, newQuantity);
        }
        else if (quantityChange > 0)
        {
            _inventory[itemName] = quantityChange;
        }
        else
        {
            Debug.LogWarning($"Envanterde {itemName} bulunmuyor. {quantityChange} miktarında çıkartılamaz.");
        }

        SaveGame();
    }

    public int GetInventoryQuantity(string itemName)
    {
        if (_inventory.ContainsKey(itemName))
        {
            return _inventory[itemName];
        }
        else
        {
            return 0;
        }
    }

    public Dictionary<string, int> GetInventory()
    {
        return new Dictionary<string, int>(_inventory);
    }

    public void AddToExpenses(int amount)
    {
        dailyExpenses += amount;
        Debug.Log("Günlük harcama güncellendi: " + dailyExpenses);
        SaveGame();
    }

    public void UpdateDay(int amount)
    {
        currentDay += amount;
        SaveGame();
        OnDayUpdated?.Invoke(currentDay);
    }
}

[System.Serializable]
public class SaveData
{
    public float playerBalance;
    public int currentDay;
    public List<InventoryItem> inventoryItems;
    public List<string> unlockedUpgrades;
    public int dailyEarnings;
    public int dailyExpenses;
    public float volume;
    public int resolutionIndex;
    public int qualityLevel;
}

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public int quantity;
}
