using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public ItemType slotType;  // Tipo de objeto que puede equiparse aquí
    public InventoryUI inventoryUI;
    private Image slotImage; // Imagen que se actualizara con el icono del objeto

    public Sprite defaultSlotImage;

    void Start()
    {
        // Obtenemos la imagen del slot para actualizarla
        slotImage = GetComponent<Image>();

        if (defaultSlotImage != null)
        {
            slotImage.sprite = defaultSlotImage;  // Asignamos la imagen predeterminada
            slotImage.enabled = true;
        }
        else
        {
            Debug.LogWarning("No se ha asignado la imagen predeterminada para el slot " + slotType);
        }
    }

    // Método para arrastrar el objeto hacia equipamiento
    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();

        if (draggedItem != null && draggedItem.GetItem().itemType == slotType)
        {
            inventoryUI.EquipItem(draggedItem.GetItem());
            Destroy(draggedItem.gameObject);
        }
    }

    // Este método es llamado cuando se hace clic en un slot
    public void OnPointerClick(PointerEventData eventData)
    {
        InventoryManager inventoryManager = inventoryUI.inventoryManager;

        if (inventoryManager.equippedItems.ContainsKey(slotType))
        {
            // Si ya hay un objeto equipado, lo desequipamos
            inventoryUI.UnequipItem(slotType);
            slotImage.sprite = defaultSlotImage;
        }
    }

    // Método para actualizar la imagen del slot con el icono del objeto equipado
    public void UpdateSlotImage(Item item)
    {
        if (slotImage == null)
        {
            return;
        }
        
        // Si hay un objeto, mostramos su icono en el slot
        if (item != null && item.icon != null)
        {
            slotImage.sprite = item.icon;
            slotImage.enabled = true;  // Asegúrate de que la imagen es visible
        }
        else
        {
            // Si no hay objeto, mostramos la imagen predeterminada
            if (defaultSlotImage != null)
            {
                slotImage.sprite = defaultSlotImage;
                slotImage.enabled = true;
            }
            
        }

    }
}
