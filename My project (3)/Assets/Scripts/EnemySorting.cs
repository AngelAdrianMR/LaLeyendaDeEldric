using UnityEngine;

public class EnemySorting : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning("No hay SpriteRenderer en " + gameObject.name);
            enabled = false; // Desactiva el script si no hay SpriteRenderer
        }

        if (boxCollider == null)
        {
            Debug.LogWarning("No hay BoxCollider2D en " + gameObject.name);
            enabled = false; // Desactiva el script si no hay BoxCollider2D
        }
    }

    void Update()
    {
        // Obtener la posición de la base del jugador
        float baseY = boxCollider.bounds.min.y;

        // Ajustar el orden de dibujo según la base del jugador
        spriteRenderer.sortingOrder = Mathf.RoundToInt(baseY * -1);
    }
}
