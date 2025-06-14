using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

// Clase serializable para guardar y cargar el inventario
[System.Serializable]
public class InventoryData
{
    public List<string> itemNames = new List<string>(); // Lista de ítems en el inventario
    public List<int> itemQuantities = new List<int>(); // Cantidad de cada ítem
    public Dictionary<ItemType, string> equippedItems = new Dictionary<ItemType, string>(); // Ítems equipados por tipo
}

public class InventoryManager : MonoBehaviour
{
    public ItemDatabase ItemDatabase; // Base de datos de los ítems
    private Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>(); // Ítems accesibles por nombre

    public Dictionary<Item, int> inventory = new Dictionary<Item, int>(); // Lista de objetos en el inventario
    public const int maxInventorySize = 49; // Tamaño máximo del inventario
    public Dictionary<ItemType, string> equippedItems = new Dictionary<ItemType, string>(); // Objetos equipados

    // Referencias
    private PlayerAtribute playerAtribute;
    private PlayerMovement playerMovement;
    private InventoryUI inventoryUI;
    public GameObject player;

    void Awake()
    {
        foreach (var item in ItemDatabase.allItems)
        {
            // Rellenamos la lista con los nombres en minúscula
            itemDictionary[item.itemName.ToLower()] = item; 
        }

        if (inventoryUI == null)
        {
            // Obtenemos la UI si no está asignada
            inventoryUI = FindFirstObjectByType<InventoryUI>();
        }
    }

    void Start()
    {
        // Obtener los componentes PlayerAtribute y PlayerMovement del jugador
        if (player != null)
        {
            playerAtribute = player.GetComponent<PlayerAtribute>();
            playerMovement = player.GetComponent<PlayerMovement>();
        }

        // Si no se encuentran los componentes, muestra un error
        if (playerAtribute == null)
        {
            Debug.LogError("El componente PlayerAtribute no está asignado en el jugador.");
        }
        if (playerMovement == null)
        {
            Debug.LogError("El componente PlayerMovement no está asignado en el jugador.");
        }
    }

    // Agrega un ítem al inventario
    public void AddItem(Item item, int quantity = 1)
    {
        if (item == null)
        {
            Debug.LogError("Intentando añadir un item nulo al inventario.");
            return;
        }

         // Traducir el ítem al directamenrte
        if (string.IsNullOrEmpty(item.localizedName) && !string.IsNullOrEmpty(item.keyName))
        {
            item.localizedName = LanguageManager.Instance.GetText(item.keyName);
            item.localizedDescription = LanguageManager.Instance.GetText(item.keyDesc);
        }

        // Buscamos materiales iguales por tipo de material
        if (item.itemType == ItemType.Material)
        {
            foreach (var kvp in inventory)
            {
                if (kvp.Key.materialType == item.materialType)
                {
                    inventory[kvp.Key] += quantity;
                    Debug.Log($"Apilado {item.itemName} x{quantity}. Total: {inventory[kvp.Key]}");
                    return;
                }
            }
        }

        // Si este material existe aumentamos su cantidad, si no ponemos la cantidad obtenida
        if (inventory.ContainsKey(item))
        {
            inventory[item] += quantity;
        }
        else
        {
            inventory[item] = quantity;
        }

        if (item.isPotion)
        {
            // Activar misión si no existe
            if (!MissionManager.Instance.activeMissions.Any(m => m.id == "m3") &&
                !MissionManager.Instance.completedMissions.Any(m => m.id == "m3"))
            {
                MissionManager.Instance.AddMissionById("m3");
            }

            // Si ya está activa, completarla
            var mision = MissionManager.Instance.GetMissionById("m3");
            if (mision != null && mision.isActive && !mision.isCompleted)
            {
                MissionManager.Instance.CompleteMission("m3");
            }
        }


    }

    // Eliminar un item del inventario
    public void RemoveItem(Item item, int amountRemove = 1)
    {
        if (inventory.ContainsKey(item))
        {
            if (inventory[item] > amountRemove)
            {
                inventory[item] -= amountRemove; 
            }
            else
            {
                inventory.Remove(item); // Si solo queda 1, lo eliminamos completamente
            }

        }
    }

    // Equipar un item en el panel de equipo
    public void EquipItem(Item item)
    {
        ItemType itemType = item.itemType;

        // Desequipar si ya hay uno en ese equipo
        if (equippedItems.ContainsKey(item.itemType))
        {
            UnequipItem(item.itemType);
        }
        
        // Guarda el objeto en equipados
        equippedItems[item.itemType] = item.itemName;

        //Aplicamos los efectos del objeto
        ApplyItemEffects(item);
        
        //Eliminamos el ítem
        RemoveItem(item);

        // Actulizar UI
        inventoryUI.UpdateInventoryUI();
        inventoryUI.UpdateEquipmentUI();
    }

