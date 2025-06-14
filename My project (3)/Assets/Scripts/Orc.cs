using UnityEngine;
using System.Collections;

public class Orc : MonoBehaviour
{
    public Transform pointA; // Punto A
    public Transform pointB; // Punto B
    public float speed = 2f; // Velocidad de movimiento
    public int health = 20; // Vida del enemigo
    public int attackDamage = 4; // Daño al jugador
    public GameObject dropItem; // Objeto que dropea al morir
    public float knockbackForce = 3f; // Fuerza de retroceso cuando recibe daño

    private Transform targetPoint; // El punto actual al que se dirige
    private Rigidbody2D rb; // Referencia al Rigidbody2D para movimiento
    Animator animator; // Controlador de animaciones
    private SpriteRenderer sprite; // Para voltear el sprite según dirección
    private bool isWaiting = false; // Indica si está esperando al llegar a un punto
    private bool isKnockedBack = false; // Indica si está recibiendo retroceso

    private WorldObject worldObject; // Referencia para lista de destrucción

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        targetPoint = pointB; // Inicialmente el enemigo va hacia el punto B
        animator.SetBool("IsRunning", true);

        worldObject = GetComponent<WorldObject>();
    }

    void Update()
    {
        // Si no está esperando ni en retroceso, se mueve entre puntos
        if (!isWaiting && !isKnockedBack)
        {
            MoveBetweenPoints();
        }
    }

    // Corrección de movimiento en Y
    void FixedUpdate()
    {
        // Elimina el movimiento vertical para mantenerlo en una sola línea
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
    }

    // Lógica de movimientos entre puntos
    void MoveBetweenPoints()
    {
        // Moverse hacia el punto objetivo (A o B)
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        rb.linearVelocity = direction * speed; // Corrección aquí

        // voltear sprite
        if (direction.x > 0)
        {
            sprite.flipX = false;
        }
        else if (direction.x < 0)
        {
            sprite.flipX = true;
        }

        // Si llega al punto objetivo,esperar 3 segundos y cambiar al siguiente punto
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.5f)
        {
            StartCoroutine(WaitBeforeMoving());
        }
    }

    // Corutina para esperar antes de cambiar de dirección
    IEnumerator WaitBeforeMoving()
    {
        isWaiting = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("IsRunning", false);

        yield return new WaitForSeconds(2f); // Timepo de espera

        // Cambiar al otro punto
        targetPoint = (targetPoint == pointA) ? pointB : pointA;
        animator.SetBool("IsRunning", true);
        isWaiting = false;
    }

    // Detección de colisiones con armas o jugador
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si lo golpea el arma
        if (collision.gameObject.CompareTag("ToolCollider"))
        {
            BoxCollider2D weaponCollider = collision.GetComponent<BoxCollider2D>();
            
            if (weaponCollider != null && weaponCollider.enabled) // Solo si el arma está activa
            {
                PlayerAtribute player = collision.GetComponentInParent<PlayerAtribute>(); // Obtener referencia al player
                if (player != null)
                {
                    TakeDamage(player.attackPoints);
                }
            }
        }

        // Si colisiona directamente con el jugador
        if (collision.CompareTag("Player"))
        {
            PlayerAtribute playerAttributes = collision.GetComponent<PlayerAtribute>();
            
            if (playerAttributes != null)
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.playerHitSound);
                playerAttributes.TakeDamage(playerAttributes.attackPoints);
                Debug.Log("El jugador ha recibido daño: " + playerAttributes.attackPoints);
            }
        }
    }

    // Aplicar daño al enemigo
    public void TakeDamage(int damage)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.enemyHitSound);

        health -= damage;
        Debug.Log("Enemigo recibe daño: " + damage + " Vida restante: " + health);

        // Aplicar knockback
        StartCoroutine(ApplyKnockback());

        if (health <= 0)
        {
            Die(); // Aplicamos este metodo si la vida llega a 0
        }
    }

    // Corutina para el efecto de retroceso al recibir un golpe
    IEnumerator ApplyKnockback()
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;

        // Retrocede en dirección opuesta al punto objetivo
        Vector2 knockbackDirection = (transform.position - targetPoint.position).normalized;
        rb.linearVelocity = knockbackDirection * knockbackForce;

        yield return new WaitForSeconds(0.2f);

        isKnockedBack = false;
    }

    // Lógica para la muerte
    void Die()
    {
        Debug.Log("Enemigo derrotado!");
        animator.SetBool("IsDead", true);
        rb.linearVelocity = Vector2.zero;
        this.enabled = false; // Desactiva este script

        // Si hay item lo soltamos
        if (dropItem != null)
        {
            Instantiate(dropItem, transform.position, Quaternion.identity);
        }
        
        // Apuntamos a la lista de objetos destruidos
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