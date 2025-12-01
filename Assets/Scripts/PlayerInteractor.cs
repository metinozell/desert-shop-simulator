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
    
    // DEĞİŞİKLİK: UI'ın RectTransform'unu saklamak için değişken
    private RectTransform promptRectTransform;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();
        
        if (promptText == null)
            Debug.LogError("PlayerInteractor: PromptText is not assigned!");
        else
        {
            // DEĞİŞİKLİK: Başlangıçta RectTransform referansını al
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

    /// <summary>
    /// Casts a ray from the camera to detect interactable objects.
    /// </summary>
    void HandleRaycast()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        
        IInteractable interactable = null;
        bool hasHit = Physics.Raycast(ray, out currentHit, interactionDistance);

        if (hasHit)
        {
            interactable = currentHit.collider.GetComponent<IInteractable>();
        }

        // --- İpucu Metni Yönetimi (GÜNCELLENDİ) ---

        if (interactable != null)
        {
            // Eğer yeni bir objeye bakmaya başladıysak, metni ayarla
            if (interactable != currentInteractable)
            {
                currentInteractable = interactable;
                promptText.text = $"[E] {currentInteractable.InteractionPrompt}";
            }

            // UI Metnini 3D Objenin Konumuna Taşı
            Vector3 worldPos = currentHit.collider.bounds.center;
            worldPos += Vector3.up * promptVerticalOffset;

            // 3D konumu 2D ekran koordinatına çevir
            // İşte 'screenPos' değişkeni burada:
            Vector2 screenPos = playerCamera.WorldToScreenPoint(worldPos);

            // Objenin kameranın önünde olduğundan emin ol
            bool isOffScreen = screenPos.x <= 0 || screenPos.x >= Screen.width ||
                               screenPos.y <= 0 || screenPos.y >= Screen.height ||
                               playerCamera.WorldToScreenPoint(worldPos).z <= 0;

            if (!isOffScreen)
            {
                promptText.gameObject.SetActive(true);
                
                // DEĞİŞİKLİK: transform.position yerine RectTransform.position kullan.
                // Bu, Canvas Scaler ile %100 uyumlu çalışır.
                promptRectTransform.position = screenPos; 
            }
            else
            {
                promptText.gameObject.SetActive(false);
            }
        }
        // Eğer boşluğa bakıyorsak (veya artık bakmıyorsak)
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