using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// Controlador del menú principal del juego
public class MenuController : MonoBehaviour
{
    // Iniciar una nueva partida
    public void StartNewGame()
    {
        PlayerPrefs.SetInt("LoadGame", 0); // Marca que se trata de una partida nueva
        SceneManager.LoadScene("NewGame"); // Carga la escena inicial para nueva partida
    }

    // Cargar partida guardada
    public void LoadGame()
    {
        PlayerPrefs.SetInt("LoadGame", 1); // Marca que debe cargarse una partida existente
        StartCoroutine(LoadSceneAsync("SampleScene")); // Carga la escena donde se continúa la partida
    }

    // Carga una escena de forma asíncrona
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Espera hasta que termine de cargar la escena
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // Método para cerrar el juego
    public void ExitGame()
    {
        Application.Quit();
    }
}
