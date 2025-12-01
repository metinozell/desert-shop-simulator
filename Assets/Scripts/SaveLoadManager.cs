using UnityEngine;
using System.Collections.Generic;
using System.IO; 
[System.Serializable]
public class SaveData
{
    public int currentDay;
    public float money;
    public List<SerializableInventorySlot> inventorySlots;
    public float reputation;
    public bool hasFastOven;
    public int currentLevel;
    public float currentXP;
    public float xpToNextLevel;
    public int skillPoints;
    public bool isFasterMovementUnlocked;
    public bool isPatientCustomersUnlocked;
    public SaveData()
    {
        inventorySlots = new List<SerializableInventorySlot>();
    }
}

[System.Serializable]
public class SerializableInventorySlot
{
    public string itemName;
    public int quantity;

    public SerializableInventorySlot(string name, int qty)
    {
        itemName = name;
        quantity = qty;
    }
}


public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private string saveFilePath;

    void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }

    public void SaveGame()
    {
        Debug.Log("Saving game to: " + saveFilePath);
        SaveData data = new SaveData();
        data.currentDay = GameManager.instance.GetCurrentDay();
        data.money = UIManager.instance.Money;
        data.reputation = GameManager.instance.GetCurrentReputation();
        data.hasFastOven = UpgradeManager.instance.GetFastOvenStatus();
        data.currentLevel = GameManager.instance.GetCurrentLevel();
        data.currentXP = GameManager.instance.GetCurrentXP();
        data.xpToNextLevel = GameManager.instance.GetXPToNextLevel();
        data.skillPoints = GameManager.instance.GetSkillPoints();
        data.isFasterMovementUnlocked = SkillManager.instance.isFasterMovementUnlocked;
        data.isPatientCustomersUnlocked = SkillManager.instance.isPatientCustomersUnlocked;
        foreach (InventorySlot slot in InventoryManager.instance.slots)
        {
            data.inventorySlots.Add(new SerializableInventorySlot(slot.item.itemName, slot.quantity));
        }

        string jsonData = JsonUtility.ToJson(data, true);
        try
        {
            File.WriteAllText(saveFilePath, jsonData);
            Debug.Log("Game saved successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save game: " + e.Message);
        }
    }

    public bool LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("LoadGame: No save file found. Starting new game.");
            return false;
        }

        Debug.Log("Loading game from: " + saveFilePath);

        try
        {
            string jsonData = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(jsonData);
            GameManager.instance.SetCurrentDay(data.currentDay);
            UIManager.instance.SetMoney(data.money);
            GameManager.instance.SetReputation(data.reputation);
            UpgradeManager.instance.SetFastOvenStatus(data.hasFastOven);
            GameManager.instance.SetLevelData(
                data.currentLevel,
                data.currentXP,
                data.xpToNextLevel,
                data.skillPoints
            );
            SkillManager.instance.SetFasterMovement(data.isFasterMovementUnlocked);
            SkillManager.instance.SetPatientCustomers(data.isPatientCustomersUnlocked);
            InventoryManager.instance.slots.Clear(); 
            foreach (SerializableInventorySlot slot in data.inventorySlots)
            {
                ItemData item = Database.instance.GetItemByName(slot.itemName);
                if (item != null)
                {
                    InventoryManager.instance.AddItem(item, slot.quantity);
                }
                else
                {
                    Debug.LogWarning($"LoadGame: Could not find item '{slot.itemName}' in Database. Skipping.");
                }
            }

            Debug.Log("Game loaded successfully.");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load game: " + e.Message);
            return false;
        }
    }

    public bool DoesSaveFileExist()
    {
        return File.Exists(saveFilePath);
    }
}