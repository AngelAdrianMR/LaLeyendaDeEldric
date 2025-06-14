using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Esta clase permite que un ítem pueda arrastrarse dentro de la UI  de inventario

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Referencias
    private Item item;
    private InventoryUI inventoryUI;
    private Transform parentAfterDrag;
    private Image image;

    // Método para guardar referencias y asignar icono
    public void SetItem(Item newItem, InventoryUI ui)
    {
        item = newItem;
        inventoryUI = ui;
        image = GetComponent<Image>();
        image.sprite = item.icon;
    }

    // Método para obtener el ítem
    public Item GetItem()
    {
        return item;
    }

    // Metodo al comenzar el arrastre, guardamos el padre y lo ponemos al frente
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    // Mueve el objeto siguiendo el puntero
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    // Mueve el objeto al soltar
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }
}
