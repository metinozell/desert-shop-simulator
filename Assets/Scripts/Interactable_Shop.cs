using UnityEngine;

public class Interactable_Shop : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "Open Shop Menu";
    public bool Interact(PlayerInteractor interactor)
    {
        Debug.Log("Opening Shop Menu...");
        
        if (ShopManager.instance != null)
        {
            ShopManager.instance.OpenShopMenu();
            return true;
        }
        else
        {
            Debug.LogError("Interactable Shop: ShopManager could not be found on scene!");
            return false;
        }
    }
}
