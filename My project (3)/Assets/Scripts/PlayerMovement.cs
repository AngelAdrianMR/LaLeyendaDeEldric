using UnityEngine;

// Este script controla el movimiento, animaciones y acciones del jugador

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;          // Referencia al Animator
    public Rigidbody2D rb;             // Referencia al Rigidbody2D
    public float moveSpeed = 5f;       // Velocidad de movimiento
    public float runSpeedMultiplier = 1.5f; // Multiplicador de velocidad para correr

    // Referencia a los atributos del jugador
    private PlayerAtribute playerStats;
    private InventoryManager inventoryManager;  // Referencia al InventoryManager

    // Variables para movimiento
    private Vector2 movement;
    private bool isWalking;
    private bool isRunning;

    // Variables para las acciones
    private bool isAttacking;
    private bool isMining;
    private bool isChopping;
    private bool isPicking;
    private bool hability01;

    // Variable para la última dirección
    private Vector2 lastMovementDirection = Vector2.right; // Valor inicial (mirando a la derecha)

    // Tiempo para la animación
    private float actionDuration;      // Duración de la animación
    private float actionTimer;         // Temporizador para la animación

    public ItemPickup nearbyItem; // Objeto cercano para recoger


    void Start()
    {
        // Buscar el componente PlayerStats en el mismo objeto
        playerStats = GetComponent<PlayerAtribute>();

        if (playerStats == null)
        {
            Debug.LogError("No se encontró el script PlayerStats en el jugador.");
        }

        // Buscar automáticamente el GameManager y obtener el InventoryManager
        inventoryManager = FindFirstObjectByType<InventoryManager>();

        if (inventoryManager == null)
        {
            Debug.LogError("No se encontró InventoryManager en el GameManager.");
        }

    }

    void Update()
    {
        // Obtener las entradas de movimiento
        float horizontal = Input.GetAxisRaw("Horizontal");  // A/D o flechas
        float vertical = Input.GetAxisRaw("Vertical");      // W/S o flechas

        // Movimiento
        movement = new Vector2(horizontal, vertical).normalized;

        isWalking = movement.magnitude > 0; // Si hay movimiento, es true

        // Comprobar si se está corriendo
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // Si el jugador se mueve, actualizar la última dirección de movimiento
        if (isWalking)
        {
            lastMovementDirection = movement; // Guardamos la última dirección
        }

        // Realizar las acciones
        HandleActions();
        MovePlayer();

        // Intentar recoger objeto si se presiona "E"
        if (Input.GetKeyDown(KeyCode.E) && nearbyItem != null)
        {
            PickUpItem();
        }

        animator.SetBool("IsDead", false);
        
    }

    void MovePlayer()
    {
        // Ajustar la velocidad según si el jugador está corriendo
        float currentMoveSpeed = isRunning ? moveSpeed * runSpeedMultiplier : moveSpeed;

        // Aplicar movimiento usando Rigidbody2D
        rb.linearVelocity = movement * currentMoveSpeed;

        // Si el jugador deja de moverse, usar la última dirección registrada en el Animator
        if (isWalking)
        {
            
            animator.SetFloat("DirectionX", movement.x);
            animator.SetFloat("DirectionY", movement.y);
            
        }
        else
        {
            animator.SetFloat("DirectionX", lastMovementDirection.x);
            animator.SetFloat("DirectionY", lastMovementDirection.y);
        }

        // Actulizar animación
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsRunning", isRunning);

        // Voltear el sprite horizontalmente si es necesario
        if (lastMovementDirection.x < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (lastMovementDirection.x > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        // Reducir estamina al correr
        if (isRunning)
        {
            playerStats.DrainStaminaWhileRunning(); // Gasta 1 de estamina cada 5 segundos
        }
    }


    void HandleActions()
    {

        // Si no se está realizando una acción, verificar teclas de acción
        if (actionTimer >= actionDuration)
        {
            // Activar animación de ataque con la tecla "Space"
            if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
            {
                isAttacking = true;
                animator.SetBool("IsAttacking", true);
                actionDuration = animator.GetCurrentAnimatorStateInfo(0).length; // Duración de la animación
                actionTimer = 0f; // Reiniciar temporizador
                playerStats.GainExperience("attack"); // Gana XP de ataque
            }

            // Picar con la tecla "G"
            if (Input.GetKeyDown(KeyCode.G) && !isMining && playerStats.currentStamina >= 3)
            {
                if (inventoryManager.HasEquipped(ItemType.Pickaxe))
                {
                    isMining = true;
                    animator.SetBool("IsMining", true);
                    actionDuration = animator.GetCurrentAnimatorStateInfo(0).length; // Duración de la animación
                    actionTimer = 0f; // Reiniciar temporizador
                    playerStats.ConsumeStamina(3); // Gasta 3 de estamina
                    playerStats.GainExperience("mining"); // Gana XP de minería
                }
                else
                {
                    Debug.Log("Necesitas un pico para picar esta mena.");
                }
            }

            // Talar con la tecla "F"
            if (Input.GetKeyDown(KeyCode.F) && !isChopping && playerStats.currentStamina >= 3)
            {
                if (inventoryManager.HasEquipped(ItemType.Axe))
                {   
                    isChopping = true;
                    animator.SetBool("IsChopping", true);
                    actionDuration = animator.GetCurrentAnimatorStateInfo(0).length; // Duración de la animación
                    actionTimer = 0f; // Reiniciar temporizador
                    playerStats.ConsumeStamina(3); // Gasta 3 de estamina
                    playerStats.GainExperience("chopping"); // Gana XP de leñador
                }
                else
                {
                    Debug.Log("Necesitas un hacha para talar este árbol.");
                }
            }

            // Recoger objetos con la tecla "E"
            if (Input.GetKeyDown(KeyCode.E) && !isPicking)
            {
                isPicking = true;
                //animator.SetBool("IsPicking", true);
                actionDuration = animator.GetCurrentAnimatorStateInfo(0).length; // Duración de la animación
                actionTimer = 0f; // Reiniciar temporizador
                playerStats.GainExperience("stamina"); // Gana XP en resistencia
            }

            // Habilidad clavar espada en el suelo
            if(Input.GetKeyDown(KeyCode.Alpha1) && !isAttacking && playerStats.currentStamina >= 10)
            {
                hability01 = true;
                animator.SetBool("IsHability01", true);
                actionDuration = animator.GetCurrentAnimatorStateInfo(0).length;
                actionTimer = 0f;
                playerStats.ConsumeStamina(10);
                playerStats.attackPoints = 30;
            }
        }
    

        // Control del temporizador para saber cuándo terminar la animación
        if (actionTimer < actionDuration)
        {
            actionTimer += Time.deltaTime; // Incrementamos el temporizador
        }
        else
        {
            // Cuando el temporizador termine, volvemos a Idle
            if (isAttacking)
            {
                isAttacking = false;
                animator.SetBool("IsAttacking", false);
            }

            if (isMining)
            {
                isMining = false;
                animator.SetBool("IsMining", false);
            }

            if (isChopping)
            {
                isChopping = false;
                animator.SetBool("IsChopping", false);
            }

            if (isPicking)
            {
                isPicking = false;
                animator.SetBool("IsPicking", false);
            }

            if(hability01)
            {
                hability01 = false;
                animator.SetBool("IsHability01", false);
                playerStats.attackPoints = 5;
            }


        }
    }

    // Recoger el objeto
    public void PickUpItem()
    {
        nearbyItem.PickupItem();
        nearbyItem = null; // Limpiamos la referencia
    }

    // Detectar objetos cercanos
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            nearbyItem = other.GetComponent<ItemPickup>();
        }
    }

    // Detectar que nos alejamos de un item
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            nearbyItem = null;
        }
    }
}
