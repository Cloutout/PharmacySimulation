using UnityEngine;

public class Broom : MonoBehaviour
{
    private bool isHeld = false; 
    private Transform playerHand;
    private float cleaningRange = 2f; 
    

    void Start()
    {
        playerHand = GameObject.FindGameObjectWithTag("PlayerHand")?.transform;
        if (playerHand == null) {
            Debug.LogError("PlayerHand tag'ine sahip bir objeyi bulamadım. Lütfen tag'i doğru şekilde atayın.");
        }
    }

    void Update()
    {
        if (isHeld)
        {
            transform.position = Vector3.Lerp(transform.position, playerHand.position, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, playerHand.rotation, Time.deltaTime * 10f);

            if (Input.GetMouseButton(0)) 
            {
                CleanDirt();
            }
        }
    }

    public void GrabBroom()
    {
        isHeld = true; 
    }

    public void DropBroom()
    {
        isHeld = false; 
    }

    private void CleanDirt()
    {
        Collider[] hitDirt = Physics.OverlapSphere(transform.position, cleaningRange);

        foreach (var hit in hitDirt)
        {
            if (hit.CompareTag("Dirt"))
            {
                Destroy(hit.gameObject); 
            }
        }
    }
}