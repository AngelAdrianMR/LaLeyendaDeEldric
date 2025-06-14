using UnityEngine;

// Este script representa un ítem físico que el jugador puede recoger del suelo
public class ItemPickup : MonoBehaviour
{
    public Item item; // Referencia al objeto que este pickup representa

    private WorldObject worldObject; // Referencia al objeto del mundo (para control de destrucción persistente)

    void Start() 
    {
        // Intentamos obtener el componente WorldObject del objeto
        worldObject = GetComponent<WorldObject>();
    }

    // Cuando el jugador entra en el área de colisión del ítem
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                // Asigna este ítem como el más cercano para el jugador
                player.nearbyItem = this;
            }
        }
    }

    // Cuando el jugador se aleja del ítem
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null && player.nearbyItem == this)
            {
                // Si era el ítem asignado, se limpia la referencia
                player.nearbyItem = null;
            }
        }
    }

    // Método para recoger el objeto
    public void PickupItem()
    {
        InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();

        // Añadir el ítem al inventario
        inventoryManager.AddItem(item);

        // Actualizar la interfaz gráfica del inventario
        FindFirstObjectByType<InventoryUI>().UpdateInventoryUI();

        // Eliminar el objeto del mundo
        if (worldObject != null)
        {
            Debug.Log("El item fue guardado");
            worldObject.DestroyObject(); // Usamos el sistema persistente de destrucción
        }
        else
        {
            Destroy(gameObject); // Eliminación normal si no tiene WorldObject
        }
    }
}
