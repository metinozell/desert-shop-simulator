using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public List<InventorySlot> slots = new List<InventorySlot>();

    public void AddItem(ItemData item, int quantity)
    {
        if (item.isStackable)
        {
            InventorySlot stackableSlot = slots.FirstOrDefault(s => s.item == item);

            if (stackableSlot != null)
            {
                stackableSlot.AddQuantity(quantity);
                Debug.Log($"Added {quantity} {item.itemName}. New total: {stackableSlot.quantity}");
            }
            else
            {
                InventorySlot newSlot = new InventorySlot(item, quantity);
                slots.Add(newSlot);
                Debug.Log($"Added new item {item.itemName} with quantity {quantity}");
            }
        }
        else
        {
            for (int i = 0; i < quantity; i++)
            {
                slots.Add(new InventorySlot(item, 1));
            }
            Debug.Log($"Added {quantity}x unstackable item: {item.itemName}");
        }
    }

    public bool HasItem(ItemData item, int quantity)
    {
        if (item.isStackable)
        {
            InventorySlot slot = slots.FirstOrDefault(s => s.item == item);
            if (slot != null)
            {
                return slot.quantity >= quantity;
            }
            return false; 
        }
        else
        {
            int count = slots.Count(s => s.item == item);
            return count >= quantity;
        }
    }

    public bool RemoveItem(ItemData item, int quantity)
    {
        if (!HasItem(item, quantity))
        {
            Debug.LogWarning($"InventoryManager: Tried to remove {quantity}x {item.itemName}, but not enough in inventory.");
            return false;
        }

        if (item.isStackable)
        {
            InventorySlot slot = slots.FirstOrDefault(s => s.item == item);
            slot.RemoveQuantity(quantity);

            if (slot.quantity <= 0)
            {
                slots.Remove(slot);
            }
            Debug.Log($"Removed {quantity}x {item.itemName}. Remaining: {slot?.quantity ?? 0}");
        }
        else
        {
            for (int i = 0; i < quantity; i++)
            {
                InventorySlot slotToRemove = slots.First(s => s.item == item);
                slots.Remove(slotToRemove);
            }
            Debug.Log($"Removed {quantity}x unstackable item: {item.itemName}");
        }
        return true;
    }
    
    public int GetItemQuantity(ItemData item)
    {
        if (item == null) return 0;

        if (item.isStackable)
        {
            InventorySlot slot = slots.FirstOrDefault(s => s.item == item);
            if (slot != null)
            {
                return slot.quantity;
            }
        }
        else
        {
            return slots.Count(s => s.item == item);
        }
        return 0;
    }
}