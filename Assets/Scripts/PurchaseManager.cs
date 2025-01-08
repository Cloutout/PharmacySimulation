using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PurchaseManager : MonoBehaviour
{
    [Header("Medicine Data")]
    public List<Medicine> availableMedicines;

    [Header("UI Elements")]
    public Transform purchasePanel;
    public Transform medicinePurchaseCanvas;
    public GameObject medicineItemPrefab;
    public Transform cartPanel;
    public GameObject cartItemPrefab;
    public TextMeshProUGUI totalCostText;

    [Header("Other Managers")]
    
    public BillsManager billsManager;

    [Header("Delivery Settings")]
    public GameObject cubePrefab;
    public Transform spawnPoint;

    private readonly Dictionary<Medicine, int> _cart = new Dictionary<Medicine, int>();
    private readonly List<PendingDelivery> _pendingDeliveries = new List<PendingDelivery>();
    private float _totalCost;
    private bool _isPurchasePanelActive;

    [System.Serializable]
    public class PendingDelivery
    {
        public Medicine medicine;
        public int remainingDays;
        public int quantity; 
    }

    private void Start()
    {
        ValidateSetup();
        UpdateMedicineUI();
        UpdateTotalCostUI();
        TogglePurchasePanel(false);
    }

    private void Update()
    {
        HandlePurchasePanelToggle();
    }

    private void ValidateSetup()
    {
        if (totalCostText == null)
        {
            Debug.LogError("Total cost text is not assigned!");
        }

        foreach (var medicine in availableMedicines)
        {
            if (medicine == null)
            {
                Debug.LogError("Found a null medicine in the availableMedicines list!");
                continue;
            }

            if (medicine.MedicineImage == null)
                Debug.LogError($"Medicine image is missing for {medicine.MedicineName}");
            if (string.IsNullOrEmpty(medicine.MedicineName))
                Debug.LogError($"Medicine name is missing for an entry!");
            if (medicine.Price <= 0)
                Debug.LogError($"Medicine price is invalid for {medicine.MedicineName}");
        }
    }

    private void HandlePurchasePanelToggle()
    {
        if (Input.GetKeyDown(KeyCode.L) && !_isPurchasePanelActive)
        {
            OpenPurchasePanel();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && _isPurchasePanelActive)
        {
            ClosePurchasePanel();
        }
    }

    private void UpdateMedicineUI()
    {
        foreach (Transform child in purchasePanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var medicine in availableMedicines)
        {
            var item = Instantiate(medicineItemPrefab, purchasePanel);
            var image = item.transform.Find("MedicineImage").GetComponent<Image>();
            var nameText = item.transform.Find("MedicineName").GetComponent<TextMeshProUGUI>();
            var stockText = item.transform.Find("MedicineQuantity").GetComponent<TextMeshProUGUI>();
            var priceText = item.transform.Find("MedicinePrice").GetComponent<TextMeshProUGUI>();
            var descText = item.transform.Find("MedicineDescription").GetComponent<TextMeshProUGUI>();
            var deliveryText = item.transform.Find("DeliveryTime").GetComponent<TextMeshProUGUI>();
            var addToCartButton = item.transform.Find("AddToCartButton").GetComponent<Button>();

            image.sprite = medicine.MedicineImage;
            nameText.text = medicine.MedicineName;
            stockText.text = $"Stock: {medicine.StockQuantity}";
            priceText.text = $"Price: {medicine.Price}";
            descText.text = medicine.MedicineDescription;
            deliveryText.text = $"Delivery Time: {medicine.DeliveryTime} days";

            addToCartButton.onClick.AddListener(() => AddToCart(medicine, 1));
        }
    }

    private void AddToCart(Medicine medicine, int quantity)
    {
        if (!_cart.ContainsKey(medicine))
        {
            _cart[medicine] = 0;
        }

        _cart[medicine] += quantity;

        _totalCost += medicine.Price * quantity;
        UpdateCartItemUI(medicine);
        UpdateTotalCostUI();
    }

    private void UpdateCartItemUI(Medicine medicine)
    {
        Transform existingCartItem = cartPanel.Find(medicine.MedicineName);

        if (existingCartItem != null)
        {
            existingCartItem.Find("CartItemQuantity").GetComponent<TextMeshProUGUI>().text = $"x{_cart[medicine]}";
            existingCartItem.Find("CartItemPrice").GetComponent<TextMeshProUGUI>().text = $"Total: {_cart[medicine] * medicine.Price}";
        }
        else
        {
            var cartItem = Instantiate(cartItemPrefab, cartPanel);
            cartItem.name = medicine.MedicineName;
            cartItem.transform.Find("CartItemImage").GetComponent<Image>().sprite = medicine.MedicineImage;
            cartItem.transform.Find("CartItemName").GetComponent<TextMeshProUGUI>().text = medicine.MedicineName;
            cartItem.transform.Find("CartItemQuantity").GetComponent<TextMeshProUGUI>().text = $"x{_cart[medicine]}";
            cartItem.transform.Find("CartItemPrice").GetComponent<TextMeshProUGUI>().text = $"Total: {_cart[medicine] * medicine.Price}";
        }
    }

    public void ConfirmPurchase()
    {
        
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager Instance is not set!");
            return;
        }

        if (SaveManager.Instance.playerBalance < _totalCost)
        {
            Debug.Log("Insufficient funds!");
            return;
        }

        
        SaveManager.Instance.UpdatePlayerBalance(-Mathf.RoundToInt(_totalCost));

        
        SaveManager.Instance.AddToExpenses(Mathf.RoundToInt(_totalCost));

        foreach (var item in _cart)
        {
            Medicine medicine = item.Key;
            int quantity = item.Value;

            if (medicine.StockQuantity >= quantity)
            {
                medicine.StockQuantity -= quantity;

                _pendingDeliveries.Add(new PendingDelivery
                {
                    medicine = medicine,
                    remainingDays = medicine.DeliveryTime,
                    quantity = quantity 
                });

                Debug.Log($"{quantity} adet {medicine.MedicineName} teslimat listesine eklendi.");
            }
            else
            {
                Debug.LogWarning($"Not enough stock for {medicine.MedicineName}.");
            }
        }

        ClearCart();
        UpdateMedicineUI();
        Debug.Log("Purchase successful.");
    }

    public void ProgressDeliveryDay()
    {
        Debug.Log("ProgressDeliveryDay metodu çağrıldı.");

        for (int i = _pendingDeliveries.Count - 1; i >= 0; i--)
        {
            _pendingDeliveries[i].remainingDays--;

            if (_pendingDeliveries[i].remainingDays <= 0)
            {
                SpawnMedicine(_pendingDeliveries[i].medicine, _pendingDeliveries[i].quantity);
                _pendingDeliveries.RemoveAt(i);
            }
        }

        Debug.Log($"Kalan teslimat sayısı: {_pendingDeliveries.Count}");
    }

    private void SpawnMedicine(Medicine medicine, int quantity)
    {
        if (cubePrefab != null && spawnPoint != null)
        {
            for (int i = 0; i < quantity; i++)
            {
                Instantiate(cubePrefab, spawnPoint.position, Quaternion.identity);
            }
            Debug.Log($"{quantity} adet {medicine.MedicineName} teslim edildi!");

            SaveManager.Instance.UpdateInventory(medicine.MedicineName, quantity);
        }
        else
        {
            Debug.LogError("cubePrefab veya spawnPoint atanmadı!");
        }
    }

    private void ClearCart()
    {
        _cart.Clear();
        _totalCost = 0;

        foreach (Transform child in cartPanel)
        {
            Destroy(child.gameObject);
        }

        UpdateTotalCostUI();
        ResetInputSystem();
    }

    private void UpdateTotalCostUI()
    {
        totalCostText.text = $"Total: {_totalCost}";
    }

    private void OpenPurchasePanel()
    {
        TogglePurchasePanel(true);
    }

    private void ClosePurchasePanel()
    {
        TogglePurchasePanel(false);
    }

    private void TogglePurchasePanel(bool state)
    {
        _isPurchasePanelActive = state;
        purchasePanel.gameObject.SetActive(state);
        medicinePurchaseCanvas.gameObject.SetActive(state);
        cartPanel.gameObject.SetActive(state);
        Time.timeScale = state ? 0 : 1;

        Cursor.visible = state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;

        ResetInputSystem();
    }

    private void ResetInputSystem()
    {
        var inputModule = EventSystem.current.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        if (inputModule != null)
        {
            inputModule.enabled = false;
            inputModule.enabled = true;
        }
    }
}
