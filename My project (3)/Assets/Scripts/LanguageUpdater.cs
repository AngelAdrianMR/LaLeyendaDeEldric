using TMPro;
using UnityEngine;

// Este script se encarga de actualizar los textos localizados en la interfaz
public class LanguageUpdater : MonoBehaviour
{
    void Start()
    {
        UpdateAllTexts(); // Se llama automáticamente al cargar la escena
    }

    // Método que actualiza todos los textos TMPro
    public void UpdateAllTexts()
    {
        // Encuentra todos los componentes TMPro
        TextMeshProUGUI[] texts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();

        foreach (var tmp in texts)
        {
            // Usa el nombre del objeto como clave para buscar la traducción
            string key = tmp.gameObject.name;

            // Obtener la traducción desde el LanguageManager
            string value = LanguageManager.Instance.GetText(key);

            // Asignar la traducción al texto
            tmp.text = value;
        }
    }
}
