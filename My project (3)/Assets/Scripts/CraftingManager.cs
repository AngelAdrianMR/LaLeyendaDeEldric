using System.Collections.Generic;
using UnityEngine;

// Gestiona la lógica para fabricar/craftear objetos

public class CraftingManager : MonoBehaviour
{
    public InventoryManager inventoryManager; // Referencia al inventario
    public List<Recipe> recipes; // Lista de recetas disponibles

    public InventoryUI inventoryUI; // Referencia para la UI

    // Método para fabricar un objeto a partir de una receta
    public void CraftItem(Recipe recipe)
    {
        if (CanCraft(recipe))
        {
            // Eliminar los materiales del inventario usando MaterialType
            foreach (var material in recipe.materialsRequired)
            {
                int materialsRemoved = 0;
                List<Item> itemsToRemove = new List<Item>(); // Lista de items a eliminar

                foreach (var kvp in inventoryManager.inventory) // Recorremos el diccionario
                {
                    if (kvp.Key.materialType == material.materialType)
                    {
                        int available = kvp.Value; // Cantidad disponible de este material

                        // Cantidad que se debe eliminar
                        int toRemove = Mathf.Min(material.amount - materialsRemoved, available);
                        materialsRemoved += toRemove;

                        // Agregar a la lista de eliminación
                        if (toRemove > 0)
                            itemsToRemove.Add(kvp.Key);

                        // Si ya removimos la cantidad necesaria, salimos
                        if (materialsRemoved >= material.amount)
                            break;
                    }
                }

                // Ahora eliminamos los items de la lista
                foreach (var item in itemsToRemove)
                {
                    inventoryManager.RemoveItem(item, material.amount);
                }
            }

            // Buscar directamente el objeto Item en la receta
            GameObject resultItemInstance = Instantiate(recipe.resultPrefab); // Asegúrate de que Recipe tiene una variable resultItem del tipo Item
            // Accedemos al ítem fabricado
            ItemPickup itemPickup = resultItemInstance.GetComponent<ItemPickup>();

            if (itemPickup != null)
            {
                Item craftedItem = itemPickup.item;
                if (craftedItem != null)
                {
                    inventoryManager.AddItem(craftedItem); // Agregar el item al inventario correctamente
                    CheckCraftingMissions(craftedItem); // Verificamos si cumple misión
                    AudioManager.Instance.PlaySound(AudioManager.Instance.craftSound); // Sonido
                    Debug.Log("Has fabricado: " + craftedItem);
                }
                else 
                {
                    Debug.LogError("El item en el prefab es null");
                }
            }
            else
            {
                Debug.LogError("El item de la receta es null. Revisa la configuración de la receta.");
            }

            //Destruir el objeto de la escena
            Destroy(resultItemInstance);

            // Actualizar la UI del inventario
            inventoryUI.UpdateInventoryUI();
        }
        else
        {
            Debug.Log("No tienes suficientes materiales para fabricar esto.");
        }
    }

    // Método para verificar si se tienen los materiales necesarios
    public bool CanCraft(Recipe recipe)
    {
        if (recipe.materialsRequired == null || recipe.materialsRequired.Count == 0)
        {
            Debug.LogError("La receta no tiene materiales requeridos o la lista es null.");
            return false;
        }

        Debug.Log("Verificando materiales para la receta...");
        foreach (var material in recipe.materialsRequired)
        {
            Debug.Log("Comprobando material: " + material.materialType);

            // Cambiar la verificación a MaterialType en lugar de Prefab
            if (!inventoryManager.HasEnoughItems(material.materialType, material.amount))
            {
                Debug.LogWarning("No hay suficientes materiales: " + material.materialType);
                return false;
            }
        }
        return true;
    }

    // Revisa si el objeto fabricado completa una misión activa
    void CheckCraftingMissions(Item craftedItem)
    {
        foreach (var mission in MissionManager.Instance.activeMissions.ToArray())
        {
            if (mission.requieredCrafting && !mission.isCompleted)
            {
                if (craftedItem.itemName == mission.requieredItemName)
                {
                    mission.Complete(); // Marcamos la misión como completada
                    MissionManager.Instance.CompleteMission(mission.id);

                    Debug.Log("Misión completada al fabricar: " + craftedItem.itemName);
                }
            }
        }
    }

}
