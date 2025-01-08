using UnityEngine;

[CreateAssetMenu(fileName = "New Medicine", menuName = "Medicine")]
public class Medicine : ScriptableObject
{
    [Header("General Information")]
    [Tooltip("Benzersiz bir ID, manuel olarak atanabilir veya otomatik oluşturulabilir.")]
    [SerializeField] private string medicineID;
    public string MedicineID => medicineID; 
    
    [SerializeField] private string medicineName;
    public string MedicineName => medicineName;
    
    [TextArea]
    [SerializeField] private string medicineDescription;
    public string MedicineDescription => medicineDescription;

    [Header("Market Data")]
    [SerializeField, Min(0)] private int deliveryTime;
    public int DeliveryTime => deliveryTime;

    [SerializeField] private Sprite medicineImage;
    public Sprite MedicineImage => medicineImage;

    [SerializeField, Min(0)] private float sellingPrice;
    public float SellingPrice => sellingPrice;

    [SerializeField, Range(0f, 1f)] private float taxRate;
    public float TaxRate => taxRate;

    [SerializeField, Min(0)] private int marketPrice;
    public int MarketPrice => marketPrice;

    [Header("Packaging Information")]
    [SerializeField, Min(0)] private int quantityInBox;
    public int QuantityInBox => quantityInBox;

    [SerializeField, Min(0)] private float price;
    public float Price => price;

    [Header("Stock Data")]
    [SerializeField, Min(0)] private int stockQuantity;
    public int StockQuantity
    {
        get => stockQuantity;
        set => stockQuantity = value;
    }

    private void OnValidate()
    {
       
        if (string.IsNullOrEmpty(medicineID))
        {
            medicineID = System.Guid.NewGuid().ToString();
            Debug.LogWarning($"{medicineName} için benzersiz bir ID oluşturuldu: {medicineID}");
        }

        
        if (price == 0 && marketPrice > 0 && taxRate > 0)
        {
            price = marketPrice * (1 + taxRate);
            Debug.Log($"{medicineName} için fiyat otomatik hesaplandı: {price}");
        }
    }
}
