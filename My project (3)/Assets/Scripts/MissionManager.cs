using UnityEngine;
using System.Collections.Generic;

//  Controla misiones activas, completadas y secuenciales
public class MissionManager : MonoBehaviour
{
    // Singleton para acceso global
    public static MissionManager Instance;

    [SerializeField] public List<Mission> activeMissions = new List<Mission>();
    [SerializeField] public List<Mission> completedMissions = new List<Mission>();

    public MissionDatabase missionDatabase; // Base de datos

    void Awake()
    {
        // asegura que solo haya una instancia
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Añade una misión por ID desde la base de datos, si no está ya activa o completada
    public void AddMissionById(string id)
    {
        if (activeMissions.Exists(m => m.id == id) || completedMissions.Exists(m => m.id == id)) return;

        // Busca la misión en la base de datos
        Mission mission = missionDatabase.GetMissionById(id);
        if (mission != null)
        {
            // Crea una copia de la misión para no modificar la original
            Mission newMission = new Mission(mission.id, mission.missionName, mission.description)
            {
                requieredItemName = mission.requieredItemName,
                requieredCrafting = mission.requieredCrafting,
                coinReward = mission.coinReward,
                nextMissionId = mission.nextMissionId,
                keyName = mission.keyName,
                keyDescription = mission.keyDescription
            };

            newMission.Activate(); // Activamos la mision
            newMission.Translate(); // La traducimos
            activeMissions.Add(newMission); // Añadimos la copia de la mision a misiones activas
            Debug.Log("Misión activada desde base de datos: " + newMission.missionName);
        
            // Actualiza la UI si está activa
            var ui = FindFirstObjectByType<MissionUI>();
            if (ui != null && ui.gameObject.activeSelf)
            {
                ui.RefreshMissionUI();
            }
        }
        else
        {
            Debug.LogWarning("No se encontró la misión con ID: " + id);
        }
    }

    // Marca una misión como completada y activa la siguiente (si tiene)
    public void CompleteMission(string missionId)
    {
        Mission mission = activeMissions.Find(m => m.id == missionId);
        if (mission != null)
        {
            mission.Complete();
            activeMissions.Remove(mission);
            completedMissions.Add(mission);

            // Recompensa al jugador    
            PlayerAtribute player = FindFirstObjectByType<PlayerAtribute>();
            if (player != null)
            {
                player.coins += mission.coinReward;
            }

            // Activar siguiente misión desde base de datos
            if (!string.IsNullOrEmpty(mission.nextMissionId))
            {
                AddMissionById(mission.nextMissionId);
            }

            MissionUI missionUI = FindFirstObjectByType<MissionUI>();
            if (missionUI != null)
            {
                missionUI.PopulateMissionList(); // O llama directo al método si es público
            }
        }
    }

    // Devuelve una misión activa o completada según su ID
    public Mission GetMissionById(string id)
    {
        return activeMissions.Find(m => m.id == id) ?? completedMissions.Find(m => m.id == id);
    }

    // Carga listas de misiones desde datos guardados
    public void LoadMissionsFromSave(List<Mission> savedActive, List<Mission> savedCompleted)
    {
        activeMissions = new List<Mission>(savedActive);
        completedMissions = new List<Mission>(savedCompleted);

        // Volver a traducir todas las misiones
        foreach (var mission in activeMissions)
        {
            mission.Translate();
        }

        foreach (var mission in completedMissions)
        {
            mission.Translate();
        }

        // Actualizar UI
        var ui = FindFirstObjectByType<MissionUI>();
        if (ui != null && ui.gameObject.activeSelf)
        {
            ui.RefreshMissionUI();
        }
    }

}
