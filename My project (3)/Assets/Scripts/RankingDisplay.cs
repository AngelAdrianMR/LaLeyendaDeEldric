using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class RankingDisplay : MonoBehaviour
{
    public GameObject rankingPanel;              // Panel UI que contiene el ranking
    public GameObject entryPrefab;               // Prefab de texto para cada entrada
    public Transform contentParent;              // Donde se instancian las entradas

    // URL del servidor que devuelve el JSON con el ranking
    private string url = "http://rankingeldric.atwebpages.com/leer.php";

    // Método público para mostrar el ranking y cargar los datos
    public void MostrarRanking()
    {
        rankingPanel.SetActive(true);
        StartCoroutine(CargarRanking());
    }

    // Corrutina que realiza la solicitud HTTP, parsea el JSON y muestra los datos
    IEnumerator CargarRanking()
    {
        // Limpiar entradas anteriores
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Envia la petición GET a la URL del servidor
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al obtener el ranking: " + www.error);
                yield break;
            }

            // Parsear JSON
            string json = www.downloadHandler.text;

            // Asegúrate de que el JSON tiene el formato { "items": [...] }
            RankingList rankingList = JsonUtility.FromJson<RankingList>(json);

            // Validar si el objeto fue correctamente deserializado
            if (rankingList == null || rankingList.items == null)
            {
                Debug.LogError("Error al parsear el ranking.");
                yield break;
            }

            // Mostrar cada entrada con su posición
            int posicion = 1;
            foreach (RankingEntry entrada in rankingList.items)
            {
                GameObject nuevaEntrada = Instantiate(entryPrefab, contentParent);
                nuevaEntrada.GetComponent<TextMeshProUGUI>().text = $"{posicion}. {entrada.name} - {entrada.attack} pts";
                posicion++;
            }
        }
    }

    // Oculta el panel de ranking
    public void OcultarRanking()
    {
        rankingPanel.SetActive(false);
    }
}
