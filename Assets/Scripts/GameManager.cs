using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        EndOfDay
    }

    public GameState currentState;

    [HideInInspector]
    public bool IsUiMenuOpen { get; set; } = false;

    [Header("UI Manager References")]
    public UIManager uiManager;

    [Header("Time and Day Control")]
    [SerializeField, Range(0.1f, 60f)] private float timeSpeed = 5;
    private float currentTime = 600f;
    private bool isTimePaused = false;
    private int currentDay = 1;
    public float ScaledDeltaTime => Time.deltaTime * timeSpeed;

    

    [Header("Player Stats")]
    [SerializeField] private float currentReputation = 50f;
    [SerializeField] private float maxReputation = 100f;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float currentXP = 0;
    [SerializeField] private float xpToNextLevel = 100;
    [SerializeField] private int skillPoints = 0;

    void Start()
    {
        if (uiManager == null)
            uiManager = FindObjectOfType<UIManager>();
        
        if (SaveLoadManager.instance.LoadGame())
        {
            Debug.Log("Successfully loaded saved game data.");
        }
        else
        {
            uiManager.UpdateDayUI(currentDay);
        }
        ChangeState(GameState.Playing);
        AudioManager.instance.StartMusic();
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            UpdateTime();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
            }
            else if (currentState == GameState.Paused && !IsUiMenuOpen) 
            {
                ChangeState(GameState.Playing);
            }
        }
        /*
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (currentState == GameState.Playing)
                ChangeState(GameState.Paused);
            else if (currentState == GameState.Paused)
                ChangeState(GameState.Playing);
        }*/
    }

    void UpdateTime()
    {
        if (isTimePaused) return;

        currentTime += Time.deltaTime * timeSpeed;

        if (uiManager != null)
        {
            uiManager.UpdateTimeUI(currentTime);
        }

        if (currentTime >= 1320f)
        {
            ChangeState(GameState.EndOfDay);
        }
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState && currentState != GameState.Playing) return;

        currentState = newState;

        switch (currentState)
        {
            case GameState.Playing:
                Time.timeScale = 1;
                isTimePaused = false;
                break;
            case GameState.Paused:
                Time.timeScale = 0;
                isTimePaused = true;
                // uiManager.ShowPauseMenu(true);
                break;
            case GameState.EndOfDay:
                isTimePaused = true;
                Debug.Log("GameManager: Day is over.");
                SaveLoadManager.instance.SaveGame();
                if (uiManager != null)
                {
                    uiManager.TriggerEndOfDay();
                }              
                break;
        }
    }

    public void StartNewDay()
    {
        currentDay++;
        currentTime = 600f;
        ChangeState(GameState.Playing);

        if (uiManager != null)
        {
            uiManager.StartNewDay();
            uiManager.UpdateDayUI(currentDay);
        }
    }

    public int GetCurrentDay() => currentDay;

    public void SetCurrentDay(int day)
    {
        currentDay = day;
        uiManager.UpdateDayUI(currentDay);
    }

    public void AddReputation(float amount)
    {
        currentReputation += amount;
        currentReputation = Mathf.Clamp(currentReputation, 0, maxReputation);
        Debug.Log($"Reputation increased by {amount}. New Reputation: {currentReputation}");
        if (uiManager != null)
        {
            uiManager.UpdateReputationUI(currentReputation, maxReputation);
        }
    }

    public void RemoveReputation(float amount)
    {
        currentReputation -= amount;
        currentReputation = Mathf.Clamp(currentReputation, 0, maxReputation);
        Debug.Log($"Reputation decreased by {amount}. New Reputation: {currentReputation}");

        if (uiManager != null)
            uiManager.UpdateReputationUI(currentReputation, maxReputation);
    }

    public float GetCurrentReputation() 
    { 
        return currentReputation; 
    }

    public void SetReputation(float value)
    {
        currentReputation = value;
        currentReputation = Mathf.Clamp(currentReputation, 0, maxReputation);
        if (uiManager != null)
            uiManager.UpdateReputationUI(currentReputation, maxReputation);
    }
    
    public void AddXP(float amount)
    {
        currentXP += amount;
        Debug.Log($"Gained {amount} XP. Current XP: {currentXP} / {xpToNextLevel}");
        
        // TODO: UI'da XP barını güncelle (Adım 15.X)
        // if (uiManager != null) 
        //     uiManager.UpdateXPUI(currentXP, xpToNextLevel);

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        skillPoints++;
        currentXP -= xpToNextLevel; 
        xpToNextLevel *= 1.5f;
        AudioManager.instance.PlayLevelUp();
        Debug.Log($"LEVEL UP! Reached Level {currentLevel}. You have {skillPoints} skill point(s).");
        if (SkillTreeUIManager.instance != null)
            SkillTreeUIManager.instance.UpdateUI();
    }
    public bool SpendSkillPoint(int amount = 1)
    {
        if (skillPoints >= amount)
        {
            skillPoints -= amount;
            Debug.Log($"Skill point spent. Remaining: {skillPoints}");
            if (SkillTreeUIManager.instance != null)
                SkillTreeUIManager.instance.UpdateUI();
            return true;
        }
        Debug.LogWarning("Not enough skill points to spend!");
        return false;
    }
    public int GetCurrentLevel() { return currentLevel; }
    public float GetCurrentXP() { return currentXP; }
    public float GetXPToNextLevel() { return xpToNextLevel; }
    public int GetSkillPoints() { return skillPoints; }

    public void SetLevelData(int level, float xp, float nextLevelXP, int points)
    {
        currentLevel = level;
        currentXP = xp;
        xpToNextLevel = nextLevelXP;
        skillPoints = points;
    }
}
