using UnityEngine;

public class Interactable_Register : MonoBehaviour, IInteractable
{
    [Header("Connections")]
    public CustomerQueueSpot queueSpot; 
    public Transform despawnPoint;
    private BakeScreenManager bakeManager;
    private UIManager uiManager;

    void Start()
    {
        bakeManager = BakeScreenManager.instance;
        uiManager = FindObjectOfType<UIManager>();
    }
    public string InteractionPrompt
    {
        get
        {
            if (queueSpot.CustomerInSpot == null)
            {
                return "Waiting for Customer...";
            }
            if (bakeManager.HasPreparedDessert)
            {
                return "Serve Dessert";
            }
            return "Take Order";
        }
    }

    public bool Interact(PlayerInteractor interactor)
    {
        if (queueSpot.CustomerInSpot == null)
        {
            Debug.Log("Interact Denied: No customer at register.");
            return false;
        }
        if (bakeManager.HasPreparedDessert)
        {
            ServeDessert();
            return true;
        }
        else
        {
            TakeOrder();
            return true;
        }
    }

    private void TakeOrder()
    {
        Debug.Log("Opening Order Screen for customer...");
        OrderScreenManager orderManager = FindObjectOfType<OrderScreenManager>();
        if (orderManager != null)
        {
            orderManager.OpenOrderScreen();
        }
        else
        {
            Debug.LogError("Interactable_Register: OrderScreenManager not found!");
        }
    }

    private void ServeDessert()
    {
        Debug.Log("--- SERVE DESSERT BAŞLADI ---");
        RecipeData recipe = BakeScreenManager.instance.GetCurrentRecipe();
        if (recipe == null) {    }
        else if (uiManager == null) {    }
        else
        {
            uiManager.AddMoney(recipe.baseReward);
            uiManager.OrderCompleted(recipe.baseReward);
            Debug.Log($"Earned: ${recipe.baseReward}");
            AudioManager.instance.PlayChaChing();
            GameManager.instance.AddReputation(5);
            GameManager.instance.AddXP(25);
        }
        
        if (despawnPoint == null)
        {
            Debug.LogError("Interactable_Register: DespawnPoint is not assigned in Inspector!");
        }
        else if (queueSpot.CustomerInSpot == null)
        {
             Debug.LogError("ServeDessert Error: CustomerInSpot was null when trying to send them away!");
        }
        else
        {
            CustomerAI customerScript = queueSpot.CustomerInSpot;
            Debug.Log($"[A] Gönderilecek Müşteri Objesi: {customerScript.gameObject.name}");
            Debug.Log($"[B] CustomerAI script'inin 'enabled' durumu: {customerScript.enabled}");
            if (!customerScript.enabled)
            {
                Debug.LogWarning("[C] Script devre dışıydı! Şimdi zorla AÇILIYOR.");
                customerScript.enabled = true;
            }
            Debug.Log("[D] LeaveShop() fonksiyonu ŞİMDİ ÇAĞRILIYOR...");
            customerScript.LeaveShop(despawnPoint);
            Debug.Log("[E] LeaveShop() fonksiyonu ÇAĞRILDI (Eğer arada hata vermediyse).");
        }
        bakeManager.ResetBakeState();
        Debug.Log("--- SERVE DESSERT BİTTİ ---");
    }
}
