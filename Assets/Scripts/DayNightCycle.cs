using TMPro;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Cycle Settings")]
    public float dayDuration = 480f;

    [Header("UI References")]
    public TextMeshProUGUI timeDisplay;
    public TextMeshProUGUI dayDisplay;
    public GameObject endDayButton;
    public GameObject endDaySummaryPanel;
    public TextMeshProUGUI earningsText;
    public TextMeshProUGUI expensesText;
    public TextMeshProUGUI totalMoneyText;
    public GameObject startNewDayButton;

    [Header("Lighting")]
    public Light sunLight;
    public Light moonLight;

    private float _timeElapsed;
    private bool _isDayEnded;

    private PurchaseManager _purchaseManager;
    private BillsManager _billsManager;

    // DirtManager'ı referans alıyoruz
    private DirtManager _dirtManager;

    private void Start()
    {
        InitializeLighting();
        InitializeUI();

        _purchaseManager = FindObjectOfType<PurchaseManager>();
        if (_purchaseManager == null)
        {
            Debug.LogError("PurchaseManager bulunamadı!");
        }

        _billsManager = FindObjectOfType<BillsManager>();
        if (_billsManager == null)
        {
            Debug.LogError("BillsManager bulunamadı!");
        }

        // DirtManager'ı bulup başlatıyoruz
        _dirtManager = FindObjectOfType<DirtManager>();
        if (_dirtManager == null)
        {
            Debug.LogError("DirtManager bulunamadı!");
        }
    }

    private void Update()
    {
        if (_isDayEnded)
        {
            HandleEndOfDayInput();
        }
        else
        {
            AdvanceDayTime();
        }
    }

    private void InitializeLighting()
    {
        if (sunLight != null)
        {
            sunLight.intensity = 1f;
        }
        else
        {
            Debug.LogError("SunLight referansı atanmadı!");
        }

        if (moonLight != null)
        {
            moonLight.enabled = false;
        }
        else
        {
            Debug.LogError("MoonLight referansı atanmadı!");
        }
    }

    private void InitializeUI()
    {
        if (endDayButton != null)
            endDayButton.SetActive(false);

        if (endDaySummaryPanel != null)
            endDaySummaryPanel.SetActive(false);

        if (startNewDayButton != null)
            startNewDayButton.SetActive(false);
    }

    private void AdvanceDayTime()
    {
        _timeElapsed += Time.deltaTime;

        UpdateTimeDisplay();
        RotateSun();
        AdjustAmbientLight();

        if (_timeElapsed >= dayDuration)
        {
            EndDay();
        }
    }

    private void UpdateTimeDisplay()
    {
        float totalMinutes = (_timeElapsed / dayDuration) * 480f;
        float hours = Mathf.Floor(totalMinutes / 60);
        float minutes = Mathf.Floor(totalMinutes % 60);
        timeDisplay.text = $"{(10 + hours):00}:{minutes:00}";
    }

    private void RotateSun()
    {
        if (sunLight != null)
        {
            float timeOfDay = _timeElapsed / dayDuration;
            sunLight.transform.rotation = Quaternion.Euler(new Vector3((timeOfDay * 360f) - 90f, 170f, 0f));
        }
    }

    private void AdjustAmbientLight()
    {
        float timeOfDay = _timeElapsed / dayDuration;
        RenderSettings.ambientLight = timeOfDay < 0.75f ? Color.white : new Color(0.1f, 0.1f, 0.3f);

        if (moonLight != null)
        {
            moonLight.enabled = timeOfDay >= 0.75f;
        }
    }

    private void EndDay()
    {
        _isDayEnded = true;

        if (endDayButton != null)
            endDayButton.SetActive(true);

        Debug.Log("Day Ended - Press Enter to continue.");

        if (_billsManager != null)
        {
            _billsManager.AddDailyRent();
        }

        // Kir üretimini gün sonunda başlat
        _dirtManager.GenerateDirt();
    }

    private void HandleEndOfDayInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!endDaySummaryPanel.activeSelf)
            {
                ShowEndOfDaySummary();
            }
            else
            {
                StartNewDay();
            }
        }
    }

    private void ShowEndOfDaySummary()
    {
        earningsText.text = $"Günlük Kazanç: {SaveManager.Instance.dailyEarnings}";
        expensesText.text = $"Toplam Harcama: {SaveManager.Instance.dailyExpenses}";
        totalMoneyText.text = $"Toplam Para: {SaveManager.Instance.playerBalance}";

        if (endDaySummaryPanel != null)
            endDaySummaryPanel.SetActive(true);

        if (startNewDayButton != null)
            startNewDayButton.SetActive(true);

        SaveManager.Instance.ResetDailyValues();
    }

    private void StartNewDay()
    {
        _timeElapsed = 0f;
        _isDayEnded = false;

        if (endDayButton != null)
            endDayButton.SetActive(false);

        if (endDaySummaryPanel != null)
            endDaySummaryPanel.SetActive(false);

        if (startNewDayButton != null)
            startNewDayButton.SetActive(false);

        SaveManager.Instance.UpdateDay(1);

        if (_purchaseManager != null)
        {
            _purchaseManager.ProgressDeliveryDay();
        }
        else
        {
            Debug.LogError("PurchaseManager bulunamadı!");
        }

        Debug.Log($"Moving to Day {SaveManager.Instance.currentDay}");
    }
}
