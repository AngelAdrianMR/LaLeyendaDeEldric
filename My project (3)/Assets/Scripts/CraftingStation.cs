using UnityEngine;
using TMPro;

// Script que gestiona el comportamiento de la mesa de crafteo
public class CraftingStation : MonoBehaviour
{
    public GameObject infoPanel; // Panel de información
    public TextMeshProUGUI infoText; // Texto de información
    public GameObject inventoryPanel; // Panel de inventario
    public GameObject craftPanel; // Panel de fabricación
    public GameObject equipPanel; // Panel de equipo

    private bool isPlayerInRange = false; // Indica si el jugador esta en el area
    
    void Update()
    {
        if (isPlayerInRange)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                OpenCraftingMenu(); // Abre el panel de crafteo
            }
        }
    }

    // Método para abrir el panel de crafteo
    private void OpenCraftingMenu()
    {
        craftPanel.SetActive(true);
        infoPanel.SetActive(false);
                
    }

    // Si el jugador entra en el rango, ocultamos el panel equip para mostrar solo el de crafteo
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            equipPanel.SetActive(false);
            ShowInfoMessage(LanguageManager.Instance.GetText("interact_crafting_station"));
        }
    }

    // Si sale, lo mostramos para el inventario normal
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            HideInfoMessage();
            craftPanel.SetActive(false);
            equipPanel.SetActive(true);
            
        }
    }

    // Mostramos mensaje en el panel de información
    private void ShowInfoMessage(string message)
    {
        if (infoPanel != null && infoText != null)
        {
            infoPanel.SetActive(true);
            infoText.text = message;
        }
    }

    // Ocultamos panel
    private void HideInfoMessage()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }
}
