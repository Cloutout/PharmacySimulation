using UnityEngine;
using UnityEngine.AI;

public class Yapayzeka : MonoBehaviour
{
    public Transform bitisTarget; 
    public Transform baslangicNoktasi; 
    public int dukkanmusteri; 

    private NavMeshAgent agent;
    private Transform kasaTarget;

    private bool alisverisYapildimi = false; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        
        if (baslangicNoktasi != null)
        {
            transform.position = baslangicNoktasi.position; 
        }

        
        GameObject kasaObjesi = GameObject.FindWithTag("kasa");
        if (kasaObjesi != null)
        {
            kasaTarget = kasaObjesi.transform;
        }
        else
        {
            Debug.LogWarning("Kasa etiketiyle obje bulunamad�!");
            return; 
        }

        
        agent.SetDestination(kasaTarget.position);
    }

    void Update()
    {
        
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!alisverisYapildimi)
            {
                
                alisverisYapildimi = true;
                Debug.Log("Al��veri� yap�ld�, biti� noktas�na gidiliyor...");
            }

            
            if (alisverisYapildimi)
            {
                agent.SetDestination(bitisTarget.position);
            }
        }

        
        if (alisverisYapildimi && agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.Log("Biti� noktas�na ula��ld�!");
            Destroy(gameObject); 
        }
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("kap�"))
        {
            dukkanmusteri++; 
            Debug.Log("D�kkan m��teri say�s� artt�: " + dukkanmusteri);
        }
    }
}

