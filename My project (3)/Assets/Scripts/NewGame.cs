using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class NewGame : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Button botonJugar;
    public static string playerName;

    void Start()
    {
        nameInput.onValueChanged.AddListener(delegate {
            nameInput.textComponent.color = Color.black;
            botonJugar.interactable = false;
        });

        botonJugar.interactable = false;
    }

    public void SavePlayerName()
    {
        PlayerPrefs.SetString("playerName", nameInput.text);
    }

    public void Game()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Ya no requiere parámetro, toma el texto directo
    public void VerificarNombre()
    {
        string nombre = nameInput.text.Trim(); // elimina espacios vacíos

        // Si el campo está vacío, no envíes nada
        if (string.IsNullOrEmpty(nombre))
        {
            nameInput.textComponent.color = Color.red;
            botonJugar.interactable = false;
            Debug.LogWarning("Nombre vacío");
            return;
        }

        StartCoroutine(VerificarNombreEnServidor(nombre));
    }

    IEnumerator VerificarNombreEnServidor(string nombre)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", nombre);

        UnityWebRequest www = UnityWebRequest.Post("http://rankingeldric.atwebpages.com/verificar_nombre.php", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string respuesta = www.downloadHandler.text;
            Debug.Log("Respuesta: " + respuesta);

            if (respuesta.Contains("exists"))
            {
                nameInput.textComponent.color = Color.red;
                botonJugar.interactable = false;
            }
            else if (respuesta.Contains("available"))
            {
                nameInput.textComponent.color = Color.green;
                botonJugar.interactable = true;
            }
            else
            {
                nameInput.textComponent.color = Color.red;
                botonJugar.interactable = false;
                Debug.LogWarning("Respuesta inesperada");
            }
        }
        else
        {
            Debug.LogError("Error al verificar nombre: " + www.error);
            nameInput.textComponent.color = Color.red;
            botonJugar.interactable = false;
        }
    }
}
