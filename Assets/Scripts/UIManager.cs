using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private void Awake()
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

    [Header("Top Bar References")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI moneyText;
    public Slider reputationSlider;

    [Header("Game Data")]
    private float currentTime = 600f;
    [SerializeField] private float money = 100f;
    public float Money => money;
    private bool isDayOver = false;

    [SerializeField] private float dailyRevenue = 0f;
    public float DailyRevenue => dailyRevenue;
    private int ordersCompleted = 0;
    public int OrdersCompleted => ordersCompleted;
    private int totalOrders = 0;
    public int TotalOrders => totalOrders;

    [Header("Time Control")]
    [SerializeField, Range(0.1f, 60f)] private float timeSpeed = 5; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateMoneyUI(money);
        UpdateTimeUI(600f);
    }

    // Update is called once per frame

    public bool SpendMoney(float amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateMoneyUI(money);
            return true;
        }
        else
        {
            Debug.Log("SpendMoney Error: Not Enough Money!");
            return false;
        }
    }
    
    public void UpdateTimeUI(float timeInMinutes)
    {
        int hours = Mathf.FloorToInt(timeInMinutes / 60f);
        int minutes = Mathf.FloorToInt(timeInMinutes % 60f);
        string period = hours >= 12 ? "PM" : "AM";
        hours = hours % 12;
        if (hours == 0) hours = 12;
        timeText.text = $"{hours:00}:{minutes:00}{period}";
    }

    public void UpdateMoneyUI(float newMoneyAmount)
    {
        moneyText.text = $"${newMoneyAmount:F2}";
    }

    public void UpdateDayUI(int day)
    {
        dayText.text = "Day " + day.ToString();
    }

    public void TriggerEndOfDay()
    {
        Debug.Log("UIManager: Day ended. Showing end of day screen.");
        EndOfDayScreenManager endOfDayManager = FindObjectOfType<EndOfDayScreenManager>();
        if (endOfDayManager != null)
        {
            endOfDayManager.ShowEndOfDay();
        }
        else
        {
            Debug.LogError("EndOfDayScreenManager could not be found in the scene!");
        }
    }

    public void StartNewDay()
    {
        dailyRevenue = 0f;
        ordersCompleted = 0;
        totalOrders = 0;
    }
    
    /*void UpdateUI()
    {
        dayText.text = "Day " + currentDay.ToString();
        int hours = Mathf.FloorToInt(currentTime / 60f);
        int minutes = Mathf.FloorToInt(currentTime % 60f);
        string period = hours >= 12 ? "PM" : "AM";
        hours = hours % 12;
        if (hours == 0) hours = 12;
        timeText.text = $"{hours:00}:{minutes:00}{period}";
        moneyText.text = $"${money:F2}";
    }*/

    public void AddMoney(float amount)
    {
        money += amount;
        UpdateMoneyUI(money);
    }

    public void OrderCompleted(float revenue)
    {
        dailyRevenue += revenue;
        ordersCompleted++;
        totalOrders++;
    }

    public void OrderFailed()
    {
        totalOrders++;
    }

    public void SetMoney(float amount)
    {
        money = amount;
        UpdateMoneyUI(money);
    }
    
    public void UpdateReputationUI(float current, float max)
    {
        if (reputationSlider != null)
        {
            reputationSlider.maxValue = max;
            reputationSlider.value = current;
        }
    }
}