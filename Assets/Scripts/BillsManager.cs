using UnityEngine;
using TMPro;

public class BillsManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI electricityBillText;
    public TextMeshProUGUI rentBillText;
    public GameObject billsPanel;

    [Header("Billing Rates")]
    public float dailyRent = 100f;
    public float electricityRatePerSecond = 0.1f;

    private float _electricityBill;
    private float _rentBill;

    private float _totalElectricityUsage; 
    private int _activeLightSources;

    private bool _needsUIUpdate;
    private bool _isBillsPanelActive;

    private MouseLook _playerMouseLook; 
    private PlayerMovement _playerMovement;

    private void Start()
    {
        billsPanel.SetActive(false);
        UpdateBillsUI();

        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        
        _playerMouseLook = FindObjectOfType<MouseLook>();
        if (_playerMouseLook == null)
        {
            Debug.LogError("MouseLook script'i bulunamadı!");
        }

        
        _playerMovement = FindObjectOfType<PlayerMovement>();
        if (_playerMovement == null)
        {
            Debug.LogError("PlayerMovement script'i bulunamadı!");
        }
    }

    private void Update()
    {
        HandleBillsPanelToggle();
        CalculateElectricityBill();

        if (_needsUIUpdate)
        {
            UpdateBillsUI();
            _needsUIUpdate = false;
        }
    }

    private void HandleBillsPanelToggle()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleBillsPanel(!_isBillsPanelActive);
        }
    }

    private void ToggleBillsPanel(bool state)
    {
        _isBillsPanelActive = state;
        billsPanel.SetActive(state);

        if (state)
        {
            // BillsPanel açıldı, mouse imlecini göster ve kilidi kaldır
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Oyunu durdur
            Time.timeScale = 0f;

            // Oyuncu kontrolünü devre dışı bırak
            if (_playerMouseLook != null)
                _playerMouseLook.isUIActive = true;
            if (_playerMovement != null)
                _playerMovement.isUIActive = true;
        }
        else
        {
            // BillsPanel kapandı, mouse imlecini gizle ve kilitle
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // Oyunu tekrar başlat
            Time.timeScale = 1f;

            // Oyuncu kontrolünü etkinleştir
            if (_playerMouseLook != null)
                _playerMouseLook.isUIActive = false;
            if (_playerMovement != null)
                _playerMovement.isUIActive = false;
        }

        UpdateBillsUI();
    }

    private void CalculateElectricityBill()
    {
        if (_activeLightSources > 0)
        {
            // Oyun durduğunda fatura birikmesin istiyorsanız Time.deltaTime kullanın
            _totalElectricityUsage += Time.deltaTime * _activeLightSources;
            _electricityBill = _totalElectricityUsage * electricityRatePerSecond;
            _needsUIUpdate = true;
        }
    }

    /// <summary>
    /// Gün sonunda kira faturasını ekler.
    /// </summary>
    public void AddDailyRent()
    {
        _rentBill += dailyRent;
        _needsUIUpdate = true;
    }

    /// <summary>
    /// Fatura miktarlarını UI üzerinde günceller.
    /// </summary>
    private void UpdateBillsUI()
    {
        if (electricityBillText != null)
            electricityBillText.text = $"Elektrik: {_electricityBill:F2}$";

        if (rentBillText != null)
            rentBillText.text = $"Kira: {_rentBill:F2}$";
    }

    /// <summary>
    /// Oyuncu faturaları ödediğinde çağrılır.
    /// </summary>
    public void PayAllBills()
    {
        float totalBill = _electricityBill + _rentBill;

        if (SaveManager.Instance.playerBalance >= totalBill)
        {
            SaveManager.Instance.UpdatePlayerBalance(-totalBill);
            ResetBills();
            Debug.Log("Tüm faturalar ödendi.");
        }
        else
        {
            Debug.Log("Faturaları ödemek için yeterli bakiye yok.");
        }
    }

    /// <summary>
    /// Fatura miktarlarını ve sayaçları sıfırlar.
    /// </summary>
    private void ResetBills()
    {
        _electricityBill = 0;
        _rentBill = 0;
        _totalElectricityUsage = 0;
        UpdateBillsUI();
    }

    /// <summary>
    /// Elektrik kullanımını takip etmek için ışık kaynağını kaydeder veya kaldırır.
    /// </summary>
    /// <param name="isOn">Işık açıldıysa true, kapandıysa false.</param>
    public void RegisterLightSource(bool isOn)
    {
        _activeLightSources += isOn ? 1 : -1;
        _activeLightSources = Mathf.Max(0, _activeLightSources); // Negatif değeri engelle
    }
}
