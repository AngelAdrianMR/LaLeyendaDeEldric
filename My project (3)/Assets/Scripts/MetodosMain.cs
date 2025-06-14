using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

// Script que proporciona métodos reutilizables desde el menú principal
public class MetodosMain : MonoBehaviour
{
    private GameManager gameManager; // Referencia al sistema de gestión del juego

    void Start()
    {
        // Buscar GameManager en la escena
        gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("No se encontró GameManager en la escena");
        }
    }

    // Método para cargar partida guardada desde una ruta específica
    public void Load()
    {
        if (gameManager != null)
        {
            // Ruta fija del archivo de guardado (ejemplo en una máquina local)
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, "saveData.json");
            Debug.Log("Buscando archivo en: " + filePath);

            // Si el archivo existe, se intenta cargar el juego
            if (File.Exists(filePath))
            {
                Debug.Log("Archivo encontrado. Cargando juego...");
                gameManager.LoadGame();
            }
            else
            {
                Debug.LogWarning("No se encontró el archivo de guardado en la ruta especificada.");
            }
        }
        else
        {
            Debug.LogError("GameManager no está inicializado.");
        }
    }
}
