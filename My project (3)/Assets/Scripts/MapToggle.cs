using UnityEngine;

public class MapToggle : MonoBehaviour
{
    public GameObject mapPanel;

    void Start()
    {
        if (mapPanel == null)
        {
            mapPanel = GameObject.Find("MapPanel");
        }

        if (mapPanel != null)
        {
            mapPanel.SetActive(false); // Ocultamos al inicio
        }
        else
        {
            Debug.LogError("MapPanel no encontrado.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && mapPanel != null)
        {
            mapPanel.SetActive(!mapPanel.activeSelf); // Toggle
        }
    }
}
