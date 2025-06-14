using UnityEngine;
using System.Collections;

// Traducimos todos los textos.
public class LanguageInitializer : MonoBehaviour
{
    public ItemDatabase itemDatabase;

    void Start()
    {
        StartCoroutine(InitializeLanguage());
    }

    IEnumerator InitializeLanguage()
    {
        // Espera hasta que LanguageManager esté instanciado (máximo 1 segundo de espera)
        float timer = 0f;
        while (LanguageManager.Instance == null && timer < 1f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (LanguageManager.Instance != null)
        {
            // Cargar idioma desde PlayerPrefs
            string savedLang = PlayerPrefs.GetString("language", "es");
            LanguageManager.Instance.LoadLanguage(savedLang);

            // Actualizar todos los textos
            FindFirstObjectByType<LanguageUpdater>()?.UpdateAllTexts();
        }
        else
        {
            Debug.LogError("LanguageManager no está disponible tras esperar.");
        }
    }


}
