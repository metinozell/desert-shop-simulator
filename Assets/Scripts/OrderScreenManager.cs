using UnityEngine;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;
using System.Xml.Serialization;
using System.Collections.Generic;

public class OrderScreenManager : MonoBehaviour
{
    [Header("Order Screen References")]
    public GameObject orderScreenPanel;

    [Header("Customer Info")]
    public TextMeshProUGUI customerNameText;
    public Slider patienceBar;
    public TextMeshProUGUI patienceText;

    [Header("Order Details")]
    public Image dessertImage;
    public TextMeshProUGUI dessertNameText;
    public TextMeshProUGUI ingredientsText;
    public GameObject[] difficultyStars;

    [Header("Reward")]
    public TextMeshProUGUI rewardText;

    [Header("Managers")]
    public UIManager uiManager;
    public BakeScreenManager bakeScreenManager;
    private float currentReward = 0f;

    [Header("Order Data")]
    public List<RecipeData> possibleRecipes;
    private RecipeData currentOrderRecipe;
    private float patience = 100f;

    [Header("Game Settings")]
    [Tooltip("How fast patience decreases. Scales with game time.")]
    [SerializeField] private float patienceLossRate = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        orderScreenPanel.SetActive(false);
        if (BakeScreenManager.instance == null) Debug.LogError("OrderScreenManager: BakeScreenManager bulunamadı!");
    }

    // Update is called once per frame
    void Update()
    {
        if (orderScreenPanel.activeSelf)
        {
            float currentPatienceLoss = patienceLossRate;
            if (SkillManager.instance != null && SkillManager.instance.isPatientCustomersUnlocked)
            {
                currentPatienceLoss *= 0.75f;
            }

            patience -= GameManager.instance.ScaledDeltaTime * currentPatienceLoss;
            patience = Mathf.Clamp(patience, 0f, 100f);

            UpdatePatienceUI();

            if(patience <= 0f)
            {
                OnRejectOrder();
            }
        }
    }

    public void OpenOrderScreen()
    {
        orderScreenPanel.SetActive(true);
        GenerateRandomOrder();
        GameManager.instance.IsUiMenuOpen = true; 
        GameManager.instance.ChangeState(GameManager.GameState.Paused);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseOrderScreen()
    {
        orderScreenPanel.SetActive(false);
        patience = 100f;
    }

    void GenerateRandomOrder()
    {
        string[] names = { "Sarah M.", "John D.", "Emily R.", "Michael S.", "Jessica L.", "David K.", "Ashley T.", "Daniel P.", "Megan W.", "Andrew B." };
        customerNameText.text = names[Random.Range(0, names.Length)];

        if (possibleRecipes == null || possibleRecipes.Count == 0)
        {
            Debug.LogError("OrderScreenManager: 'possibleRecipes' listesi boş!");
            return;
        }
        
        currentOrderRecipe = possibleRecipes[Random.Range(0, possibleRecipes.Count)];
        dessertNameText.text = currentOrderRecipe.dessertName;
        //dessertImage.sprite = currentOrderRecipe.dessertImage; RESIM ATA
        rewardText.text = $"Reward: ${currentOrderRecipe.baseReward}";
        ShowDifficultyStars(currentOrderRecipe.difficulty);
    
        string ingredientsListText = "Ingredients:\n";
        foreach (IngredientRequirement req in currentOrderRecipe.requiredIngredients)
        {
            ingredientsListText += $"• {req.ingredient.itemName} (x{req.quantity})\n";
        }
        ingredientsText.text = ingredientsListText;

        patience = 100f;
    }

    void ShowDifficultyStars(int difficulty)
    {
        for (int i = 0; i < difficultyStars.Length; i++)
    {
        if (i < difficulty)
        {
            difficultyStars[i].SetActive(true);
        }
        else
        {
            difficultyStars[i].SetActive(false);
        }
    }
    }   

    void UpdatePatienceUI()
    {
        patienceBar.value = patience;
        patienceText.text = $"Patience: {Mathf.RoundToInt(patience)}%";

        Color barColor = Color.Lerp(Color.red, Color.green, patience / 100f);
        patienceBar.fillRect.GetComponent<Image>().color = barColor;
    }

    public void OnAcceptOrder()
    {

        if (currentOrderRecipe == null) return;
        
        CloseOrderScreen();

        if (BakeScreenManager.instance != null)
        {
            BakeScreenManager.instance.OpenBakeScreen(currentOrderRecipe);
        }
        else
        {
            Debug.LogError("OnAcceptOrder: BakeScreenManager.instance not found!");
        }
        GameManager.instance.IsUiMenuOpen = false;
        GameManager.instance.ChangeState(GameManager.GameState.Playing);
    }

    public void OnRejectOrder()
    {
        Debug.Log("Order Rejected!");
        if (uiManager != null)
            uiManager.OrderFailed();
        GameManager.instance.RemoveReputation(3);
        AudioManager.instance.PlayOrderDenied();
        GameManager.instance.IsUiMenuOpen = false;
        GameManager.instance.ChangeState(GameManager.GameState.Playing);
        CloseOrderScreen();
    }
}
