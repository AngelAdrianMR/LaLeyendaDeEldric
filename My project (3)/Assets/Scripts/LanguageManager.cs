using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance; // Singleton para acceso global

    public LanguageUpdater languageUpdater;   // Referencia al actualizador de textos de UI

    private Dictionary<string, string> localizedText; // Diccionario de claves y textos traducidos
    private string currentLanguage; // Código del idioma actual ("es", "en")

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Cargar el idioma desde PlayerPrefs o usar "es" por defecto
            string savedLang = PlayerPrefs.GetString("language", "es");
            LoadLanguage(savedLang);

            // Opcional: actualizar textos de UI automáticamente si está presente
            languageUpdater = FindFirstObjectByType<LanguageUpdater>();
            languageUpdater?.UpdateAllTexts();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Cargar archivo de idioma desde Resources/lang_es.json o lang_en.json
    public void LoadLanguage(string langCode)
    {
        currentLanguage = langCode;
        localizedText = new Dictionary<string, string>();

        // Cargar archivo desde Resources
        TextAsset langData = Resources.Load<TextAsset>($"lang_{langCode}");

        if (langData == null)
        {
            Debug.LogError("Archivo de idioma no encontrado: lang_" + langCode);
            return;
        }

        // Formatear y deserializar el JSON a un diccionario
        localizedText = JsonUtility.FromJson<LangWrapper>("{\"data\":" + langData.text + "}").ToDictionary();
    }

    // Obtener el texto traducido usando una clave
    public string GetText(string key)
    {
        if (localizedText != null && localizedText.ContainsKey(key))
        {
            return localizedText[key];
        }

        // Si no se encuentra la clave, se devuelve la clave como valor por defecto
        return key;
    }

    // Método público para obtener el lenguaje actual
    public string GetSelectedLanguage()
    {
        return currentLanguage;
    }

    // Clases auxiliares para deserializar el archivo JSON
    [System.Serializable]
    private class LangWrapper
    {
        public List<LangEntry> data;

        // Convierte la lista a un diccionario para facilitar la búsqueda por clave
        public Dictionary<string, string> ToDictionary()
        {
            var dict = new Dictionary<string, string>();
            foreach (var entry in data)
            {
                dict[entry.key] = entry.value;
            }
            return dict;
        }
    }

    [System.Serializable]
    private class LangEntry
    {
        public string key;   // Clave (por ejemplo: "item_espada_name")
        public string value; // Traducción (por ejemplo: "Espada de Hierro")
    }
}
