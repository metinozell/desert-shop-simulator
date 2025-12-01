using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BakeScreenManager : MonoBehaviour
{
    public static BakeScreenManager instance;
    [Header("Panel Reference")]
    public GameObject bakeScreenPanel;

    [Header("UI Elements")]
    public TextMeshProUGUI orderNameText;
    public TextMeshProUGUI timerText;

    [Header("Progress Steps")]
    public Image[] stepImages;

    [Header("Work Area")]
    [Tooltip("Single text area that will list all recipe steps.")]
    public TextMeshProUGUI recipeStepsText;

    [Header("Colors")]
    public Color activeStepColor = new Color(0.84f, 0.5f, 0.89f);
    public Color completedStepColor = new Color(1f, 0.84f, 0.84f);
    public Color pendingStepColor = new Color(0.88f, 0.88f, 0.88f);

    [Header("UI Manager")]
    public UIManager uiManager;

    private RecipeData currentRecipe;
    private int currentStepIndex = 0;
    private float stepTimer = 0f;
    private float totalOrderTimer = 0f;
    private bool isTimerRunning = false;
    public bool HasPreparedDessert { get; private set; } = false;
    private List<string> currentMissingIngredients = new List<string>();

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
        bakeScreenPanel.SetActive(false);
        if (uiManager == null)
            uiManager = FindObjectOfType<UIManager>();
    }

  
    void Update()
    {
        if (bakeScreenPanel.activeSelf && currentRecipe != null)
        {
            totalOrderTimer -= GameManager.instance.ScaledDeltaTime;
            totalOrderTimer = Mathf.Max(totalOrderTimer, 0f);

            UpdateTimerUI(totalOrderTimer);

            if (totalOrderTimer <= 0f)
            {
                OnTimerEnd();
            }

            if (isTimerRunning && stepTimer > 0)
            {
                stepTimer -= GameManager.instance.ScaledDeltaTime;
                UpdateRecipeTaskList();
                if (stepTimer <= 0)
                {
                    stepTimer = 0;
                    isTimerRunning = false;
                    Debug.Log($"The step '{currentRecipe.bakeSteps[currentStepIndex].stepName}' is completed automatically.");
                    if (GetCurrentStepName().ToLower().Contains("bake"))
                    {
                        AudioManager.instance.PlayOvenDing();
                    }
                    AdvanceToNextStep();
                }
            }
        }
    }

    public void AdvanceToNextStep()
    {
        if (isTimerRunning)
        {
            Debug.Log("Zamanlayıcı (Fırın) çalışırken etkileşim engellendi.");
            return;
        }

        bool stepSucceeded = true;
        if (currentStepIndex == 0)
        {
            if (CheckIngredients(currentRecipe))
            {
                ConsumeIngredients(currentRecipe);
                stepSucceeded = true;
            }
            else
            {
                Debug.LogWarning("Malzemeler eksik olduğu için adım ilerletilemedi.");
                stepSucceeded = false;
            }
        }
        if (stepSucceeded)
        {
            currentStepIndex++;

            if (currentStepIndex >= currentRecipe.bakeSteps.Count)
            {
                OnBakingComplete();
                return;
            }
            BakeStep nextStepData = GetCurrentStepData();
            if (nextStepData != null)
            {
                if (nextStepData.stepDuration > 0)
                {
                    stepTimer = nextStepData.stepDuration;
                    isTimerRunning = false;
                }
                else
                {
                    stepTimer = 0;
                    isTimerRunning = false;
                }
            }
        }
        UpdateUIState();
    }

    private float orderReward = 0f;

    public void OpenBakeScreen(RecipeData recipe)
    {
        bakeScreenPanel.SetActive(true);
        currentRecipe = recipe;
        currentStepIndex = 0;

        totalOrderTimer = currentRecipe.basePreparationTime;

        orderNameText.text = $"Making: {currentRecipe.dessertName}";
        BakeStep firstStepData = GetCurrentStepData();
        if(firstStepData != null)
        {
            stepTimer = firstStepData.stepDuration;
            isTimerRunning = false;
        }
        else
        {
            stepTimer = 0;
            isTimerRunning = false;
        }
        HasPreparedDessert = false;
        GameManager.instance.IsUiMenuOpen = true;
        UpdateUIState();
    }

    public void CloseBakeScreen()
    {
        Debug.Log("Order cancelled - no reward.");
        bakeScreenPanel.SetActive(false);
    }

    void UpdateTimerUI(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        timerText.text = $"Time: {minutes}:{seconds:00}";

        if (timeInSeconds <= 30f)
            timerText.color = Color.red;
        else
            timerText.color = new Color(0.24f, 0.17f, 0.12f);
    }

    void UpdateUIState()
    {
        UpdateRecipeTaskList();
    }

    public void OnNextStepClicked()
    {
        if (currentStepIndex == 0)
        {
            if (CheckIngredients(currentRecipe))
            {
                ConsumeIngredients(currentRecipe);
            }
            else
            {
                Debug.LogError("Tried to consume ingredients, but they are missing!");
                return; 
            }
        }
        
        currentStepIndex++;
        
        if (currentStepIndex >= currentRecipe.bakeSteps.Count) 
        {
            OnBakingComplete();
        }
        else
        {
            UpdateUIState();
        }
    }


    void OnBakingComplete()
    {
        if (currentRecipe == null) return;

        Debug.Log($"{currentRecipe.dessertName} completed successfully!");
                  HasPreparedDessert = true;
        if (recipeStepsText != null)
        {
            recipeStepsText.text = "<b>Dessert is ready!</b>\nGo to the register to serve the customer.";
        }

        bakeScreenPanel.SetActive(false);
        currentStepIndex = 0;
        stepTimer = 0;
  
    }
    void OnTimerEnd()
    {
        Debug.Log("Time's up! Order failed.");

        if (uiManager != null)
        {
            uiManager.OrderFailed();
        }
        GameManager.instance.RemoveReputation(10);
        AudioManager.instance.PlayOrderDenied();
        bakeScreenPanel.SetActive(false);
              }

    private bool CheckIngredients(RecipeData recipe)
    {
        currentMissingIngredients.Clear();
        foreach (IngredientRequirement req in recipe.requiredIngredients)
        {
            int quantityOwned = InventoryManager.instance.GetItemQuantity(req.ingredient);
            int quantityNeeded = req.quantity;

            if (quantityOwned < quantityNeeded)
            {
  
                int amountMissing = quantityNeeded - quantityOwned;
                
  
                currentMissingIngredients.Add($"<color=red>Missing: {req.ingredient.itemName} (x{amountMissing})</color>");
            }
        }
        return currentMissingIngredients.Count == 0;
    }

    private void ConsumeIngredients(RecipeData recipe)
    {
        foreach (IngredientRequirement req in recipe.requiredIngredients)
        {
            InventoryManager.instance.RemoveItem(req.ingredient, req.quantity);
        }
        Debug.Log("All ingredients consumed for " + recipe.dessertName);
    }
    
    public bool IsBaking()
    {
        return currentRecipe != null;
    }

    public string GetCurrentStepName()
    {
        if (currentRecipe != null && currentStepIndex < currentRecipe.bakeSteps.Count)
        {
            return currentRecipe.bakeSteps[currentStepIndex].stepName;
        }
        return "";
    }

    public bool IsTimerRunning()
    {
        return isTimerRunning;
    }

    public bool StartCurrentStepTimer()
    {
        if (stepTimer > 0 && !isTimerRunning)
        {
            isTimerRunning = true;
            Debug.Log($"The timer has been started: {currentRecipe.bakeSteps[currentStepIndex].stepName}");
            return true;
        }
        Debug.LogWarning("The timer could not be started!");
        return false;
    }

    public BakeStep GetCurrentStepData()
    {
        if (currentRecipe != null && currentStepIndex < currentRecipe.bakeSteps.Count)
        {
            return currentRecipe.bakeSteps[currentStepIndex];
        }
        return null;
    }

    public void ResetBakeState()
    {
        HasPreparedDessert = false;
        currentRecipe = null;
        totalOrderTimer = 0;
        bakeScreenPanel.SetActive(false);
        if (recipeStepsText != null)
        {
            recipeStepsText.text = "";
        }
        GameManager.instance.IsUiMenuOpen = false;
    }

    public RecipeData GetCurrentRecipe()
    {
        return currentRecipe;
    }
    
    void UpdateRecipeTaskList()
    {
        if (recipeStepsText == null) return;
        if (currentRecipe == null)
        {
            recipeStepsText.text = "";
            return;
        }

        string taskList = ""; 
        
        for (int i = 0; i < currentRecipe.bakeSteps.Count; i++)
        {
            if (i == currentStepIndex)
            {
                taskList += $"> <b>{currentRecipe.bakeSteps[i].stepName}</b>";
                if (isTimerRunning)
                {
                    taskList += $" ({Mathf.CeilToInt(stepTimer)}s)";
                }
                if (i == 0 && currentMissingIngredients.Count > 0)
                {
                    taskList += "\n";
                    foreach (string missingItemText in currentMissingIngredients)
                    {
                        taskList += $"  {missingItemText}\n";
                    }
                }
            }
            else if (i < currentStepIndex)
            {
                taskList += $"<s>{currentRecipe.bakeSteps[i].stepName}</s>";
            }
            else
            {
                taskList += $"{currentRecipe.bakeSteps[i].stepName}";
            }
            taskList += "\n"; 
        }
        recipeStepsText.text = taskList;
    }
}