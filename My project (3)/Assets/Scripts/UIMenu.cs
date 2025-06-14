using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public GameObject mainPanel;       // Panel principal del menú (pausa)
    public GameObject settingsPanel;   // Panel de configuración u opciones

    private bool isPaused = false;     // Indica si el juego está actualmente en pausa

    void Start()
    {
        // Buscar los paneles por nombre en la jerarquía de la escena
        mainPanel = GameObject.Find("Main");
        settingsPanel = GameObject.Find("Setting");

        if (mainPanel == null)
        {
            Debug.LogError("MainPanel no encontrado.");
        }
        if (settingsPanel == null)
        {
            Debug.LogError("SettingsPanel no encontrado.");
        }

        // Asegurar que ambos paneles estén desactivados al iniciar el juego
        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    void Update()
    {
        // Detecta si se presiona la tecla ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu(); // Abre o cierra el menú de pausa
        }
    }

    // Método para alternar el menú principal de pausa
    public void ToggleMenu()
    {
        if (mainPanel == null) return;

        isPaused = !isPaused; // Cambia el estado de pausa
        mainPanel.SetActive(isPaused); // Activa/desactiva el panel principal

        // Pausa o reanuda el tiempo del juego
        Time.timeScale = isPaused ? 0 : 1;
    }

    // Abre el panel de configuración
    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    // Cierra el panel de configuración
    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }
}
