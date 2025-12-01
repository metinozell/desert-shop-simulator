using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
public class MiniGameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mixingMiniGameScreen;
    public Slider progressBar;
    public TextMeshProUGUI timerText;

    [Header("Game Settings")]
    private float gameTime = 30f;
    private float timeRemaining;
    private bool gameActive;

    void StartMiniGame()
    {
        timeRemaining = gameTime;
        gameActive = true;
        mixingMiniGameScreen.SetActive(true);
    }
  
    void Start()
    {
        
    }

  
    void Update()
    {
        if (gameActive)
        {
            HandleMiniGame();
        }
    }

    void HandleMiniGame()
    {
        timeRemaining -= Time.deltaTime;
        timerText.text = $"Time: {Mathf.Ceil(timeRemaining)}";

        float mouseInput = Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y");
        progressBar.value += mouseInput * Time.deltaTime;

        if (progressBar.value >= progressBar.maxValue)
        {
            EndMiniGame(true);
        }
        else if (timeRemaining <= 0)
        {
            EndMiniGame(false);
        }
    }
    void EndMiniGame(bool success)
    {
        gameActive = false;
        mixingMiniGameScreen.SetActive(false);
        
        if (success)
        {
            Debug.Log("Mixing successful!");
        }
        else
        {
            Debug.Log("Mixing failed!");
        }
    }
}