    // Desequipar ítem
    public void UnequipItem(ItemType itemType)
    {
        if (equippedItems.ContainsKey(itemType))
        {
            string itenName = equippedItems[itemType];
            Item itemToUnequip = GetItemByName(itenName);

            // Volver el objeto al inventario
            if (itemToUnequip != null)
            {
                AddItem(itemToUnequip);
            }

            // Eliminar el objeto del slot equipado
            equippedItems.Remove(itemType);

            // Quitar los efectos del objeto
            if (itemToUnequip != null)
            {
                RemoveItemEffects(itemToUnequip);
            }
        }
    }

    // método para utilizar la poción
    public void UsePotion(Item item)
    {
        if (item != null && item.isPotion)
        {
            // Aplicar los efectos de la poción al jugador
            ApplyPotionEffect(item);

            // Eliminar la poción del inventario después de usarla
            RemoveItem(item);
        }
    }

    // Se aplica los efectos del item equipado
    private void ApplyItemEffects(Item item)
    {
        if (playerAtribute != null)
        {
            playerAtribute.maxHealth += item.healthBonus;
            playerAtribute.attackPoints += item.attackBonus;
            playerMovement.moveSpeed += item.speedBonus;
        }
    }

    // Se aplica los efectos de las pociones consumidas
    private void ApplyPotionEffect(Item item)
    {
        playerAtribute.currentHealth += item.plusHealth;
        playerAtribute.currentStamina += item.plusStamina;
    }

    // Se eliminan los efectos de los items desequipados
    private void RemoveItemEffects(Item item)
    {
        if (playerAtribute != null)
        {
            playerAtribute.maxHealth -= item.healthBonus;
            playerAtribute.attackPoints -= item.attackBonus;
            playerMovement.moveSpeed -= item.speedBonus;
        }
    }

    // Verificamos si esta equipado
    public bool HasEquipped(ItemType itemType)
    {
        return equippedItems.ContainsKey(itemType);
    }

    // Verifica si el jugador tiene suficientes materiales para fabricar un objeto
    public bool HasEnoughItems(MaterialType requiredMaterial, int requiredAmount)
    {
        int count = 0;

        foreach (var item in inventory)
        {
            if (item.Key.materialType == requiredMaterial) // Comparación por tipo
            {
                count += item.Value; 
            }
        }

        return count >= requiredAmount;
    }

    // Obtenemos la biblioteca del inventario
    public InventoryData GetInventoryData()
    {
        InventoryData data = new InventoryData();

        // Se buscan los ítems y la cantidad
        foreach (var item in inventory)
        {
            data.itemNames.Add(item.Key.itemName);
            data.itemQuantities.Add(item.Value);
        }

        // Se buscan items equipados
        foreach (var equip in equippedItems)
        {
            data.equippedItems[equip.Key] = equip.Value;
        }
        return data;
    }

    // Cargar los datos del inventario
    public void LoadInventoryData(InventoryData data)
    {
        inventory.Clear(); // Limpiar el inventario antes de cargarlo.

        // Cargar items en el inventario
        for (int i = 0; i < data.itemNames.Count; i++)
        {
            string itemName = data.itemNames[i];
            int quantity = data.itemQuantities[i];

            // Obtener el objeto Item por su nombre
            Item item = GetItemByName(itemName);
            if (item != null)
            {
                // Añadir el item al inventario con la cantidad correcta
                AddItem(item, quantity);
            }
            else
            {
                Debug.LogWarning("No se encontró el item con nombre: " + itemName);
            }
        }

        // Cargar los objetos equipados
        foreach (var equippedItem in data.equippedItems)
        {
            string itemName = equippedItem.Value;  // Nombre del ítem equipado
            ItemType itemType = equippedItem.Key;  // Tipo de ítem

            // Obtener el objeto Item por su nombre
            Item item = GetItemByName(itemName);
            if (item != null)
            {
                // Equipar el item
                equippedItems[itemType] = itemName;
                ApplyItemEffects(item);  // Aplicar los efectos del item
            }
            else
            {
                Debug.LogWarning("No se encontró el item equipado con nombre: " + itemName);
            }
        }

        inventoryUI.UpdateInventoryUI();
        inventoryUI.UpdateEquipmentUI();
    }

    // Obtenemos un ítem por su nombre
    public Item GetItemByName(string itemName)
    {
        // Verificar si el nombre esta null o vacio
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogWarning("Intentando buscar un item con nombre vacío.");
            return null;
        }

        // Convertimos el nombre a minúsculas
        string lowerItemName = itemName.ToLower();

        // Obtenemos el item desde el diccionario usando el nombre
        if (itemDictionary.TryGetValue(lowerItemName, out Item item))
        {
            return item;
        }

        Debug.LogWarning($"No se encontró el objeto en la base de datos: {itemName}");
        return null;
    }
}


