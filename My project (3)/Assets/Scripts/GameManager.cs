using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// Este script gestiona el guardado, carga y estado general del juego

public class GameManager : MonoBehaviour
{
    private string saveFilePath; // Ruta del archivo de guardado
    private PlayerAtribute player; // Referencia al script que contiene los atributos del jugador
    private PlayerMovement playerMovement; // Movimiento del jugador
    private InventoryManager inventoryManager; // Inventario
    private GameManager gameManager; // Referencia a sí mismo

    //IDs de objetos destruidos
    private HashSet<string> destroyedObjects = new HashSet<string>();

    // Panel de gameOver
    public GameObject gameOver;

    void Awake()
    {
        // Define la ruta del archivo de guardado
        saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
    
        // Cargar idioma desde PlayerPrefs al iniciar
        string lang = PlayerPrefs.GetString("language", "es");
        LanguageManager.Instance.LoadLanguage(lang);
    }

    void Start()
    {
        // Busca los componentes necesarios en la escena
        player = FindFirstObjectByType<PlayerAtribute>();
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        gameManager = FindFirstObjectByType<GameManager>();

        // Comprobamos si alguno de los objetos es null.
        if (player == null)
        {
            Debug.LogError("PlayerAtribute no encontrado en la escena.");
        }
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement no encontrado en la escena.");
        }
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager no encontrado en la escena.");
        }

        int loadGame = PlayerPrefs.GetInt("LoadGame", 0);

        if (loadGame == 1)
        {
            Debug.Log("Cargando partida guardada...");
            LoadGame();
        }
        else 
        {
            Debug.Log("Cargando nueva partida.");
        }
    }

    // Guarda el estado actual del juego
    public void SaveGame()
    {
        SaveData data = new SaveData(player, playerMovement, inventoryManager, this);

        // Crear un objeto InventoryData
        InventoryData inventoryData = new InventoryData();

        // Agregar los nombres de los items y sus cantidades
        foreach (var kvp in inventoryManager.inventory)
        {
            inventoryData.itemNames.Add(kvp.Key.itemName);  // Asumiendo que 'kvp.Key' es un 'Item' que tiene 'itemName'
            inventoryData.itemQuantities.Add(kvp.Value);    // Asumiendo que 'kvp.Value' es la cantidad del item
        }

        // Guardar los elementos equipados
        foreach (var equip in inventoryManager.equippedItems)
        {
            inventoryData.equippedItems.Add(equip.Key, equip.Value);
        }

        // Guardar el objeto InventoryData en el SaveData
        data.inventoryData = inventoryData;

        // Guardar el JSON
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Juego guardado en: " + saveFilePath);
    }

    // Carga los datos guardados del juego
    public void LoadGame()
    {
        // Busca los objetos en la nueva escena
        player = FindFirstObjectByType<PlayerAtribute>();
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        gameManager = FindFirstObjectByType<GameManager>();

        // Verifica si los objetos fueron encontrados
        if (player == null)
        {
            Debug.LogError("No se encontró PlayerAtribute en la escena.");
        }
        if (playerMovement == null)
        {
            Debug.LogError("No se encontró PlayerMovement en la escena.");
        }
        if (inventoryManager == null)
        {
            Debug.LogError("No se encontró InventoryManager en la escena.");
        }

        // Si los objetos fueron encontrados, carga los datos del juego
        if (player != null && playerMovement != null && inventoryManager != null)
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                data.LoadInto(player, playerMovement, inventoryManager, this);
                
                Debug.Log("Cargando inventario");
                inventoryManager.LoadInventoryData(data.inventoryData);

                Debug.Log("cargando lista de objetos destruidos...");
                SetDestroyedObjects(data.destroyedObjects);

                Debug.Log("Juego cargado.");
            }
            else
            {
                Debug.LogWarning("No hay partida guardada.");
            }
        }

        // Actualiza la interfaz del inventario si está presente
        InventoryUI inventoryUI = FindFirstObjectByType<InventoryUI>();

        if (inventoryUI != null)
        {
            inventoryUI.UpdateInventoryUI();
            inventoryUI.UpdateEquipmentUI();
        }
    }

    // Sale al menú principal
    public void ExitToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main");
    }

    // Devuelve la lista de IDs de objetos destruidos
    public List<string> GetDestroyedObjects()
    {
        return new List<string>(destroyedObjects);
    }

    // Establece la lista de objetos destruidos y los elimina visualmente de la escena
    public void SetDestroyedObjects(List<string> loadedObjects)
    {
        destroyedObjects = new HashSet<string>(loadedObjects);

        // Buscar manualmente en todos los WorldObject y eliminarlos
        WorldObject[] allObjects = FindObjectsByType<WorldObject>(FindObjectsSortMode.None);

        // Eliminar objetos en la escena
        foreach (string objectID in destroyedObjects)
        {
            foreach (WorldObject obj in allObjects)
            {
                if (obj.objectID == objectID)
                {
                    Debug.Log("Eliminando objeto tras carga: " + objectID);
                    Destroy(obj.gameObject);
                }
            }
        }
    }

    // Registra un nuevo objeto destruido en la lista
    public void RegisterDestroyedObject(string objectID)
    {
        destroyedObjects.Add(objectID);
    }

    // Verifica si un objeto ya ha sido destruido
    public bool IsObjectDestroyed(string objectID)
    {
        return destroyedObjects.Contains(objectID);
    }

    // Busca y devuelve un objeto de la escena según su ID
    private GameObject FindObjectByID(string objectID)
    {
        WorldObject[] allObjects = FindObjectsByType<WorldObject>(FindObjectsSortMode.None);
        foreach (WorldObject obj in allObjects)
        {
            if (obj.objectID == objectID)
            {
                return obj.gameObject;
            }
        }
        return null;
    }

    public void CloseGameOverPanel()
    {
        // Ocultar panel GameOver
        if (gameOver != null)
        {
            gameOver.SetActive(false);
        }
    }

}
