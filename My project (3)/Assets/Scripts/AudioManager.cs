using UnityEngine;

// Gestiona la reproducción de los sonidos

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;  // Para música de fondo
    public AudioSource sfxSource;    // Para efectos de sonido

    // Diferentes sonidos para las acciones
    [Header("Audio Clips")]
    public AudioClip coinSound;
    public AudioClip playerHitSound;
    public AudioClip enemyHitSound;
    public AudioClip craftSound;
    public AudioClip breakSound;
    public AudioClip bgMusic;

    private void Awake()
    {
        // Configuramos como singleton solo para esta escena
        Instance = this;
    }

    private void Start()
    {
        // Reproducir música de fondo al iniciar
        PlayMusic(bgMusic);
    }

    // Reproduce un efecto de sonido
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Inicia la música de fondo
    public void PlayMusic(AudioClip music)
    {
        if (music != null && musicSource != null)
        {
            musicSource.clip = music;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // Controlar volumen de la música (desde un slider)
    public void SetMusicVolume(float value)
    {
        if (musicSource != null)
        {
            musicSource.volume = value;
        }
    }

    // Controlar volumen de efectos de sonido (desde un slider)
    public void SetSFXVolume(float value)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = value;
        }
    }
}
