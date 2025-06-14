using UnityEngine;
using System.Collections;

public class OrcIABoss : MonoBehaviour
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

    private bool hasHealed = false; // Controla si ya se ha curado una vez
    private int maxHealth; // Guarda la vida máxima original

    public Transform swordObject; // Referencia al objeto hijo que tiene el arma
    private BoxCollider2D swordCollider; // Collider del arma

    private WorldObject worldObject; // Referencia para lista de destruidos
    private PlayerAtribute playerAtribute; // Referencia para accder a los stats del jugador

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        swordCollider = swordObject.GetComponent<BoxCollider2D>();
        worldObject = GetComponent<WorldObject>();
        playerAtribute = GetComponentInParent<PlayerAtribute>(); // Obtener referencia al player

        maxHealth = health;
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

            if (!isAttacking)
            {
                StartCoroutine(AttackPlayer());
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
        SetBlendTreeStates();
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
            // Esquivamos
            direction = GetAvoidanceDirection(direction);
        }
        
        // Aplica movimiento en esta dirección
        rb.linearVelocity = direction * speed;

        UpdateDirection(player.position);
        SetBlendTreeStates();

        // Si no se detiene forzamos que lo haga
        if (rb.linearVelocity.magnitude < 0.01f)
        {
            isRunning = false;
            SetBlendTreeStates();
        }
    }


    // Método para obtener una dirección de esquiva
    Vector2 GetAvoidanceDirection(Vector2 direction)
    {
        // Probar hacia la izquierda
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, new Vector2(direction.y, -direction.x), obstacleDetectionDistance);
        if (hitLeft.collider == null) 
        {
            return new Vector2(direction.y, -direction.x);  // Si no hay obstáculo, esquivar hacia la izquierda
        }

        // Probar hacia la derecha
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, new Vector2(-direction.y, direction.x), obstacleDetectionDistance);
        if (hitRight.collider == null) 
        {
            return new Vector2(-direction.y, direction.x);  // Si no hay obstáculo, esquivar hacia la derecha
        }

        // Si no hay forma de esquivar, sigue al jugador
        return direction;
    }

    //Detectamos la dirección y ajustamos el sprite y collider
    void UpdateDirection(Vector2 targetPosition)
    {
        Vector2 diff = targetPosition - (Vector2)transform.position;
        Vector2 direction = diff.normalized;

        // Enviar dirección al Animator
        anim.SetFloat("DirectionX", direction.x);
        anim.SetFloat("DirectionY", direction.y);

        // Voltear el objeto 
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
        //health -= damage;
        int potentialHealth = health - damage;

        if (!hasHealed && potentialHealth <= maxHealth / 2 && health > maxHealth / 2)
        {
            hasHealed = true;
            health = maxHealth;
            attackDamage += 2;
            Debug.Log("El orco se enfurece, se cura y su daño aumenta 2 puntos: " + attackDamage);
            return; // Interrumpe el daño este golpe
        }

        health = potentialHealth;

        Debug.Log("Enemigo recibe daño: " + damage + " Vida restante: " + health);

        StartCoroutine(ApplyKnockback());

        // Si baja del 50% y aún no ha usado su "resurrección"
        if (!hasHealed && health <= maxHealth / 2)
        {
            hasHealed = true;
            health = maxHealth; // Restaura toda la vida
            attackDamage += 2; // Aumenta su daño
            Debug.Log("El orco se enfurece, se cura y su daño aumenta 2 puntos: " + attackDamage);
            return; // Evita morir en este ataque
        }

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

        // Apuntar a la lista de objetos destruidos y destruir
        if (worldObject != null)
        {
            worldObject.DestroyObject();
        }
        else
        {
            Destroy(gameObject);
        }

    }
}

