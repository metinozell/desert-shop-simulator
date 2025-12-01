using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EndOfDayScreenManager : MonoBehaviour
{

    [Header("Panel Reference")]
    public GameObject endOfDayPanel;

    [Header("Stats UI")]
    public TextMeshProUGUI revenueValueText;
    public TextMeshProUGUI ordersValueText;
    public TextMeshProUGUI satisfactionValueText;

    [Header("Button")]
    public Button continueButton;

    [Header("Managers")]
    public UIManager uiManager;

    private float dailyRevenue = 0f;
    private int ordersCompleted = 0;
    private int totalOrders = 0;
    private float totalSatisfaction = 0f;
  
    void Start()
    {
        endOfDayPanel.SetActive(false);
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }
    }

    public void ShowEndOfDay()
    {
        CalculateStats();
        UpdateUI();
        endOfDayPanel.SetActive(true);
    }

    void CalculateStats()
    {
        if (uiManager != null)
        {
            dailyRevenue = uiManager.DailyRevenue;
            ordersCompleted = uiManager.OrdersCompleted;
            totalOrders = uiManager.TotalOrders;

            if (totalOrders > 0)
            {
                totalSatisfaction = ((float)ordersCompleted / totalOrders) * 100f;
            }
        }
        else
        {
            Debug.LogError("UIManager reference is missing in EndOfDayScreenManager!");
        }

  
  
    }

    void UpdateUI()
    {
        if (revenueValueText != null)
        {
            revenueValueText.text = $"${dailyRevenue:F2}";
        }

        if (ordersValueText != null)
        {
            ordersValueText.text = $"{ordersCompleted} / {totalOrders}";
        }

        if (satisfactionValueText != null)
        {
            satisfactionValueText.text = $"{Mathf.RoundToInt(totalSatisfaction)}%";

            if (totalSatisfaction >= 80)
            {
                satisfactionValueText.color = new Color(0.18f, 0.49f, 0.2f);
            }
            else if (totalSatisfaction >= 60)
            {
                satisfactionValueText.color = new Color(0.95f, 0.61f, 0.07f); 
            }
            else
            {
                satisfactionValueText.color = Color.red;
            }
        }
    }

    void OnContinueButtonClicked()
    {
        Debug.Log("Continue to next day!");
        GameManager.instance.StartNewDay();
        endOfDayPanel.SetActive(false);
    }
    
    void AddCompletedOrder(float revenue,float satisfaction)
    {
        dailyRevenue += revenue;
        ordersCompleted++;
        totalOrders++;
        totalSatisfaction = ((totalSatisfaction * (ordersCompleted - 1)) + satisfaction) / ordersCompleted;
    }
  
    void Update()
    {
        
    }
}
