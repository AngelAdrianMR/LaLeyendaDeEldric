using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Item item; // El ítem asignado al slot
    private GameObject merchantPanel; // Asignar desde InventoryUI

    private int quantity; // Cantidad de items en el slot
    public TextMeshProUGUI quantityText; // Txt de la cantidad

    private PlayerAtribute player; // Referencia para stats
    private InventoryManager inventory; // Acceso a metodos

    void Start()
    {
        // Buscamos los objetos
        player = FindFirstObjectByType<PlayerAtribute>();
        inventory = FindFirstObjectByType<InventoryManager>();
        merchantPanel = GameObject.Find("MerchantPanel");
    }

    public void SetItem(Item newItem , int newQuantity = 1)
    {
        if (newItem == null)
        {
            Debug.LogWarning("SetItem recibió un item NULL.");
            return;
        }

        item = newItem; // Asignamos el item
        quantity = newQuantity; // Asignamos la cantidad

        if (quantityText != null)
        {
            // Mostramos la cantidad si es mayor que 1
            quantityText.text = quantity > 1 ? $"x{quantity}" : "";
        }
    }

    // Obtenemos la cantidad
    public int GetQuantity()
    {
        return quantity;
    }
   

    // Al pasar el ratón por encima del slot
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            // Mostrar tooltip
            InventoryTooltip.instance.ShowTooltip(item);
        }
    }

    // Al salir el ratón del slot
    public void OnPointerExit(PointerEventData eventData)
    {
        // Ocultar el tooltip
        InventoryTooltip.instance.HideTooltip();
    }

    // Al hacer click sobre un item
    public void OnPointerClick(PointerEventData eventData)
    {
        // si se encuentra en el panel de mercado vendemos
        if (merchantPanel != null && merchantPanel.activeSelf && item != null)
        {
            if (player != null && inventory != null)
            {
                inventory.RemoveItem(item); // Eliminamos el item
                player.AddCoins((int)item.price); // Obtenemos el dinero

                // Actualizar UI
                FindFirstObjectByType<InventoryUI>()?.UpdateInventoryUI();
            } 
            
        }
        // Si el item es una poción la consumimos
        if (item.isPotion)
        {
            // Consumimos poción
            inventory.UsePotion(item);
            // Actualizar UI
            FindFirstObjectByType<InventoryUI>()?.UpdateInventoryUI();
        }
    }
}
