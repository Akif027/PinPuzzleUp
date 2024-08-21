using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; } // Singleton instance

    public GameData gameData; // Reference to the GameData ScriptableObject
    public AudioSource musicSource; // AudioSource for background music
    public AudioSource sfxSource; // AudioSource for sound effects

    private void Awake()
    {
        // Check if there is another instance of SoundManager already
        if (Instance == null)
        {
            Instance = this; // Set this instance as the singleton instance
            DontDestroyOnLoad(gameObject); // Make sure this object is not destroyed on scene load

            // Configure musicSource
            musicSource.loop = true; // Loop background music

            // Optionally configure other settings like volume here
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    void Start()
    {
        PlayBackgroundMusic();

    }
    public void PlayBackgroundMusic()
    {
        PlayMusic(gameData.BackGroundA);
    }

    public void PlayOnFillPool()
    {
        PlaySFX(gameData.OnfillpoolA);
    }

    public void PlayEndTheGame()
    {
        PlaySFX(gameData.EndTheGameA);
    }

    public void PlayOnCombo()
    {
        PlaySFX(gameData.OnComboA);
    }

    public void PlayOnButtonPress()
    {
        PlaySFX(gameData.OnButtonPressA);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip != null && musicSource != null)
        {
            musicSource.clip = clip;
            musicSource.Play(); // Play music or restart if already playing
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
