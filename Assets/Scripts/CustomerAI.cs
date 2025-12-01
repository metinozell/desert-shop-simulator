using UnityEngine;
using UnityEngine.AI; 

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerAI : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform queuePoint;
    private bool isLeaving = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!agent.isOnNavMesh)
        {
            Debug.LogError($"CustomerAI Start Error: Agent for '{gameObject.name}' is not on a NavMesh!", this.gameObject);
        }
        else
        {
            Debug.Log($"CustomerAI Start: Agent is on NavMesh. ({gameObject.name})", this.gameObject);
        }
        AudioManager.instance.PlayDoorBell();
        if (queuePoint == null)
        {
            Debug.LogError("CustomerAI: QueuePoint is not assigned!", this.gameObject);
            return;
        }
        GoToQueue();
    }

    void Update()
    {
        if (isLeaving && agent != null)
        {
            
            if (agent.hasPath && !agent.pathPending && agent.remainingDistance < 0.5f)
            {
                Debug.Log("Customer reached DespawnPoint. Destroying...", this.gameObject);
                Destroy(gameObject);
            }
        }
    }

    public void GoToQueue()
    {
        if (queuePoint != null && agent != null)
        {
            isLeaving = false; 
            GetComponent<CapsuleCollider>().isTrigger = false; 

            Debug.Log("Customer is going to queue...", this.gameObject);
            agent.isStopped = false; 
            agent.SetDestination(queuePoint.position);
        }
    }
    public void LeaveShop(Transform despawnTarget)
    {
        Debug.Log($"--- CustomerAI.LeaveShop() HE'S BEEN CALLED! Hedef: {despawnTarget.name} ---", this.gameObject);

        if (agent == null)
        {
             Debug.LogError("LeaveShop Error: NavMeshAgent is null!", this.gameObject);
             return;
        }
        if (despawnTarget == null)
        {
             Debug.LogError("LeaveShop Error: despawnTarget is null!", this.gameObject);
             return;
        }

        isLeaving = true;
        GetComponent<CapsuleCollider>().isTrigger = true;
        
        bool success = agent.SetDestination(despawnTarget.position);
        agent.isStopped = false; 

        Debug.Log($"SetDestination success: {success}. Agent Is On NavMesh: {agent.isOnNavMesh}", this.gameObject);
        
        if (!success)
        {
             Debug.LogError("SetDestination failed! Check if DespawnPoint is on the NavMesh.", this.gameObject);
        }
    }
}