using UnityEngine;

public class PlayerSorting : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D circleCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CapsuleCollider2D>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning("No hay SpriteRenderer en " + gameObject.name);
            enabled = false; // Desactiva el script si no hay SpriteRenderer
        }

        if (circleCollider == null)
        {
            Debug.LogWarning("No hay BoxCollider2D en " + gameObject.name);
            enabled = false; // Desactiva el script si no hay BoxCollider2D
        }
    }

    void Update()
    {
        // Obtener la posición de la base del jugador
        float baseY = circleCollider.bounds.min.y;

        // Ajustar el orden de dibujo según la base del jugador
        spriteRenderer.sortingOrder = Mathf.RoundToInt(baseY * -1);
    }
}
