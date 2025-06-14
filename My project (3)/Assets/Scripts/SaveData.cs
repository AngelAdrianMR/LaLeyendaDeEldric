using System.Collections.Generic;
using UnityEngine;

// Esta clase almacena los datos necesarios para guardar la partida

[System.Serializable]
public class SaveData
{
    // Atributos del jugador
    public int health;
    public int stamina;
    public int currentHealth;
    public int currentStamina;
    public int attackLevel;
    public int miningLevel;
    public int choppingLevel;
    public int runningLevel;
    public int staminaLevel;
    public int attackXP;
    public int miningXP;
    public int choppingXP;
    public int runningXP;
    public int staminaXP;
    public int coins;
    public string playerName;

    public float posX, posY;  // Posición del jugador
    public float moveSpeed;   // Velocidad de movimiento
    public float runMultiplier; // Multiplicador de velocidad

    public InventoryData inventoryData; // Datos del inventario
    public List<EquippedItem> equippedItems = new List<EquippedItem>(); // Lista de objetos equipados
    public List<string> destroyedObjects = new List<string>(); // Lista de objetos destruidos en el mapa

    public List<Mission> savedActiveMissions = new List<Mission>(); // Misiones activas
    public List<Mission> savedCompletedMissions = new List<Mission>(); // Misiones completadas

    // Constructor actualizado para usar InventoryData
    public SaveData(PlayerAtribute player, PlayerMovement movement, InventoryManager inventory, GameManager gameManager)
    {
        // Guardar atributos del jugador
        health = player.maxHealth;
        stamina = player.maxStamina;
        currentHealth = player.currentHealth;
        currentStamina = player.currentStamina;
        attackLevel = player.attackLevel;
        miningLevel = player.miningLevel;
        choppingLevel = player.choppingLevel;
        runningLevel = player.runningLevel;
        staminaLevel = player.staminaLevel;
        attackXP = player.attackXP;
        miningXP = player.miningXP;
        choppingXP = player.choppingXP;
        runningXP = player.runningXP;
        staminaXP = player.staminaXP;
        coins = player.coins;

        // Guardar posición
        posX = movement.transform.position.x;
        posY = movement.transform.position.y;

        // Guardar velocidad
        moveSpeed = movement.moveSpeed;
        runMultiplier = movement.runSpeedMultiplier;

        // Guardar inventario como InventoryData
        inventoryData = inventory.GetInventoryData();  // Usamos el método que convierte el inventario a InventoryData

        // Guardar objetos equipados
        foreach (var equip in inventory.equippedItems)
        {
            equippedItems.Add(new EquippedItem(equip.Key, equip.Value));
        }

        // Guardar objetos destruidos
        destroyedObjects = gameManager.GetDestroyedObjects();

        // Guardar estado de misiones
        savedActiveMissions = new List<Mission>(MissionManager.Instance.activeMissions);
        savedCompletedMissions = new List<Mission>(MissionManager.Instance.completedMissions);
    }

    // Cargar los datos
    public void LoadInto(PlayerAtribute player, PlayerMovement movement, InventoryManager inventory, GameManager gameManager)
    {
        // Cargar atributos
        player.maxHealth = health;
        player.maxStamina = stamina;
        player.currentHealth = currentHealth;
        player.currentStamina = currentStamina;
        player.attackLevel = attackLevel;
        player.miningLevel = miningLevel;
        player.choppingLevel = choppingLevel;
        player.runningLevel = runningLevel;
        player.staminaLevel = staminaLevel;
        player.attackXP = attackXP;
        player.miningXP = miningXP;
        player.choppingXP = choppingXP;
        player.runningXP = runningXP;
        player.staminaXP = staminaXP;
        player.coins = coins;

        // Cargar posición
        movement.transform.position = new Vector3(posX, posY, 0);

        // Cargar velocidad
        movement.moveSpeed = moveSpeed;
        movement.runSpeedMultiplier = runMultiplier;

        // Cargar inventario y elementos destruidos con un retraso
        if (inventory != null)
        {
            gameManager.StartCoroutine(DelayedLoadInventory(inventory));  // Retardamos la carga
        }
        else
        {
            Debug.LogError("El inventario no está inicializado.");
        }

        // Verificar que GameManager aún existe antes de llamar StartCoroutine
        if (gameManager != null)
        {
            gameManager.StartCoroutine(DelayedDestroyObjects(gameManager));
        }
        else
        {
            Debug.LogError("GameManager es null al intentar eliminar objetos.");
        }
    
        // Restaurar misiones
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.LoadMissionsFromSave(savedActiveMissions, savedCompletedMissions);
        }

    }

    private System.Collections.IEnumerator DelayedLoadInventory(InventoryManager inventory)
    {
        // Esperamos un momento para asegurar que todo esté inicializado correctamente
        yield return new WaitForSeconds(2f);

        float startTime = Time.time;

        // Cargar inventario
        if (inventory != null)
        {
            inventory.LoadInventoryData(inventoryData);
            Debug.Log("Inventario cargado");
        }
        else
        {
            Debug.LogError("Inventario no pudo ser cargado.");
        }

        // Cargar elementos equipados
        foreach (var equippedItem in equippedItems)
        {
            // Verificar si el objeto ya está en el inventario antes de equiparlo
            Item item = inventory.GetItemByName(equippedItem.itemName);
            if (item != null)
            {
                inventory.EquipItem(item);
                Debug.Log("Elemento equipado: " + equippedItem.itemName);
            }
            else
            {
                Debug.LogWarning("No se encontró el item para equipar: " + equippedItem.itemName);
            }

            // Verificamos el tiempo de ejecución para asegurarnos que no se quede atascado
            if (Time.time - startTime > 5f) // Si tarda más de 5 segundos
            {
                Debug.LogError("La carga del inventario está tardando demasiado.");
                break; // Salir del bucle si excede el tiempo de espera
            }
        }
    }

    //Corutina para eliminar objetos destruidos en la escena
    private System.Collections.IEnumerator DelayedDestroyObjects(GameManager gameManager)
    {
        yield return new WaitForSeconds(2f); // Esperar a que los objetos se carguen en la escena
        gameManager.SetDestroyedObjects(destroyedObjects);
    }
}

// Clase para representar un item del inventairo con su cantidad
[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public int quantity;

    public InventoryItem(string name, int qty)
    {
        itemName = name;
        quantity = qty;
    }
}

// Clase para guardar un item equipado
[System.Serializable]
public class EquippedItem
{
    public ItemType itemType;
    public string itemName;

    public EquippedItem(ItemType type, string name)
    {
        itemType = type;
        itemName = name;
    }
}
