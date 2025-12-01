using UnityEngine;

public class AudioManager : MonoBehaviour
{
  
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
  

    [Header("Audio Sources (Ses Kaynakları)")]
    public AudioSource musicSource;       public AudioSource sfxSource;     
    [Header("Audio Clips (Ses Dosyaları)")]
    [Header("Music")]
    public AudioClip mainMusic;   
    [Header("UI SFX")]
    public AudioClip uiButtonClick;       public AudioClip levelUpSound;    
    [Header("Game SFX")]
    public AudioClip chaChingSound;           public AudioClip orderDeniedSound;        public AudioClip doorBellSound;           public AudioClip ovenDingSound;       
  
  
  
  
    public void StartMusic()
    {
        if (musicSource == null || mainMusic == null)
        {
            Debug.LogWarning("AudioManager: MusicSource or MainMusic clip not assigned.");
            return;
        }
        
        if (musicSource.isPlaying)
            return;   
        musicSource.clip = mainMusic;
        musicSource.Play();
    }

  
  

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