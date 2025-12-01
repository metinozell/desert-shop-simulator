using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTreeUIManager : MonoBehaviour
{
    public static SkillTreeUIManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    [Header("UI References")]
    [Tooltip("This is the panel itself (the SkillTreeScreen object).")]
    public GameObject skillTreePanel;
    
    [Header("Indicators")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI skillPointText;

    [Header("Skill Buttons")]
    public Button movementButton;
    public Button patienceButton;

    void Start()
    {
        skillTreePanel.SetActive(false);
    }
    public void OpenMenu()
    {
        AudioManager.instance.PlayButtonClick();
        skillTreePanel.SetActive(true);
        GameManager.instance.IsUiMenuOpen = true;
        GameManager.instance.ChangeState(GameManager.GameState.Paused);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        UpdateUI();
    }
    public void CloseMenu()
    {
        AudioManager.instance.PlayButtonClick();
        skillTreePanel.SetActive(false);
        GameManager.instance.IsUiMenuOpen = false;
        GameManager.instance.ChangeState(GameManager.GameState.Playing); 
        
    }
    public void UpdateUI()
    {
        if (!skillTreePanel.activeInHierarchy)
            return;
        levelText.text = $"Level: {GameManager.instance.GetCurrentLevel()}";
        skillPointText.text = $"Points Available: {GameManager.instance.GetSkillPoints()}";
        bool hasPoints = GameManager.instance.GetSkillPoints() > 0;
        if (SkillManager.instance.isFasterMovementUnlocked)
        {
            movementButton.interactable = false; 
        }
        else
        {
            movementButton.interactable = hasPoints; 
        }
        if (SkillManager.instance.isPatientCustomersUnlocked)
        {
            patienceButton.interactable = false; 
        }
        else
        {
            patienceButton.interactable = hasPoints; 
        }
    }
    public void OnClick_UnlockFasterMovement()
    {
        AudioManager.instance.PlayButtonClick();
        SkillManager.instance.UnlockSkill("FASTER_MOVEMENT");
        UpdateUI();
    }

    public void OnClick_UnlockPatientCustomers()
    {
        AudioManager.instance.PlayButtonClick();
        SkillManager.instance.UnlockSkill("PATIENT_CUSTOMERS");
        UpdateUI();
    }
}