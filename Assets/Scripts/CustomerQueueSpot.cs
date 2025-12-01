using UnityEngine;

public class CustomerQueueSpot : MonoBehaviour
{
    public CustomerAI CustomerInSpot { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Customer") && CustomerInSpot == null)
        {
            CustomerInSpot = other.GetComponent<CustomerAI>();
            Debug.Log("Customer has arrived at queue spot.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Customer") && other.GetComponent<CustomerAI>() == CustomerInSpot)
        {
            CustomerInSpot = null;
            Debug.Log("Customer has left the queue spot.");
        }
    }
}