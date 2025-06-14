using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public PlayerAtribute playerAtribute;       // Referencia al script PlayerAtribute
    public PlayerMovement playerMovement;       // Referencia al script PlayerMovement

    // Referencias a los elementos de la UI
    // Barras del Panel Play (siempre visible)
    public Image healthBarPlay;
    public Image staminaBarPlay;

    // Barras y textos en el Panel Stats (se muestra con "C")
    public Image healthBarStats;
    public Image staminaBarStats;
    public Text healthTextStats;
    public Text staminaTextStats;

    // Textos de niveles de habilidades
    public Text attackLevelText;
    public Text runningLevelText;
    public Text miningLevelText;
    public Text choppingLevelText;

    // Nuevos textos agregados (puntos de ataque, velocidad y multiplicador de velocidad)
    public Text attackPointsText;  // Puntos de ataque
    public Text moveSpeedText;    // Velocidad de movimiento
    public Text speedMultiplierText;  // Multiplicador de velocidad

    // Referencia a monedas
    public Text coinsText;

    // Panel de estadísticas (se muestra cuando se presiona "C")
    public GameObject statsPanel;
    private CanvasGroup statsCanvasGroup;

    // Imágenes para mostrar el progreso de la experiencia
    public Image attackXPProgressImage;
    public Image runningXPProgressImage;
    public Image miningXPProgressImage;
    public Image choppingXPProgressImage;

    // Claves para traducción
    private const string KEY_ATTACK = "stats_attack";
    private const string KEY_RUNNING = "stats_running";
    private const string KEY_MINING = "stats_mining";
    private const string KEY_CHOPPING = "stats_chopping";
    private const string KEY_ATTACK_POINTS = "stats_attack_points";
    private const string KEY_SPEED = "stats_speed";
    private const string KEY_SPEED_MULTIPLIER = "stats_speed_multiplier";


    void Start()
    {
        // Inicializamos la referencia al CanvasGroup para controlar la visibilidad del panel de stats
        statsCanvasGroup = statsPanel.GetComponent<CanvasGroup>();
        if (statsCanvasGroup == null)
        {
            statsCanvasGroup = statsPanel.AddComponent<CanvasGroup>();  // Si no tiene CanvasGroup, lo añadimos
        }

        // Inicializamos el panel de stats como invisible al comenzar
        statsCanvasGroup.alpha = 0f;
        statsCanvasGroup.interactable = false;
        statsCanvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        // Detectamos si el jugador presiona la tecla "C"
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Alternamos la visibilidad del panel de stats
            ToggleStatsPanel();
        }

        // Actualiza las barras de vida y estamina
        UpdateHealthBar();
        UpdateStaminaBar();

        // Actualiza los textos de vida y estamina en el panel Stats
        UpdateHealthText();
        UpdateStaminaText();

        // Actualiza los niveles de habilidades
        attackLevelText.text = LanguageManager.Instance.GetText(KEY_ATTACK) + ": " + playerAtribute.attackLevel;
        runningLevelText.text = LanguageManager.Instance.GetText(KEY_RUNNING) + ": " + playerAtribute.runningLevel;
        miningLevelText.text = LanguageManager.Instance.GetText(KEY_MINING) + ": " + playerAtribute.miningLevel;
        choppingLevelText.text = LanguageManager.Instance.GetText(KEY_CHOPPING) + ": " + playerAtribute.choppingLevel;
        
        // Actualiza los textos de los nuevos atributos
        attackPointsText.text = LanguageManager.Instance.GetText(KEY_ATTACK_POINTS) + ": " + playerAtribute.attackPoints;

        // Actualiza la velocidad de movimiento y el multiplicador de velocidad
        moveSpeedText.text = LanguageManager.Instance.GetText(KEY_SPEED) + ": " + playerMovement.moveSpeed;
        speedMultiplierText.text = LanguageManager.Instance.GetText(KEY_SPEED_MULTIPLIER) + ": " + playerMovement.runSpeedMultiplier;

        // Actualiza las monedas
        coinsText.text = " " + playerAtribute.coins;

        // Actualiza las imágenes de progreso de experiencia
        UpdateXPProgressImages();
    }

    // Actualiza las barras de vida en ambos paneles
    private void UpdateHealthBar()
    {
        float healthPercentage = (float)playerAtribute.currentHealth / playerAtribute.maxHealth;

        // Actualiza la barra de vida del panel Play
        healthBarPlay.fillAmount = healthPercentage;

        // Actualiza la barra de vida del panel Stats
        healthBarStats.fillAmount = healthPercentage;
    }

    // Actualiza las barras de estamina en ambos paneles
    private void UpdateStaminaBar()
    {
        float staminaPercentage = (float)playerAtribute.currentStamina / playerAtribute.maxStamina;

        // Actualiza la barra de estamina del panel Play
        staminaBarPlay.fillAmount = staminaPercentage;

        // Actualiza la barra de estamina del panel Stats
        staminaBarStats.fillAmount = staminaPercentage;
    }

    // Actualiza los textos de vida en el panel Stats
    private void UpdateHealthText()
    {
        healthTextStats.text = $"{playerAtribute.currentHealth} / {playerAtribute.maxHealth}";
    }

    // Actualiza los textos de estamina en el panel Stats
    private void UpdateStaminaText()
    {
        staminaTextStats.text = $"{playerAtribute.currentStamina} / {playerAtribute.maxStamina}";
    }

    // Actualiza las imágenes de experiencia de habilidades
    private void UpdateXPProgressImages()
    {
        // Calcula el porcentaje de progreso para cada habilidad
        float attackProgress = (float)playerAtribute.attackXP / playerAtribute.GetXPThreshold("attack");
        float runningProgress = (float)playerAtribute.runningXP / playerAtribute.GetXPThreshold("running");
        float miningProgress = (float)playerAtribute.miningXP / playerAtribute.GetXPThreshold("mining");
        float choppingProgress = (float)playerAtribute.choppingXP / playerAtribute.GetXPThreshold("chopping");

        // Actualiza las imágenes de progreso con los valores calculados
        attackXPProgressImage.fillAmount = Mathf.Clamp01(attackProgress);
        runningXPProgressImage.fillAmount = Mathf.Clamp01(runningProgress);
        miningXPProgressImage.fillAmount = Mathf.Clamp01(miningProgress);
        choppingXPProgressImage.fillAmount = Mathf.Clamp01(choppingProgress);
    }

    // Alterna la visibilidad del panel de stats
    private void ToggleStatsPanel()
    {
        // Si el panel está visible, lo ocultamos, y viceversa
        if (statsCanvasGroup.alpha == 0f)
        {
            statsCanvasGroup.alpha = 1f;  // Hacemos visible el panel
            statsCanvasGroup.interactable = true;  // Permite la interacción con los elementos del panel
            statsCanvasGroup.blocksRaycasts = true;  // Permite que los eventos de raycast (clicks) interactúen con el panel

            // Pausamos el juego cuando el panel está visible
            Time.timeScale = 0f;  // Detiene el juego
        }
        else
        {
            statsCanvasGroup.alpha = 0f;  // Hacemos invisible el panel
            statsCanvasGroup.interactable = false;  // Desactiva la interacción con el panel
            statsCanvasGroup.blocksRaycasts = false;  // Desactiva la interacción con los eventos de raycast

            // Reanudamos el juego cuando el panel está oculto
            Time.timeScale = 1f;  // Reanuda el juego
        }
    }
}
