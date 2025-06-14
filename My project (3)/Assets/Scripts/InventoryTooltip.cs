using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Script encargado de mostrar un panel de información al pasar el cursor sobre un ítem
public class InventoryTooltip : MonoBehaviour
{
    public static InventoryTooltip instance; // Para acceso global

    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    
    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false); // Asegurar que inicia desactivado
        Debug.Log("InventoryTooltip instance set!");  // <-- Verifica que se asigna correctamente.
    }

    public void ShowTooltip(Item item)
    {
        // Verificar si el item tiene información válida
        if (item == null)
        {
            Debug.LogWarning("El item es null en ShowTooltip!");
            return;
        }
        // Localizamos el texto según el idioma
        itemNameText.text = item.localizedName;
        itemDescriptionText.text = item.localizedDescription;

        // Verificar si los textos se asignan correctamente
        Debug.Log("Nombre del item: " + item.itemName);
        Debug.Log("Descripción del item: " + item.description);

        // Activar el Tooltip
        gameObject.SetActive(true);  // Asegúrate de que se activa el tooltip

        Debug.Log("Mostrando Tooltip para: " + item.itemName);  // Verifica que se muestre
    }

    // Método para ocultar el tooltip
    public void HideTooltip()
    {
        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
    }
}
