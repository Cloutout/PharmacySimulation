using UnityEngine;

public class DirtManager : MonoBehaviour
{
    [Header("Dirt Generation Settings")]
    [SerializeField] private GameObject dirtPrefab; 
    [SerializeField] private int dirtCountMin = 5;  
    [SerializeField] private int dirtCountMax = 10;
    [SerializeField] private float minX = -10f;    
    [SerializeField] private float maxX = 10f;     
    [SerializeField] private float minZ = -10f;    
    [SerializeField] private float maxZ = 10f;    

    
    public void GenerateDirt()
    {
        int dirtCount = Random.Range(dirtCountMin, dirtCountMax); 

        for (int i = 0; i < dirtCount; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(minX, maxX), 
                0.5f, // Yükseklik
                Random.Range(minZ, maxZ)
            );

            Instantiate(dirtPrefab, randomPosition, Quaternion.identity); 
        }
    }
}