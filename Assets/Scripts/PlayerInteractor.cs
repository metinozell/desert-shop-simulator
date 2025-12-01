using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera; 
    public TextMeshProUGUI promptText; 

    [Header("Settings")]
    public float interactionDistance = 2f; 
    [Tooltip("The vertical offset from the object's center to place the prompt.")]
    public float promptVerticalOffset = 0.5f;

    private IInteractable currentInteractable;
    private RaycastHit currentHit;
    
  
    private RectTransform promptRectTransform;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();
        
        if (promptText == null)
            Debug.LogError("PlayerInteractor: PromptText is not assigned!");
        else
        {
  
            promptRectTransform = promptText.GetComponent<RectTransform>(); 
            promptText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Playing)
        {
            if(currentInteractable != null)
            {
                currentInteractable = null;
                promptText.gameObject.SetActive(false);
            }
            return;
        }
        
        HandleRaycast();
    }

  
  
  
    void HandleRaycast()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        
        IInteractable interactable = null;
        bool hasHit = Physics.Raycast(ray, out currentHit, interactionDistance);

        if (hasHit)
        {
            interactable = currentHit.collider.GetComponent<IInteractable>();
        }

  

        if (interactable != null)
        {
  
            if (interactable != currentInteractable)
            {
                currentInteractable = interactable;
                promptText.text = $"[E] {currentInteractable.InteractionPrompt}";
            }

  
            Vector3 worldPos = currentHit.collider.bounds.center;
            worldPos += Vector3.up * promptVerticalOffset;

  
  
            Vector2 screenPos = playerCamera.WorldToScreenPoint(worldPos);

  
            bool isOffScreen = screenPos.x <= 0 || screenPos.x >= Screen.width ||
                               screenPos.y <= 0 || screenPos.y >= Screen.height ||
                               playerCamera.WorldToScreenPoint(worldPos).z <= 0;

            if (!isOffScreen)
            {
                promptText.gameObject.SetActive(true);
                
  
  
                promptRectTransform.position = screenPos; 
            }
            else
            {
                promptText.gameObject.SetActive(false);
            }
        }
  
        else if (currentInteractable != null)
        {
            currentInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }
    
    public void OnInteract(InputValue value)
    {
        if (currentInteractable != null && 
            GameManager.instance.currentState == GameManager.GameState.Playing &&
            value.isPressed)
        {
            currentInteractable.Interact(this); 
        }
    }
}