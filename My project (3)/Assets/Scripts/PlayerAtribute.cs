using UnityEngine;

public class PlayerAtribute : MonoBehaviour
{
    // Variables de salud
    public int maxHealth = 100;  // Salud máxima del jugador
    public int currentHealth;    // Salud actual del jugador

    // Variables de estamina
    public int maxStamina = 100; // Estamina máxima del jugador
    public int currentStamina;   // Estamina actual del jugador

    // Tiempo para la estamina
    private float staminaTimer = 0f; // Temporizador para el consumo de estamina
    private float staminaDrainInterval = 5f; // Intervalo de tiempo para drenar estamina (en segundos)

    // Niveles de habilidades (inicialmente en 1)
    public int attackLevel = 1;  // Nivel de la habilidad de ataque
    public int miningLevel = 1;  // Nivel de la habilidad de minería
    public int choppingLevel = 1; // Nivel de la habilidad de tala
    public int runningLevel = 1; // Nivel de la habilidad de correr
    public int staminaLevel = 1; // Nivel de la habilidad de estamina

    // Contadores de experiencia para cada habilidad
    public int attackXP = 0;  // Experiencia acumulada para la habilidad de ataque
    public int miningXP = 0;  // Experiencia acumulada para la habilidad de minería
    public int choppingXP = 0; // Experiencia acumulada para la habilidad de tala
    public int runningXP = 0;  // Experiencia acumulada para la habilidad de correr
    public int staminaXP = 0;  // Experiencia acumulada para la habilidad de estamina

    // Umbral base de XP para subir de nivel (se incrementa cada 10 niveles)
    public int baseXPThreshold = 15;

    // Recompensas de nivel
    public int attackPoints = 10;   // Puntos de ataque
    public float moveSpeedBonus = 1f; // Multiplicador de velocidad al correr
    public int extraHealth = 0;     // Aumento de salud adicional
    public int extraStamina = 0;    // Aumento de estamina adicional

    // Variable para las monedas
    public int coins;

    // Para el manejo de la muerte
    public Animator animator;
    public GameObject gameOverPanel;

    void Start()
    {
        // Inicializa la salud y la estamina del jugador al máximo al comenzar el juego
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    // Método para añadir monedas
    public void AddCoins(int amount)
    {
        coins += amount;
    }

    // Método para ganar experiencia en una habilidad específica
    public void GainExperience(string skill)
    {
        // Obtener el umbral de XP necesario para subir de nivel según la habilidad
        int xpThreshold = GetXPThreshold(skill);

        // Evaluamos qué habilidad se está mejorando y sumamos XP
        switch (skill)
        {
            case "attack":
                attackXP++;  // Aumentamos la experiencia de ataque
                if (attackXP >= xpThreshold) LevelUp(ref attackLevel, ref attackXP, "attack"); // Subir de nivel
                break;

            case "mining":
                miningXP++;  // Aumentamos la experiencia de minería
                if (miningXP >= xpThreshold) LevelUp(ref miningLevel, ref miningXP, "mining"); // Subir de nivel
                break;

            case "chopping":
                choppingXP++;  // Aumentamos la experiencia de tala
                if (choppingXP >= xpThreshold) LevelUp(ref choppingLevel, ref choppingXP, "chopping"); // Subir de nivel
                break;

            case "running":
                runningXP++;  // Aumentamos la experiencia de correr
                if (runningXP >= xpThreshold) LevelUp(ref runningLevel, ref runningXP, "running"); // Subir de nivel
                break;

        }
    }

    // Calcula el umbral de XP requerido para subir de nivel según el nivel actual
    public int GetXPThreshold(string skill)
    {
        int level = 1; // Valor por defecto para el nivel

        // Obtenemos el nivel actual de la habilidad correspondiente
        switch (skill)
        {
            case "attack": level = attackLevel; break;
            case "mining": level = miningLevel; break;
            case "chopping": level = choppingLevel; break;
            case "running": level = runningLevel; break;
        }

        // El umbral de XP se incrementa en 15 puntos por cada 10 niveles alcanzados
        return baseXPThreshold + ((level / 10) * baseXPThreshold);
    }

    // Método para subir de nivel una habilidad
    private void LevelUp(ref int level, ref int xp, string skill)
    {
        if (level < 50) // El nivel máximo es 50
        {
            level++;  // Aumentamos el nivel de la habilidad
            xp = 0;   // Reiniciamos la experiencia de esa habilidad
            ApplyLevelReward(skill, level); // Aplicamos la recompensa de nivel
            Debug.Log($"{skill} ha subido a nivel {level}"); // Mensaje de debug
        }
    }

    // Método que aplica recompensas al subir de nivel
    private void ApplyLevelReward(string skill, int level)
    {
        switch (skill)
        {
            case "attack":
                attackPoints += 1; // Aumentamos los puntos de ataque
                break;

            case "chopping":
                moveSpeedBonus += 0.1f; // Aumentamos la velocidad de correr
                break;

            case "mining":
                maxStamina += 1; // Aumentamos la estamina máxima
                break;

            case "running":
                maxStamina += 5; // Aumentamos la estamina máxima
                currentStamina = maxStamina; // Restauramos la salud al subir nivel de estamina
                break;
        }
    }

    // Método para consumir estamina al hacer acciones
    public void ConsumeStamina(int amount)
    {
        currentStamina = Mathf.Max(0, currentStamina - amount); // Restamos la estamina consumida, asegurándonos de que no sea negativa
    }

    //Método para consumir estamina mientras corre
    public void DrainStaminaWhileRunning()
    {
        staminaTimer += Time.deltaTime; // Aumentamos el temporizador con el tiempo

        // Si han pasado 5 segundos, gastamos 1 de estamina y reiniciamos el temporizador
        if (staminaTimer >= staminaDrainInterval)
        {
            ConsumeStamina(1); // Gasta 1 de estamina
            GainExperience("running"); //Ganamos experiencia
            staminaTimer = 0f; // Reiniciamos el temporizador
        }
    }

    // Método para recibir daño
    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage); // Restamos el daño recibido

        if (currentHealth == 0) // Si la salud llega a 0
        {
            Debug.Log("Jugador ha muerto."); 
            
            // Activar animación de muerte
            if (animator != null)
            {
                animator.SetBool("IsDead", true);
            }

            // Mostrar panel GameOver
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }        
        }
    }
}
