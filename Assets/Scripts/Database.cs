using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Database : MonoBehaviour
{
    public static Database instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("Game Data")]
    public List<ItemData> allItems;
    public List<RecipeData> allRecipes;

    private Dictionary<string, ItemData> itemDictionary;

    void InitializeDatabase()
    {
        itemDictionary = new Dictionary<string, ItemData>();
        foreach (ItemData item in allItems)
        {
            if (!itemDictionary.ContainsKey(item.itemName))
            {
                itemDictionary.Add(item.itemName, item);
            }
            else
            {
                Debug.LogWarning($"Database: Duplicate itemName found! '{item.itemName}'");
            }
        }
        Debug.Log("Item Database initialized.");
    }

    public ItemData GetItemByName(string itemName)
    {
        if (itemDictionary.TryGetValue(itemName, out ItemData item))
        {
            return item;
        }
        Debug.LogWarning($"Database: ItemData with name '{itemName}' not found!");
        return null;
    }
}