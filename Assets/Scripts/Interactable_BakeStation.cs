using UnityEngine;

public class Interactable_BakeStation : MonoBehaviour, IInteractable
{
    [Header("Station Settings")]
    [Tooltip("This is the name of the step that the station has completed. It must match exactly with the 'Step Name' in the 'Recipe'!")]
    public string associatedStepName;
    public string InteractionPrompt
    {
        get
        {
            if (!BakeScreenManager.instance.IsBaking())
                return "";

            if (BakeScreenManager.instance.GetCurrentStepName() == associatedStepName)
            {
                return $"Use {associatedStepName} Station";
            }
            return ""; 
        }
    }

    public bool Interact(PlayerInteractor interactor)
    {
        if (!BakeScreenManager.instance.IsBaking())
            return false;

        if (BakeScreenManager.instance.GetCurrentStepName() == associatedStepName)
        {
            if (BakeScreenManager.instance.IsTimerRunning())
            {
                Debug.Log("Interaction is blocked while the timer (oven) is running.");
                return false;
            }

            BakeStep currentStepData = BakeScreenManager.instance.GetCurrentStepData();
            if (currentStepData != null && currentStepData.stepDuration > 0)
            {
                BakeScreenManager.instance.StartCurrentStepTimer();
                return true;
            }
            else
            {
                Debug.Log($"Correctly (without timer) interacted with the station: {associatedStepName}");
                BakeScreenManager.instance.AdvanceToNextStep();
                return true;
            }
        }
        else
        {
            Debug.LogWarning($"The wrong station! Expected: {BakeScreenManager.instance.GetCurrentStepName()}, Clicked: {associatedStepName}");
            return false;
        }
    }

}