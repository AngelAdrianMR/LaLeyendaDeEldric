using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Controlador de la UI de misiones
public class MissionUI : MonoBehaviour
{
    public GameObject missionPanel;             // Panel principal de misiones
    public GameObject missionEntryPrefab;       // Prefab de una entrada (botón) de misión
    public Transform missionListActiveContent;  // Contenedor de misiones activas
    public Transform missionListCompletedContent; // Contenedor de misiones completadas

    public TextMeshProUGUI titleText;           // Texto del título al mostrar detalles
    public TextMeshProUGUI descriptionText;     // Texto de la descripción de la misión

    private void Start()
    {
        missionPanel.SetActive(false); // Oculta el panel al iniciar
    }

    private void Update()
    {
        // Presionar la tecla J abre/cierra el panel
        if (Input.GetKeyDown(KeyCode.J))
        {
            bool isActive = missionPanel.activeSelf;
            missionPanel.SetActive(!isActive);

            // Si se abre, se actualiza el contenido
            if (!isActive)
            {
                RefreshMissionUI();
            }
        }
    }

    // Crea las listas visuales de misiones activas y completadas
    public void PopulateMissionList()
    {
        // Limpia las listas anteriores
        foreach (Transform child in missionListActiveContent)
            Destroy(child.gameObject);

        foreach (Transform child in missionListCompletedContent)
            Destroy(child.gameObject);

        // Crear entradas para las misiones activas
        foreach (var mission in MissionManager.Instance.activeMissions)
            CreateMissionEntry(mission, false, missionListActiveContent);

        // Crear entradas para las misiones completadas
        foreach (var mission in MissionManager.Instance.completedMissions)
            CreateMissionEntry(mission, true, missionListCompletedContent);
    }

    // Crea una entrada visual para una misión (con botón)
    void CreateMissionEntry(Mission mission, bool isCompleted, Transform parent)
    {
        GameObject entry = Instantiate(missionEntryPrefab, parent);
        TextMeshProUGUI text = entry.GetComponentInChildren<TextMeshProUGUI>();
        text.text = mission.localizedName + (isCompleted ? " (Completada)" : "");

        Button btn = entry.GetComponent<Button>();
        if (btn != null)
        {
            // Mostrar detalles al hacer clic
            btn.onClick.AddListener(() =>
            {
                ShowMissionDetails(mission);
            });
        }
    }

    // Muestra el título y la descripción de la misión seleccionada
    void ShowMissionDetails(Mission mission)
    {
        Debug.Log($"[UI] Mostrar detalles de: {mission.localizedName}");

        titleText.text = mission.localizedName;
        descriptionText.text = mission.localizedDesc;
    }

    // Reconstruye completamente la UI
    public void RefreshMissionUI()
    {
        PopulateMissionList();
    }
}
