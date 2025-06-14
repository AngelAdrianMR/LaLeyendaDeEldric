using UnityEngine;

public class WorldObject : MonoBehaviour
{
    public string objectID;
    private GameManager gameManager;
 
    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        // Generar un ID único y depurar
        objectID = gameObject.name.Replace("(Clone)", "").Trim() + transform.position.ToString();
        Debug.Log($"Checking object: {objectID}");

        // COMPARAR CON LISTA DE OBJETOS GUARDADOS
        foreach (var savedID in gameManager.GetDestroyedObjects())
        {
            Debug.Log($"Comparando con guardado: {savedID}");
        }

        if (gameManager.IsObjectDestroyed(objectID))
        {
            Debug.Log($"Objeto debería destruirse: {objectID}");
            Destroy(gameObject);
        }
    }

    // Método para destruir el objeto manualmente y registrar su ID
    public void DestroyObject()
    {
        Debug.Log("Destruyendo objeto: " + objectID);
        gameManager.RegisterDestroyedObject(objectID);
        Destroy(gameObject);
    }
}
