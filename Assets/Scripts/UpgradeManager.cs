using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    [Header("Upgrade Data Files")]
    [Tooltip("Drag the UPG_FastOven asset here.")]
    public ItemData fastOvenUpgradeData; 

    [Header("Upgrade Situations")]
    public bool HasFastOven { get; private set; } = false;
    public bool UnlockUpgrade(ItemData item)
    {
        if (item == fastOvenUpgradeData)
        {
            if (HasFastOven)
            {
                Debug.LogWarning("You already have a Fast Oven.");
                return false;
            }
            HasFastOven = true;
            Debug.Log("The Upgrade Lock has been Unlocked: Industrial Oven!");
            return true;
        }
        
        Debug.LogWarning($"An unknown upgrade ({item.itemName}) was attempted to be purchased.");
        return false;
    }
    public bool IsUpgradeUnlocked(ItemData item)
    {
        if (item == fastOvenUpgradeData)
            return HasFastOven;
        
        return false;
    }
    public float ApplyBakeSpeed(float baseDuration)
    {
        if (HasFastOven)
        {
            return baseDuration * 0.75f; 
        }
        return baseDuration;
    }
    public bool GetFastOvenStatus()
    {
        return HasFastOven;
    }
    
    public void SetFastOvenStatus(bool status)
    {
        HasFastOven = status;
    }
}