using UnityEngine;

// gestiona las monedas y su recolección

public class Coin : MonoBehaviour
{
    public int coinValue = 1;  // Valor de la moneda, puedes cambiarlo según sea necesario.

    private WorldObject worldObject;

    void Start()
    {
        worldObject = GetComponent<WorldObject>();
    }

    // Método que se ejecuta cuando el jugador entra en contacto con la moneda
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificamos si el objeto que tocó la moneda es el jugador
        if (other.CompareTag("Player"))
        {
            // Obtener el componente PlayerAtribute del jugador
            PlayerAtribute playerAtribute = other.GetComponent<PlayerAtribute>();
            if (playerAtribute != null)
            {
                // Sumar las monedas al jugador
                playerAtribute.AddCoins(coinValue);

                // reproducir el sonido de la moneda 
                AudioManager.Instance.PlaySound(AudioManager.Instance.coinSound);

                // Destruir la moneda
                if (worldObject != null)
                {
                    worldObject.DestroyObject(); // Llama al método del script WorldObject
                }
                else
                {
                    Destroy(gameObject); // Si por algún motivo no tiene WorldObject, destrúyelo de forma normal
                }
            }
        }
    }
}
