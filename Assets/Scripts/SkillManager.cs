using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    [Header("Beceri Durumları")]
    public bool isFasterMovementUnlocked { get; private set; } = false;
    public bool isPatientCustomersUnlocked { get; private set; } = false;
    // (Gelecekte eklenecekler: isHigherTipsUnlocked, isFasterMixingUnlocked vb.)

    public void UnlockSkill(string skillID)
    {
        if (GameManager.instance.SpendSkillPoint(1))
        {
            switch (skillID)
            {
                case "FASTER_MOVEMENT":
                    isFasterMovementUnlocked = true;
                    Debug.Log("Skill Unlocked: Faster Movement!");
                    break;
                case "PATIENT_CUSTOMERS":
                    isPatientCustomersUnlocked = true;
                    Debug.Log("Skill Unlocked: Patient Customers!");
                    break;
                default:
                    Debug.LogWarning($"Bilinmeyen skillID: {skillID}");
                    break;
            }
        }
    }
    public void SetFasterMovement(bool status) { isFasterMovementUnlocked = status; }
    public void SetPatientCustomers(bool status) { isPatientCustomersUnlocked = status; }
}