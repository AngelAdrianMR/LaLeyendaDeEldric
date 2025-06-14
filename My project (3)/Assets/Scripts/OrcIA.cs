using UnityEngine;
using System.Collections;

public class OrcIA : MonoBehaviour
{
    public Transform player;  // Referencia al jugador
    public float detectionRange = 10f;  // Rango de detección para el jugador
    public float speed = 3f;  // Velocidad de movimiento del enemigo
    public float obstacleDetectionDistance = 2f;  // Distancia para detectar obstáculos
    public float knockbackForce = 3f;  // Fuerza de retroceso
    public int health = 20; // Vida del orco
    public int attackDamage = 4; // Daño que causa el orco
    public GameObject dropItem; // Item que soltará al morir

    private Animator anim; // Controlador de animaciones
    private SpriteRenderer spriteRenderer; // Sprite (Para el volteo)
    private Rigidbody2D rb; // Físicas
    private bool isKnockedBack = false; // Variable para saber si esta siendo empujado

    public float attackRange = 1.2f; // Distancia mínima para atacar
    // Estados
    private bool isAttacking = false;
    private bool isRunning = false;

    public Transform swordObject; // Referencia al objeto hijo que tiene el arma
    private BoxCollider2D swordCollider; // Collider del arma

    private WorldObject worldObject; // Referencia para lista de destruidos
    private PlayerAtribute playerAtribute; // Referencia para accder a los stats del jugador

    // Tiempo entre ataque
    private float lastAttackTime = 0f;
    public float attackCooldown = 1f;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        swordCollider = swordObject.GetComponent<BoxCollider2D>();
        worldObject = GetComponent<WorldObject>();
        playerAtribute = GetComponentInParent<PlayerAtribute>(); // Obtener referencia al player
    }


    void Update()
    {
        // No hace nada si no hay jugador o esta siendo empujado
        if (player == null || isKnockedBack) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Si está en rango de ataque se detiene y ataca
        if (distance <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            isRunning = false;
            UpdateDirection(player.position);
            SetBlendTreeStates();

            // Ataca solo si pasa el tiempo de coldown
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(AttackPlayer());
                lastAttackTime = Time.time;
            }
        }
        // Si esta en el rango lo persigue
        else if (distance < detectionRange)
        {
            isRunning = true;
            MoveTowardsPlayer();
        }
        //Si esta lejos vuelve a idle
        else
        {
            isRunning = false;
            rb.linearVelocity = Vector2.zero;
            isAttacking = false;
            UpdateDirection(player.position);
            SetBlendTreeStates();
        }
    }

    // Corutina para atacar
    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        SetBlendTreeStates();

        // Detener el movimiento y mirar al jugador
        rb.linearVelocity = Vector2.zero;
        UpdateDirection(player.position);
        
        yield return new WaitForSeconds(0.4f);

        isAttacking = false;

    }

    // Movimiento hacia el jugador con esquiva
    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        // Detectamos si hay un obstáculo delante
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, obstacleDetectionDistance);

        if (hit.collider != null)
        {
            Debug.DrawRay(transform.position, direction * obstacleDetectionDistance, Color.red);
            // Esquivar
            direction = GetAvoidanceDirection(direction);
        }
        
        // Aplica movimiento en esta dirección
        rb.linearVelocity = direction * speed;

        UpdateDirection(player.position);
        SetBlendTreeStates();
    }


    // Método para obtener una dirección de esquiva
    Vector2 GetAvoidanceDirection(Vector2 direction)
    {
        // Probar hacia la izquierda
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, new Vector2(direction.y, -direction.x), obstacleDetectionDistance);
        // Si no hay obstáculo, esquivar hacia la izquierda
        if (hitLeft.collider == null)
        {
            return new Vector2(direction.y, -direction.x);
        } 

        // Probar hacia la derecha
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, new Vector2(-direction.y, direction.x), obstacleDetectionDistance);
        // Si no hay obstáculo, esquivar hacia la derecha
        if (hitRight.collider == null)
        {
            return new Vector2(-direction.y, direction.x);
        }

        // Si no hay forma de esquivar, sigue al jugador
        return direction;
    }

    //Detectamos la dirección y ajustamos el sprite
    void UpdateDirection(Vector2 targetPosition)
    {
        Vector2 diff = targetPosition - (Vector2)transform.position;
        Vector2 direction = diff.normalized;

        // Enviar dirección al Animator
        anim.SetFloat("DirectionX", direction.x);
        anim.SetFloat("DirectionY", direction.y);

        // Voltear objeto 
        transform.localScale = new Vector3(direction.x < 0 ? -1f : 1f, 1f, 1f);
    }

    // Detectar si lo golpea el jugador
    private void OnTriggerEnter2D(Collider2D collision)
    {
            if (collision.CompareTag("ToolCollider")) // arma del jugador
            {
                PlayerAtribute player = collision.GetComponentInParent<PlayerAtribute>();
                if (player != null)
                {
                    TakeDamage(player.attackPoints);
                }
            }
    }

    // Aplicar retroceso al recibir un golpe
    IEnumerator ApplyKnockback()
    {
        isKnockedBack = true;
        Vector2 knockbackDirection = (transform.position - player.position).normalized;
        rb.AddForce (knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.2f);  // Duración del knockback

        isKnockedBack = false;
    }

    // Aplicar daño y muerte
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemigo recibe daño: " + damage + " Vida restante: " + health);

        StartCoroutine(ApplyKnockback());

        if (health <= 0)
        {
            Die();
        }
    }

    // Enviar parámetros al Animator
    void SetBlendTreeStates()
    {
        anim.SetBool("isRun", isRunning);
        anim.SetBool("isAttack", isAttacking);
    }

    // Lógica de muerte
    void Die()
    {
        Debug.Log("Enemigo derrotado!");

        // Suelta el ítem
        if (dropItem != null)
        {
            Instantiate(dropItem, transform.position, Quaternion.identity);
        }

        // "Apuntar a la lista" de objetos destruidos y destruir
        if (worldObject != null)
        {
            worldObject.DestroyObject();
        }
        else
        {
            Destroy(gameObject);
        }

        // En Die()
        if (MissionManager.Instance != null)
        {
            // Buscar la misión que quieres actualizar
            Mission mision = MissionManager.Instance.GetMissionById("m2");

            if (mision != null && mision.isActive && !mision.isCompleted)
            {
                // Sumar 1 al progreso de la mision
                int orcosMuertos = PlayerPrefs.GetInt("orcos_muertos", 0);
                orcosMuertos++;
                PlayerPrefs.SetInt("orcos_muertos", orcosMuertos);

                Debug.Log("Orcos derrotados: " + orcosMuertos);

                if (orcosMuertos >= 3)
                {
                    MissionManager.Instance.CompleteMission("m2");
                    Debug.Log("¡Misión completada!");
                }
            }
        }


    }
}

