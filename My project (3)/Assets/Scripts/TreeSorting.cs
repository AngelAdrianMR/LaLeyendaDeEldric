using UnityEngine;

public class TreeSorting : MonoBehaviour
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
            enabled = false;
        }

        if (boxCollider == null)
        {
            Debug.LogWarning("No hay BoxCollider2D en " + gameObject.name);
            enabled = false;
        }
    }

    void Update()
    {
        float baseY = boxCollider.bounds.min.y;
        spriteRenderer.sortingOrder = Mathf.RoundToInt(baseY * -1);
    }
}
