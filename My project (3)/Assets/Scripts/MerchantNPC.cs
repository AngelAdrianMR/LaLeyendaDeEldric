using UnityEngine;
using TMPro;

public class MerchantNPC : MonoBehaviour
{
    public GameObject tooltipPanel;         // Panel de mensaje contextual ("Pulsa E para comerciar")
    public TextMeshProUGUI tooltipText;     // Texto del mensaje contextual
    public GameObject merchantUI;           // Panel de la tienda (interfaz de comercio)
    public GameObject inventory;            // Panel de inventario del jugador
    public GameObject equipPanel;           // Panel de equipo del jugador

    private bool isPlayerInRange;           // Indica si el jugador está cerca del comerciante
    private bool isPanelOpen = false;       // Estado del panel de comercio

    void Start()
    {
        // Al iniciar, todo está oculto
        tooltipPanel.SetActive(false);
        merchantUI.SetActive(false);
    }

    void Update()
    {
        // Si el jugador está cerca y presiona "E"
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (isPanelOpen)
                CloseMerchantPanel(); // Cierra la tienda si ya está abierta
            else
                OpenMerchantPanel();  // Abre la tienda
        }
    }

    // Abre el panel de comercio
    void OpenMerchantPanel()
    {
        inventory.SetActive(true); // Muestra inventario del jugador
        merchantUI.SetActive(true); // Muestra la tienda
        equipPanel.SetActive(false); // Oculta panel de equipo para no saturar
        tooltipPanel.SetActive(false); // Oculta el mensaje contextual
        Time.timeScale = 0f; // Pausa el juego mientras comercia
        isPanelOpen = true;
    }

    // Cierra el panel de comercio
    void CloseMerchantPanel()
    {
        inventory.SetActive(false); // Oculta inventario
        merchantUI.SetActive(false); // Oculta tienda
        equipPanel.SetActive(true); // Vuelve a mostrar el equipo
        tooltipPanel.SetActive(true); // Muestra de nuevo el mensaje contextual
        Time.timeScale = 1f; // Reanuda el juego
        isPanelOpen = false;
    }

    // Detecta si el jugador entra en el rango del comerciante
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            tooltipPanel.SetActive(true); // Muestra mensaje de interacción

            // Traducción dinámica del texto desde el LanguageManager
            tooltipText.text = LanguageManager.Instance.GetText("interact_merchant_npc");
        }
    }

    // Detecta si el jugador sale del rango del comerciante
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            tooltipPanel.SetActive(false);

            // Si el panel está abierto y el jugador se aleja, se cierra automáticamente
            if (isPanelOpen)
            {
                CloseMerchantPanel();
            }
        }
    }
}