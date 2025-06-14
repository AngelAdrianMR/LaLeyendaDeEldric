using UnityEngine;

// Este script permite mostrar u ocultar el panel del inventario presionando la tecla "I"
public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel; // Referencia al panel visual del inventario en la UI
    public GameObject InfoPanel;

    // Al iniciar el juego, el inventario está oculto
    void Start()
    {
        inventoryPanel.SetActive(false);
    }

    // En cada frame se comprueba si el jugador ha pulsado la tecla I
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            // Alterna el estado del panel
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);

            InfoPanel.SetActive(false);
        }
    }
}

