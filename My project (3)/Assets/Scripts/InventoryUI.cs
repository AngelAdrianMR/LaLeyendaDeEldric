using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public ItemDatabase ItemDatabase; 
    public InventoryManager inventoryManager;  // Referencia al sistema de inventario
    public Transform inventoryGrid;           // Contenedor de los slots
    public GameObject inventorySlotPrefab;    // Prefab del slot

    public GameObject merchantPanel;  // panel del mercader
    public PlayerAtribute playerAtribute; // para acceder a las monedas

    // Slots para los diferentes tipos de equipoF
    public EquipmentSlot helmetSlot;
    public EquipmentSlot armorSlot;
    public EquipmentSlot glovesSlot;
    public EquipmentSlot bootsSlot;
    public EquipmentSlot swordSlot;
    public EquipmentSlot tool01Slot;
    public EquipmentSlot tool02Slot;

    void Start()
    {
        // Referencias necesarias
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        playerAtribute = FindFirstObjectByType<PlayerAtribute>();

        // Crear la UI ya traducida
        UpdateInventoryUI();
        UpdateEquipmentUI();
    }



    // Actualiza la visualización del inventario
    public void UpdateInventoryUI()
    {
        // Eliminar los slots anteriores
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);  // Elimina los slots previos
        }

        // Diccionario para almacenar los slots que contienen los mismos MaterialTypes
        Dictionary<MaterialType, GameObject> materialSlots = new Dictionary<MaterialType, GameObject>();

        // Contador de slots creados
        int index = 0;

        foreach (var kvp in inventoryManager.inventory)
        {
            if (index >= 49) break;  // Máximo 49 slots

            Item item = kvp.Key;
            int quantity = kvp.Value;

            if (item.itemType == ItemType.Material)
            {
                MaterialType materialType = item.materialType;  // Obtenemos el MaterialType del item

                if (materialSlots.ContainsKey(materialType))
                {
                    // Si el MaterialType ya existe en el diccionario, actualizar la cantidad
                    GameObject existingSlot = materialSlots[materialType];
                    TextMeshProUGUI quantityText = existingSlot.transform.Find("quantityText").GetComponent<TextMeshProUGUI>();

                    InventorySlot inventorySlot = existingSlot.GetComponent<InventorySlot>();
                    if (inventorySlot != null)
                    {
                        inventorySlot.SetItem(item);
                    }

                    if (quantityText != null)
                    {
                        int currentQuantity = 0;
                        if (!string.IsNullOrEmpty(quantityText.text)) 
                        {
                            int.TryParse(quantityText.text, out currentQuantity);
                        }

                        quantityText.text = (currentQuantity + quantity).ToString();
                    }

                    continue;  // No crear un nuevo slot
                }
                else
                {
                    // Si el MaterialType no existe, crear un nuevo slot
                    GameObject slot = Instantiate(inventorySlotPrefab, inventoryGrid);
                    slot.GetComponent<Image>().sprite = item.icon;

                    // asignar el item al slot
                    InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
                    if (inventorySlot != null)
                    {
                        inventorySlot.SetItem(item, quantity);

                        
                    }

                    // Acceder al objeto hijo "quantityText" y actualizar su texto
                    TextMeshProUGUI quantityText = slot.transform.Find("quantityText").GetComponent<TextMeshProUGUI>();
                    if (quantityText != null)
                    {
                        quantityText.text = quantity > 1 ? quantity.ToString() : ""; // Solo muestra el número si hay más de 1
                    }

                    // Asignar el item al slot para arrastrarlo (si es necesario)
                    DraggableItem draggableItem = slot.GetComponent<DraggableItem>();
                    if (draggableItem != null)
                    {
                        draggableItem.SetItem(item, this);
                    }

                    // Guardar el slot en el diccionario con la clave siendo el MaterialType
                    materialSlots[materialType] = slot;

                    index++; // Aumentar el contador solo al crear un nuevo slot
                }
            }
            else
            {
                // Si el item NO es Material, se agrega como objeto único (sin apilar)
                GameObject slot = Instantiate(inventorySlotPrefab, inventoryGrid);
                slot.GetComponent<Image>().sprite = item.icon;
                
                // asignar el item al slot
                InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
                if (inventorySlot != null)
                {
                    inventorySlot.SetItem(item, quantity);

                    
                }

                // Acceder al objeto hijo "quantityText" y actualizar su texto
                TextMeshProUGUI quantityText = slot.transform.Find("quantityText").GetComponent<TextMeshProUGUI>();
                if (quantityText != null)
                {
                    quantityText.text = quantity > 1 ? quantity.ToString() : ""; // Solo muestra el número si hay más de 1
                }

                //

                // Asignar el item al slot para arrastrarlo (si es necesario)
                DraggableItem draggableItem = slot.GetComponent<DraggableItem>();
                if (draggableItem != null)
                {
                    draggableItem.SetItem(item, this);
                }

                index++; // Aumentar el contador ya que es un objeto individual
            }
        }

        // Log después de actualizar
        Debug.Log("Inventory UI updated. Current inventory count: " + inventoryManager.inventory.Count);
    }

    // Actualizar los slots de equipo en el panel de equipo
    public void UpdateEquipmentUI()
    {
        // Actualizar la imagen de cada slot de equipo con el objeto correspondiente
        UpdateSlotImage(helmetSlot, ItemType.Helmet);
        UpdateSlotImage(armorSlot, ItemType.Armor);
        UpdateSlotImage(glovesSlot, ItemType.Gloves);
        UpdateSlotImage(bootsSlot, ItemType.Shoes);
        UpdateSlotImage(swordSlot, ItemType.Sword);
        UpdateSlotImage(tool01Slot, ItemType.Pickaxe);
        UpdateSlotImage(tool02Slot, ItemType.Axe);
    }

    // Método para actualizar la imagen de un slot específico
    private void UpdateSlotImage(EquipmentSlot slot, ItemType type)
    {
        // Si hay un objeto equipado en este tipo de slot, actualizar su imagen
        if (inventoryManager.equippedItems.ContainsKey(type))
        {
            string itemName = inventoryManager.equippedItems[type];  // Obtenemos el nombre del ítem
            Item item = inventoryManager.GetItemByName(itemName);   // Obtenemos el objeto Item completo

            if (item != null)
            {
                slot.UpdateSlotImage(item);
            }
            else
            {
                slot.UpdateSlotImage(null);  // Si no hay objeto, ocultar la imagen
            }
        }
        else
        {
            slot.UpdateSlotImage(null);  // Si no hay objeto, ocultar la imagen
        }
    }

    // Método para equipar un item
    public void EquipItem(Item item)
    {
        inventoryManager.EquipItem(item);
        UpdateInventoryUI();
        UpdateEquipmentUI();
    }

    // Método para desequipar un item
    public void UnequipItem(ItemType itemType)
    {
        inventoryManager.UnequipItem(itemType);
        UpdateInventoryUI();
        UpdateEquipmentUI();  
    }

    // Usa una poción desde la UI
    public void UsePotion(Item item) 
    {
        inventoryManager.UsePotion(item);
        UpdateInventoryUI();
    }
}
