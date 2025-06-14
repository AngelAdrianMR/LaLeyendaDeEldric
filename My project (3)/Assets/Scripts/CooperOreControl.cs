using UnityEngine;
using TMPro;

// Gestiona la mena

public class CopperOreControl : MonoBehaviour
{
    public int oreHealth = 2; // Vida de la mena de cobre
    public GameObject objectToDrop; // Objeto que soltará la mena al destruirse

    private bool isPlayerInRange = false; // Verifica si el jugador está cerca
    public InventoryManager inventoryManager;  // Referencia al InventoryManager

    public GameObject infoPanel; // Panel en el Canvas
    public TextMeshProUGUI infoText; // Texto dentro del panel

    private WorldObject worldObject; // Referencia al script WorldObject

    void Start()
    {
        worldObject = GetComponent<WorldObject>();
    }

    void Update()
    {
        // Verifica si el jugador está en rango y presiona "G" para picar
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.G))
        {
            if (inventoryManager.HasEquipped(ItemType.Pickaxe)) // Verifica si el pico está equipado
            {
                MineOre();
            }
            else
            {
                Debug.Log("Necesitas un pico para extraer este mineral.");
            }
        }

        // Si la mena llega a 0 de vida
        if (oreHealth <= 0)
        {
            DropItem(); // Soltar objeto
            if (worldObject != null)
            {
                worldObject.DestroyObject(); // Llama al método del script WorldObject
            }
            else
            {
                Destroy(gameObject); // Si por algún motivo no tiene WorldObject, destrúyelo de forma normal
            }

        }
    }

    // Función para la mena
    private void MineOre()
    {
        oreHealth--; // Reducir vida de la mena
        Debug.Log("Golpeaste la mena de cobre. Vida restante: " + oreHealth);
        AudioManager.Instance.PlaySound(AudioManager.Instance.breakSound);
    }

    // Instanciar objeto al destruirse
    private void DropItem()
    {
        if (objectToDrop != null)
        {
            Instantiate(objectToDrop, transform.position, Quaternion.identity); // Generar objeto en la posición
        }
    }

    // Detectar entrada del jugador (panel informativo)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            ShowInfoMessage(LanguageManager.Instance.GetText("interact_pick_ore"));
        }
    }

    // Detectar salida del jugador
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collider2D playerCollider = other.GetComponent<Collider2D>();
            if (!GetComponent<Collider2D>().IsTouching(playerCollider))
            {
                isPlayerInRange = false;
                HideInfoMessage();
            }
        }
    }

    // Muestra el mensaje en la UI
    private void ShowInfoMessage(string message)
    {
        if (infoPanel != null && infoText != null)
        {
            infoPanel.SetActive(true);
            infoText.text = message;
        }
    }

    // Oculta el panel de información
    private void HideInfoMessage()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }
}
