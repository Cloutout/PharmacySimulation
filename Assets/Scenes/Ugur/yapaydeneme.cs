using UnityEngine;
using UnityEngine.AI;

public class yapaydeneme : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject kasaTarget;

    void Start()
    {
        
        agent = GetComponent<NavMeshAgent>();

        
        kasaTarget = GameObject.FindGameObjectWithTag("kasa");

        
        if (kasaTarget != null)
        {
            agent.SetDestination(kasaTarget.transform.position);
        }
        else
        {
            Debug.LogWarning("Kasa etiketi olan bir obje bulunamadý!");
        }
    }

    void Update()
    {
        
        if (kasaTarget != null && agent.enabled)
        {
            agent.SetDestination(kasaTarget.transform.position);
        }
    }
}

