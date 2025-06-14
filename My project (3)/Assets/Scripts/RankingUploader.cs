using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

// Este script se encarga de subir la puntuación del jugador al servidor
public class RankingUploader : MonoBehaviour
{
    // Llama a este método desde el botón
    public void EnviarPuntuacion()
    {
        // Obtener nombre del jugador desde PlayerPrefs
        string nombre = PlayerPrefs.GetString("playerName", "Jugador");

        // Obtener puntos de ataque desde PlayerAtribute
        PlayerAtribute player = FindFirstObjectByType<PlayerAtribute>();
        int ataque = player != null ? player.attackPoints : 0;

        StartCoroutine(Enviar(nombre, ataque));
    }

    // Corrutina que realiza la petición POST con el nombre y ataque
    IEnumerator Enviar(string nombre, int ataque)
    {
        // Crear un formulario con los datos a enviar
        WWWForm form = new WWWForm();
        form.AddField("name", nombre);
        form.AddField("attack", ataque);

        // Crear la petición HTTP POST al archivo PHP en el servidor
        UnityWebRequest www = UnityWebRequest.Post("http://rankingeldric.atwebpages.com/subir.php", form);
        www.downloadHandler = new DownloadHandlerBuffer();

        // Establecer el encabezado de tipo navegador para compatibilidad
        www.SetRequestHeader("User-Agent", "Mozilla/5.0 (Unity)");

        // Esperar a que termine la solicitud
        yield return www.SendWebRequest();

        // Verificar si la solicitud fue exitosa
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Respuesta del servidor: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error al enviar puntuación: " + www.error);
        }
    }

}
