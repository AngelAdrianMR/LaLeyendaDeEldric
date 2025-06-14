using UnityEngine;

public class BushDamage : MonoBehaviour
{
    public int damage = 2; // Daño que inflige el arbusto

    // Detectamos colisión con el jugador
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerAtribute player = collision.GetComponent<PlayerAtribute>();

            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("El arbusto ha hecho daño al jugador (-2)");
            }
        }
    }
}
