using UnityEngine;
using UnityEngine.UI;

// Contorla los ajustes de audio

public class AudioSettingsUI : MonoBehaviour
{
    // Referencias a sliders
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // Cargar valores guardados o establecer valores por defecto
        float savedMusicVol = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float savedSFXVol = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        musicSlider.value = savedMusicVol;
        sfxSlider.value = savedSFXVol;

        AudioManager.Instance.SetMusicVolume(savedMusicVol);
        AudioManager.Instance.SetSFXVolume(savedSFXVol);

        // Asignar eventos a los sliders
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    // Ajustar volumen de los sonidos
    public void SetMusicVolume(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    // Ajustar volumen de la música
    public void SetSFXVolume(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
}
