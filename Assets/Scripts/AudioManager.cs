using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Основные настройки")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip musicClip;

    [Header("Звуки")]
    [SerializeField] private AudioClip fireSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudio()
    {
       
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
        }
        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = 0.2f; 

        if (fireSound != null)
        {
            GameObject fireObj = new GameObject("FireSoundSource");
            fireObj.transform.SetParent(transform);
            AudioSource fireSource = fireObj.AddComponent<AudioSource>();
            fireSource.clip = fireSound;
            fireSource.playOnAwake = false;
            fireSource.volume = 0.7f;
        }
    }
    public void PlayMusic()
    {
        if (musicSource != null && musicSource.clip != null)
        {
            musicSource.time = 0f;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void RestartMusic()
    {
        StopMusic();
        PlayMusic();
    }
    public void PlayFireSound()
    {
        AudioSource fireSource = GetComponent<AudioSource>();
        if (fireSource != null && fireSource.clip != null)
        {
            fireSource.PlayOneShot(fireSound);
        }
    }
}