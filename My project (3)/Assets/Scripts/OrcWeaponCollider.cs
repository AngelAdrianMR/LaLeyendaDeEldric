using UnityEngine;

// Script que se coloca en el collider del arma del orco
public class OrcWeaponCollider : MonoBehaviour
{
    // Daño que inflige al jugador al golpear
    public int damage = 4;

    // Se activa cuando este collider entra en contacto con otro collider marcado como "Trigger"
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerAtribute player = collision.GetComponent<PlayerAtribute>();
            
            // Si el jugador tiene el componente, le aplicamos daño
            if (player != null)
            {
                player.TakeDamage(damage); //Lama al método para restar vida
                Debug.Log("Jugador golpeado por espada del orco: -" + damage);
            }
        }
    }
}
