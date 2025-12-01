using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // --- Singleton (Tekil Nesne) Deseni ---
    public static AudioManager instance;

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
        }
    }
    // --- Singleton Sonu ---

    [Header("Audio Sources (Ses Kaynakları)")]
    public AudioSource musicSource; // Arka plan müziği için
    public AudioSource sfxSource;   // Ses efektleri için

    [Header("Audio Clips (Ses Dosyaları)")]
    [Header("Music")]
    public AudioClip mainMusic; // "main sound" dosyanız

    [Header("UI SFX")]
    public AudioClip uiButtonClick; // "button click" dosyanız
    public AudioClip levelUpSound;  // "levelup" dosyanız

    [Header("Game SFX")]
    public AudioClip chaChingSound;     // "cha-ching" dosyanız
    public AudioClip orderDeniedSound;  // "order denied" dosyanız
    public AudioClip doorBellSound;     // "door bell" dosyanız
    public AudioClip ovenDingSound;     // "oven ding" dosyanız

    /// <summary>
    /// Starts playing the main background music on loop.
    /// (Ana arka plan müziğini döngüsel olarak başlatır)
    /// </summary>
    public void StartMusic()
    {
        if (musicSource == null || mainMusic == null)
        {
            Debug.LogWarning("AudioManager: MusicSource or MainMusic clip not assigned.");
            return;
        }
        
        if (musicSource.isPlaying)
            return; // Zaten çalıyorsa tekrar başlatma

        musicSource.clip = mainMusic;
        musicSource.Play();
    }

    // --- Özel SFX Çalma Fonksiyonları ---
    // (Diğer script'ler hangi dosyayı çalacağını bilmek zorunda kalmasın diye)

    public void PlayButtonClick()
    {
        if (uiButtonClick != null)
            sfxSource.PlayOneShot(uiButtonClick);
    }

    public void PlayLevelUp()
    {
        if (levelUpSound != null)
            sfxSource.PlayOneShot(levelUpSound);
    }

    public void PlayChaChing()
    {
        if (chaChingSound != null)
            sfxSource.PlayOneShot(chaChingSound);
    }

    public void PlayOrderDenied()
    {
        if (orderDeniedSound != null)
            sfxSource.PlayOneShot(orderDeniedSound);
    }

    public void PlayDoorBell()
    {
        if (doorBellSound != null)
            sfxSource.PlayOneShot(doorBellSound);
    }

    public void PlayOvenDing()
    {
        if (ovenDingSound != null)
            sfxSource.PlayOneShot(ovenDingSound);
    }
}