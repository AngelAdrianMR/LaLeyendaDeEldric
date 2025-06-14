using UnityEngine;

// Clase que representa una flecha en el juego
public class Arrow : MonoBehaviour
{
    // Cantidad de daño que inflige la flecha
    public int damage = 4;

    // Tiempo que la flecha permanece activa antes de destruirse automáticamente
    public float lifetime = 5f;

    void Start()
    {
        // Destruye el objeto (flecha) después de un tiempo determinado
        Destroy(gameObject, lifetime);
    }

    // Se ejecuta cuando la flecha entra en contacto con otro collider
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto con el que colisiona tiene la etiqueta "Player"
        if (collision.CompareTag("Player"))
        {
            // Intenta obtener el componente PlayerAtribute del jugador
            PlayerAtribute player = collision.GetComponent<PlayerAtribute>();
            
            // Si se encuentra el componente, aplica el daño
            if (player != null)
            {
                player.TakeDamage(damage); // Aplica daño al jugador
                Debug.Log("Flecha golpeó al jugador"); // Mensaje de depuración
            }

            // Destruye la flecha tras el impacto
            Destroy(gameObject);
        }
    }
}
