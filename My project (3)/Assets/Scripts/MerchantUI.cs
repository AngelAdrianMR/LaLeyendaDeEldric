using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MerchantUI : MonoBehaviour
{
    public GameObject itemButtonPrefab;        // Prefab del botón de compra
    public Transform merchantItemList;         // Contenedor donde se colocan los botones de ítems

    public InventoryManager inventoryManager;  // Referencia al inventario del jugador
    public ItemDatabase itemDatabase;          // Base de datos de ítems disponibles en la tienda
    public PlayerAtribute player;              // Referencia al jugador para controlar monedas
    public InventoryUI inventoryUI;            // Para actualizar visualmente el inventario

    public GameObject equipmentPanel;          // Panel de equipo del jugador
    public GameObject inventory;               // Panel de inventario del jugador
    public GameObject merchantPanel;           // Panel principal del comerciante

    // Al activar el panel, actualiza la lista de ítems
    void OnEnable()
    {
        inventory.SetActive(true);            // Asegura que el inventario esté visible
        UpdateMerchantList();                 // Genera los botones de compra
    }

    // Método que actualiza todos los botones con los ítems disponibles en la tienda
    public void UpdateMerchantList()
    {
        // Limpia botones anteriores si los hubiera
        foreach (Transform child in merchantItemList)
        {
            Destroy(child.gameObject);
        }

        // Recorre todos los ítems de la base de datos
        foreach (var item in itemDatabase.allItems)
        {
            item.UpdateLocalization();
            
            GameObject button = Instantiate(itemButtonPrefab, merchantItemList); // Crear botón
            button.GetComponentInChildren<TextMeshProUGUI>().text = item.localizedName + " (" + item.price + ")";

            // Asigna acción al hacer clic en el botón
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                // Verifica si el jugador tiene suficientes monedas
                if (player.coins >= item.price)
                {
                    inventoryManager.AddItem(item, 1);        // Añade el ítem al inventario
                    player.AddCoins((int)-item.price);        // Resta las monedas
                    inventoryUI.UpdateInventoryUI();          // Refresca la interfaz
                }
                else
                {
                    Debug.Log("No tienes suficiente oro.");
                }
            });
        }
    }

    // Método para cerrar la interfaz del comerciante
    public void CloseMerchant()
    {
        equipmentPanel.SetActive(true);    // Vuelve a mostrar panel de equipo
        merchantPanel.SetActive(false);    // Oculta la tienda
        Time.timeScale = 1;                // Reanuda el juego
    }
}