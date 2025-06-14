using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Tilemaps;

public class TreeControl : MonoBehaviour
{
    public int treeHealth = 2; // La vida inicial del árbol
    public GameObject objectToDrop; // El objeto que soltará el árbol cuando muera

    private bool isPlayerInRange = false; // Para comprobar si el jugador está cerca
    private bool isChopping = false; // Verifica si el jugador está talando el árbol

    public InventoryManager inventoryManager;  // Referencia al InventoryManager

    public GameObject infoPanel; // Panel en el Canvas
    public TextMeshProUGUI infoText; // Texto dentro del panel

    public Tilemap tilemap; // Referencia al Tilemap para obtener la posición de los tiles del suelo

    private WorldObject worldObject;

    void Start()
    {
        worldObject = GetComponent<WorldObject>();
    }

    void Update()
    {
        // Verificar si el jugador está en rango para talar el árbol
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (inventoryManager.HasEquipped(ItemType.Axe)) // Verifica si el hacha está equipada
            {
                ChopTree();
            }
            else
            {
                // Si no tiene hacha, muestra un mensaje
                Debug.Log("Necesitas un hacha para talar este árbol.");
            }
        }

        // Si la vida del árbol llega a cero
        if (treeHealth <= 0)
        {
            DropItem(); // Dropear el objeto
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

    // Funciónm para talar
    private void ChopTree()
    {
        if (!isChopping)
        {
            isChopping = true;
            // Restar vida del árbol
            treeHealth--;

            AudioManager.Instance.PlaySound(AudioManager.Instance.breakSound);
        }

        StartCoroutine(ResetChoppingState());
    }

    // Tiempo de espera para el siguiente golpe
    private IEnumerator ResetChoppingState()
    {
        yield return new WaitForSeconds(1f); 
        isChopping = false; // Restablecer el estado de talar
    }


    private void DropItem()
    {
        if (objectToDrop != null)
        {
            // Aseguramos que la posición para el drop esté alineada al Tilemap también
            Vector3 dropPosition = transform.position;
            dropPosition.y += 0.5f; // Ajustar si es necesario para alinearse al nivel del Tilemap
            Instantiate(objectToDrop, dropPosition, Quaternion.identity); // Crear el objeto en la posición del árbol
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si el jugador entra en el área de colisión del árbol, se activa el mensaje para talar
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // Aquí se activa el panel
            ShowInfoMessage(LanguageManager.Instance.GetText("interact_chop_tree"));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Si el jugador sale del área de colisión, se desactiva el panel
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
