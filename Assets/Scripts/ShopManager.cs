using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using JetBrains.Annotations;
public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    [Header("UI References")]
    public GameObject shopMenuScreen;
    public TextMeshProUGUI playerMoneyText;
    public Transform contentArea;
    public GameObject itemCardPrefab;
    public TMP_Dropdown sortDropdown;

    [Header("Items")]
    public List<ItemData> itemsForSale;
    private ItemCategory currentCategoryView = ItemCategory.Ingredient;

    [Header("Manager")]
    public UIManager uiManager;

  
    
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
    void Start()
    {
        if (uiManager == null)
        {
            Debug.LogError("ShopManager: UIManager reference is not set in the inspector.");
        }

        if (sortDropdown != null)
        {
            PopulateDropdown();
        }
        else
        {
            Debug.LogWarning("Sort Dropdown is not assigned in the inspector.");
        }
        LoadItems();
        UpdateShopUI();
    }

    public void UpdateShopUI()
    {
        if (uiManager != null)
        {
            playerMoneyText.text = $"Balance: ${uiManager.Money:F2}";
        }
    }

    void LoadItems()
    {
       foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        foreach (ItemData item in itemsForSale)
        {
            if (item.category == currentCategoryView)
            {
                Debug.Log("Creating card for: " + item.itemName);
                GameObject card = Instantiate(itemCardPrefab, contentArea);
                card.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = item.itemName;
                card.transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>().text = $"${item.price}";
                card.transform.Find("ItemImage").GetComponent<Image>().sprite = item.image;
                card.transform.Find("BuyButton").GetComponent<Button>().onClick.AddListener(() => BuyItem(item));
            }
        }
    }

    public void BuyItem(ItemData item)
    {
        if (item.category == ItemCategory.Equipment)
        {
            if (UpgradeManager.instance.IsUpgradeUnlocked(item))
            {
                Debug.Log("You already have this upgrade.");
                return;
            }
            if (uiManager.SpendMoney(item.price))
            {
                Debug.Log($"Equipment Purchased: {item.itemName}");
                UpgradeManager.instance.UnlockUpgrade(item);
                UpdateShopUI();        
            }
            else
            {
                Debug.Log("There is not enough money for this upgrade..");
            }
        }
        else
        {
            if (uiManager.SpendMoney(item.price))
            {
                Debug.Log($"Items Purchased: {item.itemName}");
                UpdateShopUI();
                InventoryManager.instance.AddItem(item, 1);
            }
            else
            {
                Debug.Log("There's not enough money for this item.");
            }
        }
    }

    void PopulateDropdown()
    {
        sortDropdown.onValueChanged.AddListener(delegate { SortItems(); });
    }

    public void SortItems()
    {
       if (itemsForSale != null) 
        {
            switch (sortDropdown.value)
            {
                case 0:
                    itemsForSale.Sort((x, y) => x.price.CompareTo(y.price)); 
                    break;
                case 1:
                    itemsForSale.Sort((x, y) => x.itemName.CompareTo(y.itemName));
                    break;
                case 2:
                    itemsForSale.Sort((x, y) => x.category.CompareTo(y.category));
                    break;
            }
        }
        else
        {
            Debug.LogError("Sort Dropdown value is not set properly.");
        }

        LoadItems();
    }
    
    public void OpenShopMenu()
    {
        AudioManager.instance.PlayButtonClick();
        shopMenuScreen.SetActive(true);
        UpdateShopUI();
        GameManager.instance.IsUiMenuOpen = true;
    }

    public void CloseShopMenu()
    {
        AudioManager.instance.PlayButtonClick();
        Debug.Log("[ShopManager] CloseShopMenu() function CALLED SUCCESSFULLY!");
        shopMenuScreen.SetActive(false);
        GameManager.instance.IsUiMenuOpen = false;
        GameManager.instance.ChangeState(GameManager.GameState.Playing);
    }

    public void ShowStockTab()
    {
        AudioManager.instance.PlayButtonClick();
        currentCategoryView = ItemCategory.Ingredient;
        LoadItems();
        Debug.Log("Switched to Stock tab.");
    }

    public void ShowEquipmentTab()
    {
        AudioManager.instance.PlayButtonClick();
        currentCategoryView = ItemCategory.Equipment;
        LoadItems();
        Debug.Log("Switched to Equipment tab.");
    }

    public void ShowDecorTab()
    {
        AudioManager.instance.PlayButtonClick();
        currentCategoryView = ItemCategory.Decoration;
        LoadItems();
        Debug.Log("Switched to Decor tab.");
    }

    void Update()
    {
        
    }
}
